using System.Buffers.Binary;
using System.IO;
using System.Diagnostics;
using System.Threading;
using DvdNavigatorCrm;

namespace DvdNavigatorCrm;
public class TitleSaver
{
    const int MaxPsmLength = 1024;
    const int MinimumMillisecondsChapterLength = 10000;

    DvdTitleSet titleSet;
    DvdTitle title;
    ICollection<int> angles;
    string ifoFileBase;
    List<long> vobFileSizes = [];
    List<AudioStreamItem> audioStreams = [];
    List<int> audioStreamsInOrderFound = [];
    ISubtitleStorage storage;
    FileStream fsMpeg;
    StreamWriter chaptersWriter;
    byte[] programStreamMap;
    //bool psmAdded;
    Action<string> updateMethod;
    int totalPacketsToSave;
    int packetsSaved;
    bool stopRun;
    double? startPts;
    double? endPts;
    double previousCellPtsEnd;
    double? chainPtsOffset;
    long? maximumLength;
    const long MaxBytesPerMilliSecond = 2000;
    PartialSaveStatus partialSaveStatus;
    bool tooManyBytesRead;
    List<double> chapterOffsets = [];
    double chapterFunnyBusiness = 0;
    SortedDictionary<int, long> audioLengths = new SortedDictionary<int, long>();
    class TitleChainData
    {
        public TitleChainData(SaverCellType cellType)
        {
            this.Chunks = [];
            this.AngleCellIds = [];
            this.VobCellIds = [];
            this.CellType = cellType;
        }

        public IList<TitleChunk> Chunks { get; private set; }
        public IList<CellIdVobId> AngleCellIds { get; private set; }
        public IList<CellIdVobId> VobCellIds { get; private set; }
        public SaverCellType CellType { get; set; }
        public float PlaybackTime { get; set; }
    }

    public TitleSaver(DvdTitleSet titleSet, int titleIndex, DvdTitle title, IEnumerable<int> angles, 
        string storageFileName, string mpegFileName, 
        string chapterFileName, double? startPts, double? endPts) :
        this(titleSet, titleIndex, title, angles, titleSet.Titles[titleIndex].AudioStreams,
        storageFileName, mpegFileName, chapterFileName, startPts, endPts)
    {
    }

    public TitleSaver(DvdTitleSet titleSet, int titleIndex, DvdTitle title, IEnumerable<int> angles, 
        IEnumerable<int> audioStreamIds, string storageFileName, string mpegFileName, 
        string chapterFileName, double? startPts, double? endPts)
    {
        this.titleSet = titleSet;
        this.title = title;
        this.angles = new HashSet<int>(angles);
        string dvdPath = Path.GetDirectoryName(this.titleSet.FileName);
        this.ifoFileBase = Path.Combine(dvdPath, Path.GetFileNameWithoutExtension(this.titleSet.FileName).Remove(7));
        int ifoNumber = Int32.Parse(Path.GetFileNameWithoutExtension(this.titleSet.FileName).Substring(4, 2));
        this.startPts = startPts;
        this.endPts = endPts;
        if(this.startPts.HasValue != this.endPts.HasValue)
        {
            throw new ArgumentException("startPts and endPts must either both have values or neither");
        }
        if(this.startPts.HasValue)
        {
            this.maximumLength = Convert.ToInt64((this.endPts.Value - this.startPts.Value) * MaxBytesPerMilliSecond);
        }

        if(!String.IsNullOrEmpty(storageFileName))
        {
            this.storage = SubtitleStorage.CreateWriter(storageFileName);
            int angle = 0;
            foreach(int nextAngle in this.angles)
            {
                if(nextAngle != 0)
                {
                    angle = nextAngle;
                    break;
                }
            }
            this.storage.AddHeader(dvdPath, ifoNumber, titleIndex, angle);
            foreach(int streamId in this.title.SubtitleStreams)
            {
                SubpictureAttributes subAttributes = this.title.GetSubtitleStream(streamId);
                this.storage.AddStream(streamId, subAttributes);
            }
        }

        if(!String.IsNullOrEmpty(mpegFileName))
        {
            // truncate the file
            this.fsMpeg = File.Create(mpegFileName);
        }

        if(!String.IsNullOrEmpty(chapterFileName))
        {
            // truncate the file
            this.chaptersWriter = File.CreateText(chapterFileName);
        }

        for(int vobIndex = 1; vobIndex <= 9; vobIndex++)
        {
            string vobName = $"{ifoFileBase}{vobIndex}.VOB";
            if(File.Exists(vobName))
            {
                this.vobFileSizes.Add(new FileInfo(vobName).Length);
            }
            else
            {
                break;
            }
        }

        foreach(int streamId in audioStreamIds)
        {
            AudioAttributes audioAttributes = this.title.GetAudioStream(streamId);
            this.audioStreams.Add(new AudioStreamItem(streamId, audioAttributes));
        }

        if(this.startPts.HasValue && (this.startPts.Value > 0))
        {
            this.partialSaveStatus = PartialSaveStatus.BeforeStart;
        }
        else
        {
            this.partialSaveStatus = PartialSaveStatus.InRange;
        }

        this.programStreamMap = BuildProgramStreamMap();
    }

    public void StopRun()
    {
        this.stopRun = true;
    }

    public void Run(Action<string> updater)
    {
        this.updateMethod = updater;

        try
        {
            //this.psmAdded = false;
            List<TitleChainData> chains = [];
            TitleChainData chainData = new TitleChainData(SaverCellType.First);
            int oldProgramChainIndex = -1;
            this.TotalLength = 0L;
            foreach(TitleCell cell in this.title.TitleCells)
            {
                if(this.angles.Contains(cell.CellAngle))
                {
                    bool isDiscontinuity = false;
                    if(this.TotalLength != 0L)
                    {
                        if(cell.Cell.IsStcDiscontinuity || (cell.ProgramChainIndex != oldProgramChainIndex))
                        {
                            isDiscontinuity = true;
                        }
                    }
                    oldProgramChainIndex = cell.ProgramChainIndex;

                    if(chainData.Chunks.Count != 0)
                    {
                        if(isDiscontinuity)
                        {
                            chainData.CellType |= SaverCellType.Last;
                            chains.Add(chainData);
                            chainData = new TitleChainData(SaverCellType.First);
                        }
                        else
                        {
                            chains.Add(chainData);
                            chainData = new TitleChainData(SaverCellType.None);
                        }
                        chainData.PlaybackTime = 0.0f;
                    }

                    chainData.VobCellIds.Add(new CellIdVobId(cell.Cell.CellId, cell.Cell.VobId));
                    if(cell.CellAngle != 0)
                    {
                        chainData.AngleCellIds.Add(new CellIdVobId(cell.Cell.CellId, cell.Cell.VobId));
                    }

                    long cellStart = (long)cell.Cell.FirstVobuStartSector * 0x800L;
                    long cellEnd = (long)(cell.Cell.LastVobuEndSector + 1) * 0x800L;
                    AddChunksFromCell(chainData.Chunks, cellStart, cellEnd, vobFileSizes, ifoFileBase, isDiscontinuity, cell, this.TotalLength);
                    this.TotalLength += (cellEnd - cellStart);
                    chainData.PlaybackTime = cell.Cell.PlaybackTime;
                }
                else
                {
                    if(cell.CellAngle == 0)
                    {
                        oldProgramChainIndex = -1;
                    }
                }
            }
            if(chainData.Chunks.Count != 0)
            {
                chainData.CellType |= SaverCellType.Last;
                chains.Add(chainData);
            }

            this.stopRun = false;
            this.tooManyBytesRead = false;
            this.chainPtsOffset = null;
            this.TotalRead = 0L;
            foreach(TitleChainData chain in chains)
            {
                LoadAndSaveChain(chain.CellType, chain.Chunks, chain.VobCellIds, chain.AngleCellIds, chain.PlaybackTime);
                if(IsLoadStopped() || (this.partialSaveStatus == PartialSaveStatus.AfterEnd))
                {
                    break;
                }
            }

            if(this.chaptersWriter != null)
            {
                int chapterNumber = 1;
                double lastChapter = -MinimumMillisecondsChapterLength - 1;
                foreach(double chapterOffset in this.chapterOffsets)
                {
                    if(chapterOffset >= lastChapter + MinimumMillisecondsChapterLength)
                    {
                        TimeSpan timeStart = new TimeSpan(Convert.ToInt64(chapterOffset) * 10000L);
                        string timeLine = $"CHAPTER{chapterNumber:d2}={timeStart.Hours:d2}:{timeStart.Minutes:d2}:{timeStart.Seconds:d2}.{timeStart.Milliseconds:d3}";
                        this.chaptersWriter.WriteLine(timeLine);
                        string nameLine = $"CHAPTER{chapterNumber:d2}NAME=Chapter {chapterNumber}";
                        this.chaptersWriter.WriteLine(nameLine);
                        chapterNumber++;
                        lastChapter = chapterOffset;
                    }
                }
            }

            if(this.storage != null)
            {
                List<AudioStreamItem> audioInOrder = [];
                foreach(int streamId in this.audioStreamsInOrderFound)
                {
                    audioInOrder.AddRange(this.audioStreams.Where(
                        item => item.StreamId == streamId));
                }
                this.storage.AddVideoAudioInfo(this.titleSet.VideoAttributes, audioInOrder);
            }
        }
        finally
        {
            this.storage?.Close();
            this.fsMpeg?.Dispose();
            this.chaptersWriter?.Dispose();
        }

        foreach(KeyValuePair<int, long> entry in this.audioLengths)
        {
            Debug.WriteLine($"Stream {entry.Key:x} had {entry.Value} bytes");
        }
    }

    static void AddChunksFromCell(IList<TitleChunk> chunks, long cellStart, long cellEnd, IList<long> vobFileSizes,
        string ifoFileBase, bool isDiscontinuity, TitleCell cell, long totalLength)
    {
        long cellLength = cellEnd - cellStart;
        VobNumber vob = VobNumber.Calculate(vobFileSizes, cellStart);
        string ifoFilePath = $"{ifoFileBase}{vob.IfoFileNumber}.VOB";

        while(cellLength > vob.IfoRemainder)
        {
            chunks.Add(new TitleChunk(ifoFilePath, vob.IfoOffset, vob.IfoRemainder,
                totalLength, isDiscontinuity, cell.ProgramChain.Palette,
                cell.CellAngle));
            totalLength += vob.IfoRemainder;
            isDiscontinuity = false;
            cellLength -= vob.IfoRemainder;
            cellStart += vob.IfoRemainder;
            vob = VobNumber.Calculate(vobFileSizes, cellStart);
            ifoFilePath = $"{ifoFileBase}{vob.IfoFileNumber}.VOB";
        }

        chunks.Add(new TitleChunk(ifoFilePath, vob.IfoOffset, (int)cellLength,
            totalLength, isDiscontinuity, cell.ProgramChain.Palette,
            cell.CellAngle));
    }

    void loader_BytesRead(object sender, LoadedBytesEventArgs e)
    {
        this.TotalRead += e.ByteCount;
        this.updateMethod($"{this.TotalRead >> 20}M of {this.TotalLength >> 20}M read");
        if(this.maximumLength.HasValue && (this.TotalRead > this.maximumLength.Value))
        {
            this.tooManyBytesRead = true;
        }
    }

    void saver_SavedPacket(object sender, EventArgs e)
    {
        this.packetsSaved++;
        if((this.packetsSaved % 1024) == 0)
        {
            this.updateMethod($"{this.TotalRead >> 20}M of {this.TotalLength >> 20}M read, {this.packetsSaved >> 10}k of {this.totalPacketsToSave >> 10}k packets written");
        }
    }

    public long TotalRead { get; private set; }
    public long TotalLength { get; private set; }

    bool IsLoadStopped()
    {
        return this.stopRun || this.tooManyBytesRead;
    }

    bool IsRunStopped()
    {
        return this.stopRun;
    }

    void LoadAndSaveChain(SaverCellType cellType, IList<TitleChunk> chunks, IList<CellIdVobId> vobCellIds, IList<CellIdVobId> angleCellIds, float playbackTime)
    {
        long previousTotalRead = this.TotalRead;
        CellLoader loader = new CellLoader(vobCellIds, angleCellIds, this.audioStreams);
        loader.BytesRead += this.loader_BytesRead;
        loader.Run(chunks, new Func<bool>(this.IsLoadStopped));
        loader.BytesRead -= this.loader_BytesRead;

        // skip cells without both audio and video. They're just trouble - messing up synchronization no end
        if(this.stopRun || !loader.FirstAudioPts.HasValue || !loader.FirstVideoPts.HasValue)
        {
            this.TotalRead = previousTotalRead;
            return;
        }

        foreach(int streamId in loader.AudioStreamsFound)
        {
            if(!this.audioStreamsInOrderFound.Contains(streamId))
            {
                this.audioStreamsInOrderFound.Add(streamId);
            }
        }

        foreach(KeyValuePair<int, long> entry in loader.AudioLengths)
        {
            long lastLength;
            this.audioLengths.TryGetValue(entry.Key, out lastLength);
            this.audioLengths[entry.Key] = lastLength + entry.Value;
        }

        /*if(!this.psmAdded)
        {
            for(int index = 0; index < this.loader.Packets.Count; index++)
            {
                StreamPackHeaderBuffer packet = this.loader.Packets[index] as StreamPackHeaderBuffer;
                if(packet != null)
                {
                    if(index < this.loader.Packets.Count - 1)
                    {
                        HeaderPacketBuffer systemHeader = this.loader.Packets[index + 1] as HeaderPacketBuffer;
                        if(systemHeader != null)
                        {
                            index++;
                        }
                    }
                    this.loader.Packets.Insert(index + 1,
                        new HeaderPacketBuffer() { Data = this.programStreamMap, PacketTypeCode = MpegPSSplitter.ProgramStreamMapCode });
                    this.psmAdded = true;
                    //index++;
                    break;
                }
            }
        }*/

        if(!this.chainPtsOffset.HasValue)
        {
            // start our new mpeg file at 0 timestamp
            this.chainPtsOffset = -loader.FirstPts.Value;
        }
        else
        {
            if((cellType & SaverCellType.First) == SaverCellType.First)
            {
                this.chainPtsOffset = this.previousCellPtsEnd - loader.FirstPts.Value;
            }
            else
            {
                if(Math.Abs(this.previousCellPtsEnd - loader.FirstPts.Value) >= 500.0)
                {
                    // cells in a chain shouldn't be discontinuous, but DVD authors are evil,
                    // so if the jump is 1 second or more make an adjustment
                    this.chainPtsOffset += (this.previousCellPtsEnd - loader.FirstPts.Value);
                    this.chapterFunnyBusiness += (this.previousCellPtsEnd - loader.FirstPts.Value);
                }
            }
        }

        long filePosition = 0;
        if(this.storage != null)
        {
            if(this.fsMpeg != null)
            {
                this.fsMpeg.Flush();
                filePosition = this.fsMpeg.Position;
            }
            this.storage.AddCellStartOffsets(this.chainPtsOffset.Value, cellType, filePosition, loader.FirstPts.Value, 
                loader.FirstAudioPts.Value, loader.FirstVideoPts.Value);
        }

        Debug.WriteLine($"Cell {cellType} Offset {this.chainPtsOffset:f2} First {loader.FirstPts.Value:f2} Video {loader.FirstVideoPts.Value:f2} Audio {loader.FirstAudioPts.Value:f2} Last {loader.LastPts.Value:f2} FilePos {filePosition}");

        CellSaver saver = new CellSaver(this.chainPtsOffset.Value, this.partialSaveStatus, this.startPts, this.endPts);
        this.totalPacketsToSave = loader.Packets.Count;
        this.packetsSaved = 0;
        saver.SavedPacket += this.saver_SavedPacket;
        saver.Run(loader.Packets, this.fsMpeg, this.storage, new Func<bool>(this.IsRunStopped));
        saver.SavedPacket -= this.saver_SavedPacket;
        loader.ClearPackets();
        if(saver.FirstPackHeaderPts.HasValue)
        {
            this.chapterOffsets.Add(saver.FirstPackHeaderPts.Value - this.chapterFunnyBusiness);
        }
        this.partialSaveStatus = saver.PartialSaveStatus;

        if((cellType & SaverCellType.Last) == SaverCellType.Last)
        {
            // if we're ending a chain, store the adjusted pts since the next chain will be 
            // discontinuous anyway
            this.previousCellPtsEnd = loader.LastPts.Value + this.chainPtsOffset.Value;
        }
        else
        {
            this.previousCellPtsEnd = loader.LastPts.Value;
        }
    }

    static uint[] crc32_table = new uint[256];

    void InitializeCrc()
    {
        uint i, j, k;
        for(i = 0; i < 256; i++)
        {
            k = 0;
            for(j = (i << 24) | 0x800000; j != 0x80000000; j <<= 1)
            {
                k = (k << 1) ^ ((((k ^ j) & 0x80000000) != 0) ? (uint)0x04c11db7 : 0);
            }
            crc32_table[i] = k;
        }
    }

    byte[] BuildProgramStreamMap()
    {
        InitializeCrc();

        int psmLength = 10;
        byte[] psm = new byte[MaxPsmLength];
        psm[0] = 0;
        psm[1] = 0;
        psm[2] = 1;
        psm[3] = MpegPSSplitter.ProgramStreamMapCode;
        psm[6] = 0xe2;
        psm[7] = 0xff;

        int infoLength = 0;
        BinaryPrimitives.WriteUInt16BigEndian(psm.AsSpan(8), (ushort)infoLength);
        psmLength += infoLength;

        int esMapLength = 0;
        int esMapOffset = 12 + infoLength;

        // fill in the video stream
        psm[esMapOffset] = 0x02;
        psm[esMapOffset + 1] = 0xe0;
        psm[esMapOffset + 2] = 0;
        psm[esMapOffset + 3] = 0;
        esMapLength += 4;

        foreach(int streamId in this.title.AudioStreams)
        {
            esMapOffset = 12 + infoLength + esMapLength;
            AudioAttributes audioAttributes = this.title.GetAudioStream(streamId);

            switch(audioAttributes.CodingMode)
            {
            case AudioCodingMode.MPEG1:
            case AudioCodingMode.MPEG2:
                psm[esMapOffset] = 0x04;
                psm[esMapOffset + 1] = (byte)(streamId & 0xff);
                break;
            case AudioCodingMode.AC3:
            case AudioCodingMode.DTS:
            case AudioCodingMode.LPCM:
            default:
                psm[esMapOffset] = 0x81;
                //psm[esMapOffset + 1] = (byte)(streamId & 0xff);
                psm[esMapOffset + 1] = 0x81;
                break;
            }
            if((audioAttributes.Language == null) || (audioAttributes.Language.Length < 2))
            {
                psm[esMapOffset + 2] = 0;
                psm[esMapOffset + 3] = 0;
                esMapLength += 4;
            }
            else
            {
                psm[esMapOffset + 2] = 0;
                psm[esMapOffset + 3] = 6;
                psm[esMapOffset + 4] = 0x0a;
                psm[esMapOffset + 5] = 4;

                byte[] lang;
                if(audioAttributes.Language.Length > 2)
                {
                    lang = Encoding.ASCII.GetBytes(audioAttributes.Language);
                }
                else
                {
                    string languageCode = DvdLanguageCodes.GetLanguage639Code(
                        audioAttributes.Language);
                    lang = Encoding.ASCII.GetBytes(languageCode);
                }

                psm[esMapOffset + 6] = lang[0];
                psm[esMapOffset + 7] = lang[1];
                psm[esMapOffset + 8] = lang[2];

                psm[esMapOffset + 9] = 0;
                esMapLength += 10;
            }
        }
        
        BinaryPrimitives.WriteUInt16BigEndian(psm.AsSpan(10 + infoLength), (ushort)esMapLength);
        psmLength += esMapLength;

        BinaryPrimitives.WriteUInt16BigEndian(psm.AsSpan(4), (ushort)psmLength);

        byte[] realPsm = new byte[psmLength + 6];
        Buffer.BlockCopy(psm, 0, realPsm, 0, realPsm.Length);

        realPsm[realPsm.Length - 4] = 0;
        realPsm[realPsm.Length - 3] = 0;
        realPsm[realPsm.Length - 2] = 0;
        realPsm[realPsm.Length - 1] = 0;

        uint crc = 0xffffffff;
        for(int i = 0; i < realPsm.Length; i++)
        {
            crc = (crc << 8) ^ crc32_table[((crc >> 24) ^ realPsm[i]) & 0xff];
        }

        BinaryPrimitives.WriteUInt32BigEndian(realPsm.AsSpan(realPsm.Length - 4), crc);
        return realPsm;
    }
}

