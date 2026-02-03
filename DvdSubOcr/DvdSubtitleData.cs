using System.Buffers.Binary;
using System.Diagnostics;
using DvdNavigatorCrm;

namespace DvdSubOcr;
public class DvdSubtitleData : SubtitleInformation, ISubtitleData
{
    bool? isEmpty;
    IList<int> yuvPalette;
    byte[] data;

    public DvdSubtitleData(int streamId, IList<int> yuvPalette, byte[] data, double pts) :
        base(Decode(data, pts, yuvPalette, true))
    {
        this.StreamId = streamId;
        this.yuvPalette = yuvPalette;
        this.data = data;
        if(this.Size.IsEmpty)
        {
            this.isEmpty = true;
        }
    }

    public int StreamId { get; private set; }

    public bool TestIfEmpty()
    {
        if(!this.isEmpty.HasValue)
        {
            if(this.data.Length < 100)
            {
                using(SubtitleBitmap subtitle = DecodeBitmap())
                {
                    List<int> validColors = [];
                    for(int index = 0; index < 4; index++)
                    {
                        if(subtitle.Bitmap.Palette.Entries[index].A != 0)
                        {
                            validColors.Add(index);
                        }
                    }

                    if(validColors.Count == 0)
                    {
                        return true;
                    }

                    ContiguousEncode encode = new ContiguousEncode(subtitle.Data,
                        subtitle.Bitmap.Width, subtitle.Bitmap.Height, subtitle.Stride);
                    this.isEmpty = (encode.FindEncodes(validColors, null).Count == 0);
                }
            }
            else
            {
                this.isEmpty = false;
            }
        }
        return this.isEmpty.Value;
    }

    public SubtitleBitmap DecodeBitmap()
    {
        return Decode(this.data, this.Pts, this.yuvPalette, false) as SubtitleBitmap;
    }

    private static SubtitleInformation Decode(byte[] byteBuffer, double currentPts, IList<int> yuvPalette,
        bool onlyDecodeHeaderInformation)
    {
        int dataLength = BinaryPrimitives.ReadUInt16BigEndian(byteBuffer.AsSpan(0));
        int controlSequenceOffset = BinaryPrimitives.ReadUInt16BigEndian(byteBuffer.AsSpan(2));
        int[] paletteIndices;

        int startTime = 0, endTime = 0;
        int x1 = 0, x2 = 0, y1 = 0, y2 = 0;
        int topDataOffset = 0, bottomDataOffset = 0;

        Dictionary<int, int[]> paletteIndicesOverTime = new Dictionary<int, int[]>();
        Dictionary<int, int[]> paletteAlphasOverTime = new Dictionary<int, int[]>();
        int nextControl = controlSequenceOffset;
        bool isForced = false;
        while(nextControl + 4 < dataLength)
        {
            int time = (BinaryPrimitives.ReadUInt16BigEndian(byteBuffer.AsSpan(nextControl)) * 1024 + 89) / 90;
            int afterNextControl = BinaryPrimitives.ReadUInt16BigEndian(byteBuffer.AsSpan(nextControl + 2));
            if(afterNextControl == nextControl)
            {
                afterNextControl = dataLength;
            }
            bool endCodeFound = false;
            nextControl += 4;
            while((nextControl < afterNextControl) && !endCodeFound)
            {
                int colors;
                int offset;
                int[] indices;
                int[] alphas;
                int control = byteBuffer[nextControl++];
                switch(control)
                {
                case 0:
                    startTime = Math.Min(startTime, time);
                    isForced = true;
                    break;
                case 1:
                    startTime = Math.Min(startTime, time);
                    break;
                case 2:
                    endTime = Math.Max(time, endTime);
                    break;
                case 3:
                    if(!paletteIndicesOverTime.TryGetValue(time, out indices))
                    {
                        indices = new int[4];
                        paletteIndicesOverTime[time] = indices;
                    }
                    colors = byteBuffer[nextControl++];
                    indices[3] = colors >> 4;
                    indices[2] = colors & 0x0f;
                    colors = byteBuffer[nextControl++];
                    indices[1] = colors >> 4;
                    indices[0] = colors & 0x0f;
                    break;
                case 4:
                    if(!paletteAlphasOverTime.TryGetValue(time, out alphas))
                    {
                        alphas = new int[4];
                        paletteAlphasOverTime[time] = alphas;
                    }
                    colors = byteBuffer[nextControl++];
                    alphas[3] = colors >> 4;
                    alphas[2] = colors & 0x0f;
                    colors = byteBuffer[nextControl++];
                    alphas[1] = colors >> 4;
                    alphas[0] = colors & 0x0f;
                    for(int index = 0; index < 4; index++)
                    {
                        alphas[index] += alphas[index] << 4;
                    }
                    break;
                case 5:
                    x1 = byteBuffer[nextControl++] << 4;
                    offset = byteBuffer[nextControl++];
                    x1 += offset >> 4;
                    x2 = (offset & 0x0f) << 8;
                    x2 += byteBuffer[nextControl++];
                    y1 = byteBuffer[nextControl++] << 4;
                    offset = byteBuffer[nextControl++];
                    y1 += offset >> 4;
                    y2 = (offset & 0x0f) << 8;
                    y2 += byteBuffer[nextControl++];
                    //Debug.WriteLine(string.Format("x1 {0} x2 {1} y1 {2} y2 {3}", 
                    //	x1, x2, y1, y2));
                    break;
                case 6:
                    topDataOffset = BinaryPrimitives.ReadUInt16BigEndian(byteBuffer.AsSpan(nextControl));
                    nextControl += 2;
                    bottomDataOffset = BinaryPrimitives.ReadUInt16BigEndian(byteBuffer.AsSpan(nextControl));
                    nextControl += 2;
                    break;
                case 7:
                    break;
                case 0xff:
                    endCodeFound = true;
                    break;
                default:
                    break;
                }
            }
            nextControl = afterNextControl;
        }

        // find the palette with the highest alpha.  We're not going to do fade-in and fade-out on
        // subtitles, we're just going to show them at their max alpha for the whole time
        int timeAtMaxAlpha = -1;
        int[] paletteAlphas = null;
        if(paletteAlphasOverTime.Count != 0)
        {
            int maxAlpha = -1;
            foreach(KeyValuePair<int, int[]> alphaPair in paletteAlphasOverTime)
            {
                int combinedAlpha = alphaPair.Value[0] + alphaPair.Value[1] + alphaPair.Value[2] + alphaPair.Value[3];
                if(combinedAlpha > maxAlpha)
                {
                    maxAlpha = combinedAlpha;
                    timeAtMaxAlpha = alphaPair.Key;
                    paletteAlphas = alphaPair.Value;
                }
            }

        }
        else
        {
            Debug.WriteLine("No Palette Alphas!!");
            paletteAlphas = [0xff, 0xff, 0xff, 0xff];
        }

        if(timeAtMaxAlpha == -1)
        {
            Debug.WriteLine("timeAtMaxAlpha == -1");
            //return;
        }

        if(!paletteIndicesOverTime.TryGetValue(timeAtMaxAlpha, out paletteIndices))
        {
            if(paletteIndicesOverTime.Count != 0)
            {
                paletteIndices = paletteIndicesOverTime[0];
            }
            else
            {
                Debug.WriteLine("No Palette Indexes!!");
                if(paletteIndices == null)
                {
                    Debug.WriteLine("No Palette Indexes EVER!!");
                    return null;
                }
            }
        }

        //Debug.WriteLine(string.Format("StartTime {0} EndTime {1} pts {2}", 
        //	startTime, endTime, currentPts));

        Rectangle bitmapRect = new Rectangle(x1, y1, x2 - x1 + 1, y2 - y1 + 1);
        Rectangle zeroedBitmapRect = new Rectangle(0, 0, x2 - x1 + 1, y2 - y1 + 1);

        //bool noEndTime = false;
        if(endTime == startTime)
        {
            //noEndTime = true;
            //Debug.WriteLine("DvdSubDecoder::CreateSubPicture FAKE END TIME of +n Seconds");
            //Debug.WriteLine("No End Time");
            endTime = startTime + SubConstants.MaximumMillisecondsOnScreen;
        }
        if(bitmapRect.IsEmpty || (endTime - startTime <= 0) || (topDataOffset == 0) || (bottomDataOffset == 0))
        {
            Debug.WriteLine($"Invalid SubPicture found: {bitmapRect} Times {startTime} {endTime} Offsets {topDataOffset} {bottomDataOffset}");
            return null;
        }

        //Debug.WriteLine(string.Format("StartTime {0} EndTime {1} pts {2}", startTime, endTime, currentPts));

        if(yuvPalette.Count != 16)
        {
            throw new ArgumentException("YuvPalette");
        }

        Color[] rgbPalette = new Color[yuvPalette.Count];
        for(int index = 0; index < yuvPalette.Count; index++)
        {
            rgbPalette[index] = RGBFromYUV(yuvPalette[index] & 0xffffff);
        }

        Color[] bmpPalette = new Color[4];
        for(int index = 0; index < 4; index++)
        {
            bmpPalette[index] = Color.FromArgb(paletteAlphas[index],
                rgbPalette[paletteIndices[index]]);
        }

        if(onlyDecodeHeaderInformation)
        {
            return new SubtitleInformation(x1, y1, bitmapRect.Width, bitmapRect.Height,
                currentPts + startTime, Convert.ToSingle(endTime - startTime),
                bmpPalette, isForced);
        }

        SubtitleBitmap buffer = new SubtitleBitmap(x1, y1, bitmapRect.Width, bitmapRect.Height,
            currentPts + startTime, Convert.ToSingle(endTime - startTime),
            bmpPalette, isForced);

        unsafe
        {
            //byte[] tempBuffer = new byte[buffer.Stride * buffer.Height];
            byte[] topNibbles = new byte[(bottomDataOffset - topDataOffset) * 2];
            byte[] bottomNibbles = new byte[(controlSequenceOffset - bottomDataOffset) * 2];

            //Debug.WriteLine(string.Format("Top {0} bytes Bottom {1} bytes", topNibbles.Length,
            //	bottomNibbles.Length));

            fixed(byte* ptrTop = topNibbles, ptrBottom = bottomNibbles, ptrByteBuffer = byteBuffer/*,
                outBuffer = tempBuffer*/)
            {
                byte* srcTop = ptrByteBuffer + topDataOffset;
                byte* nibbledTop = ptrTop;
                for(int count = bottomDataOffset - topDataOffset; count != 0; count--)
                {
                    *nibbledTop++ = (byte)(*srcTop >> 4);
                    *nibbledTop++ = (byte)(*srcTop++ & 0x0f);
                }

                byte* srcBottom = ptrByteBuffer + bottomDataOffset;
                byte* nibbledBottom = ptrBottom;
                for(int count = controlSequenceOffset - bottomDataOffset; count != 0; count--)
                {
                    *nibbledBottom++ = (byte)(*srcBottom >> 4);
                    *nibbledBottom++ = (byte)(*srcBottom++ & 0x0f);
                }

                nibbledTop = ptrTop;
                nibbledBottom = ptrBottom;

                byte* lineStart = (byte*)buffer.Data.ToPointer();
                //int* lineStart = (int*)outBuffer;
                for(int line = 0; line < zeroedBitmapRect.Height; line++)
                {
                    byte* dest = lineStart;
                    int bytesLeft = zeroedBitmapRect.Width;
                    int colorIndex = 0;
                    while(bytesLeft != 0)
                    {
                        int count;
                        byte* savedNibbleTop = nibbledTop;
                        if((*(uint*)(nibbledTop) & 0xc0f0f0f) == 0)
                        {
                            count = bytesLeft;
                            colorIndex = *(nibbledTop + 3) & 0x3;
                            nibbledTop += 4;
                        }
                        else
                        {
                            count = *nibbledTop & 0xc;
                            if(count != 0)
                            {
                                count >>= 2;
                                colorIndex = *nibbledTop++ & 0x3;
                            }
                            else
                            {
                                count = *nibbledTop++;
                                if(count != 0)
                                {
                                    count <<= 2;
                                    colorIndex = *nibbledTop++;
                                    count += colorIndex >> 2;
                                    colorIndex &= 0x3;
                                }
                                else
                                {
                                    count = *nibbledTop++;
                                    if(count >= 0x4)
                                    {
                                        count <<= 2;
                                        colorIndex = *nibbledTop++;
                                        count += colorIndex >> 2;
                                        colorIndex &= 0x3;
                                    }
                                    else
                                    {
                                        count <<= 6;
                                        count += (*nibbledTop++) << 2;
                                        colorIndex = *nibbledTop++;
                                        count += colorIndex >> 2;
                                        colorIndex &= 0x3;
                                    }
                                }
                            }
                        }

                        //int color = bmpPalette[colorIndex].ToArgb();
                        byte color = (byte)colorIndex;
                        bytesLeft -= count;
                        while(count != 0)
                        {
                            *dest++ = color;
                            count--;
                        }
                    }
                    nibbledTop += (((long)nibbledTop) & 1);

                    if(++line == zeroedBitmapRect.Height)
                    {
                        break;
                    }

                    lineStart += buffer.Stride;
                    dest = lineStart;

                    bytesLeft = zeroedBitmapRect.Width;
                    colorIndex = 0;
                    while(bytesLeft != 0)
                    {
                        int count;
                        byte* savedNibbleBottom = nibbledBottom;
                        if((*(uint*)(nibbledBottom) & 0xc0f0f0f) == 0)
                        {
                            count = bytesLeft;
                            colorIndex = *(nibbledBottom + 3) & 0x3;
                            nibbledBottom += 4;
                        }
                        else
                        {
                            count = *nibbledBottom & 0xc;
                            if(count != 0)
                            {
                                count >>= 2;
                                colorIndex = *nibbledBottom++ & 0x3;
                            }
                            else
                            {
                                count = *nibbledBottom++;
                                if(count != 0)
                                {
                                    count <<= 2;
                                    colorIndex = *nibbledBottom++;
                                    count += colorIndex >> 2;
                                    colorIndex &= 0x3;
                                }
                                else
                                {
                                    count = *nibbledBottom++;
                                    if(count >= 0x4)
                                    {
                                        count <<= 2;
                                        colorIndex = *nibbledBottom++;
                                        count += colorIndex >> 2;
                                        colorIndex &= 0x3;
                                    }
                                    else
                                    {
                                        count <<= 6;
                                        count += (*nibbledBottom++) << 2;
                                        colorIndex = *nibbledBottom++;
                                        count += colorIndex >> 2;
                                        colorIndex &= 0x3;
                                    }
                                }
                            }
                        }

                        //int color = bmpPalette[colorIndex].ToArgb();
                        byte color = (byte)colorIndex;
                        bytesLeft -= count;
                        while(count != 0)
                        {
                            *dest++ = color;
                            count--;
                        }
                    }
                    nibbledBottom += (((long)nibbledBottom) & 1);
                    lineStart += buffer.Stride;
                }

            }
        }
        return buffer;
    }

    public static int Clip(int colorPart)
    {
        return Math.Min(Math.Max(0, colorPart), 255);
    }

    public static Color RGBFromYUV(int yuv)
    {
        int y = (yuv >> 16) & 0xff;
        int u = (yuv >> 8) & 0xff;
        int v = yuv & 0xff;

        int c = y - 16;
        int d = u - 128;
        int e = v - 128;

        int r = (298 * c + 409 * d + 128) >> 8;
        int g = (298 * c - 100 * e - 208 * d + 128) >> 8;
        int b = (298 * c + 516 * e + 128) >> 8;

        //y = (y - 16) * 255 / 219;
        // VobSubFile.cpp
        //r = Convert.ToByte(Math.Min(Math.Max(1.0 * y + 1.4022 * (u - 128), 0), 255));
        //g = Convert.ToByte(Math.Min(Math.Max(1.0 * y - 0.3456 * (u - 128) - 0.7145 * (v - 128), 0), 255));
        //b = Convert.ToByte(Math.Min(Math.Max(1.0 * y + 1.7710 * (v - 128), 0), 255));

        // useBT601
        //r = Convert.ToByte(Math.Min(Math.Max(1.0 * y + 1.596026317 * (u - 128), 0), 255));
        //g = Convert.ToByte(Math.Min(Math.Max(1.0 * y - 0.8129674985 * (u - 128) - 0.3917615979 * (v - 128), 0), 255));
        //b = Convert.ToByte(Math.Min(Math.Max(1.0 * y + 2.017232218 * (v - 128), 0), 255));

        // BT.709
        //r = Convert.ToByte(Math.Min(Math.Max(1.0 * y + 1.792741071 * (u - 128), 0), 255));
        //g = Convert.ToByte(Math.Min(Math.Max(1.0 * y - 0.5329093286 * (u - 128) - 0.2132486143 * (v - 128), 0), 255));
        //b = Convert.ToByte(Math.Min(Math.Max(1.0 * y + 2.112401786 * (v - 128), 0), 255));

        return Color.FromArgb(255, Clip(r), Clip(g), Clip(b));
    }
}

