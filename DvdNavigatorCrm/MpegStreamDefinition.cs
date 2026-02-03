/*
 * Copyright (C) 2007, 2008, 2009 Chris Meadowcroft <crmeadowcroft@gmail.com>
 *
 * This file is part of CMPlayer, a free video player.
 * See http://sourceforge.net/projects/crmplayer for updates.
 *
 * CMPlayer is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 2 of the License, or
 * (at your option) any later version.
 *
 * CMPlayer is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
 */


namespace DvdNavigatorCrm;
class ProgramStreamMapEntry
{
    byte[] descriptor;

    public ProgramStreamMapEntry()
    {
    }

    public byte TypeCode { get; set; }
    public string Lang { get; set; }
    public byte[] GetDescriptor() { return this.descriptor; }
    public void SetDescriptor(byte[] newDescriptor) { this.descriptor = newDescriptor; }
    public long RegistrationId { get; set; }
}

class MpegStreamDefinition : StreamDefinition
{
    public const long AC3RegistrationId = 0x41432d33;
    public const long Dts1RegistrationId = 0x44545331;
    public const long Dts2RegistrationId = 0x44545332;
    public const long Dts3RegistrationId = 0x44545333;

    int subHeaderSize;

    public MpegStreamDefinition(int streamId) : base(streamId)
    {
    }

    public MpegStreamDefinition(int streamId, int programId)
        : base(streamId, programId)
    {
    }

    public static bool IsVideoStream(int id) { return (id & 0xf0) == 0xe0; }
    public static bool IsAudioStream(int id) { return (id & 0xe0) == 0xc0; }

    public int SubHeaderSize { get { return this.subHeaderSize; } }
    public bool RateLocked { get; set; }

    internal void FindPSStreamCodec(Dictionary<int, ProgramStreamMapEntry> programStreamMap)
    {
        if(IsAudioStream(this.StreamId))
        {
            this.StreamType = StreamType.Audio;
            this.Codec = Codec.MPGA;
            if(programStreamMap.TryGetValue(this.StreamId, out ProgramStreamMapEntry entry))
            {
                this.Language = entry.Lang;
                this.Codec = entry.TypeCode switch
                {
                    0x0f => Codec.MP4A,
                    0x03 or 0x04 => Codec.MPGA,
                    _ => this.Codec,
                };
            }
        }
        else if(IsVideoStream(this.StreamId))
        {
            this.StreamType = StreamType.Video;
            this.Codec = Codec.MPGV;
            if(programStreamMap.TryGetValue(this.StreamId, out ProgramStreamMapEntry entry2))
            {
                this.Language = entry2.Lang;
                this.Codec = entry2.TypeCode switch
                {
                    0x1b => Codec.H264,
                    0x10 => Codec.MP4V,
                    0x02 => Codec.MPGV,
                    _ => this.Codec,
                };
            }
        }
        else
        {
            ProgramStreamMapEntry entry;
            if(programStreamMap.TryGetValue(this.StreamId, out entry))
            {
                this.Language = entry.Lang;
            }
        }
    }

    internal void FindPSPrivateStreamCodec(Dictionary<int, ProgramStreamMapEntry> programStreamMap)
    {
        ProgramStreamMapEntry entry;
        if(programStreamMap.TryGetValue(this.StreamId, out entry))
        {
            this.Language = entry.Lang;
        }

        if((this.StreamId & 0xf8) == 0x88)
        {
            this.Codec = Codec.DTS;
            this.StreamType = StreamType.Audio;
            this.subHeaderSize = 3;
            return;
        }
        if((this.StreamId & 0xf0) == 0x80)
        {
            this.Codec = Codec.AC3;
            this.StreamType = StreamType.Audio;
            this.subHeaderSize = 3;
            return;
        }
        if((this.StreamId & 0xe0) == 0x20)
        {
            this.Codec = Codec.SPU;
            this.StreamType = StreamType.Subtitle;
            return;
        }
        if((this.StreamId & 0xf0) == 0xa0)
        {
            this.Codec = Codec.LPCM;
            this.StreamType = StreamType.Audio;
            return;
        }
        if((this.StreamId & 0xff) == 0x70)
        {
            this.Codec = Codec.OGT;
            this.StreamType = StreamType.Subtitle;
            return;
        }
        if((this.StreamId & 0xfc) == 0x00)
        {
            this.Codec = Codec.CVD;
            this.StreamType = StreamType.Subtitle;
            return;
        }
    }

    internal void FindTSStreamCodec(ProgramStreamMapEntry entry, int streamType)
    {
        this.Language = entry.Lang;

        switch(streamType)
        {
        case 0x02:  // MPEG-2 video
            this.StreamType = StreamType.Video;
            this.Codec = Codec.MPGV;
            return;
        case 0x03:  // MPEG-2 audio
        case 0x04:  // MPEG-2 audio
            this.StreamType = StreamType.Audio;
            this.Codec = Codec.MPGA;
            return;
        case 0x11:  // MPEG4 (audio)
        case 0x0f:  // ISO/IEC 13818-7 Audio with ADTS transport syntax
            this.StreamType = StreamType.Audio;
            this.Codec = Codec.MP4A;
            return;
        case 0x10:  // MPEG4 (video)
            this.StreamType = StreamType.Video;
            this.Codec = Codec.MP4V;
            return;
        case 0x1B:  // H264 <- check transport syntax/needed descriptor
            this.StreamType = StreamType.Video;
            this.Codec = Codec.H264;
            return;
        }

        if(IsAudioStream(this.StreamId))
        {
            this.StreamType = StreamType.Audio;
            this.Codec = entry.TypeCode switch
            {
                0x0f => Codec.MP4A,
                0x03 or 0x04 => Codec.MPGA,
                _ => Codec.MPGA,
            };
        }
        else if(IsVideoStream(this.StreamId))
        {
            this.StreamType = StreamType.Video;
            this.Codec = entry.TypeCode switch
            {
                0x1b => Codec.H264,
                0x10 => Codec.MP4V,
                0x02 => Codec.MPGV,
                _ => Codec.MPGV,
            };
        }
    }

    internal void FindTSPrivateStreamCodec(ProgramStreamMapEntry entry, int streamType)
    {
        this.Language = entry.Lang;

        switch(entry.RegistrationId)
        {
        case AC3RegistrationId:
            this.Codec = Codec.AC3;
            this.StreamType = StreamType.Audio;
            this.subHeaderSize = 3;
            return;
        case Dts1RegistrationId:
        case Dts2RegistrationId:
        case Dts3RegistrationId:
            this.Codec = Codec.DTS;
            this.StreamType = StreamType.Audio;
            this.subHeaderSize = 3;
            return;
        }

        switch(streamType)
        {
        case 0x81:
            this.Codec = Codec.AC3;
            this.StreamType = StreamType.Audio;
            this.subHeaderSize = 3;
            return;
        case 0x82:
            this.Codec = Codec.SPU;
            this.StreamType = StreamType.Subtitle;
            return;
        case 0x83:  // LPCM (audio)
            this.Codec = Codec.LPCM;
            this.StreamType = StreamType.Audio;
            return;
        case 0x85:  // DTS (audio)
            this.Codec = Codec.DTS;
            this.StreamType = StreamType.Audio;
            this.subHeaderSize = 3;
            return;
        }
    }
}

