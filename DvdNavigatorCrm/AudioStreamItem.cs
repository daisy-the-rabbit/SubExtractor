/*
 * Copyright (C) 2007, 2008 Christopher R Meadowcroft <crmeadowcroft@gmail.com>
 *
 * This file is part of DvdSubOcr, a free DVD Subtitle OCR program.
 * See for updates.
 *
 * DvdSubOcr is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 2 of the License, or
 * (at your option) any later version.
 *
 * DvdSubOcr is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
 */
using DvdNavigatorCrm;

namespace DvdNavigatorCrm;

public class AudioStreamItem : IComparable<AudioStreamItem>
{
    public AudioStreamItem(int streamId, AudioAttributes audioAttributes)
    {
        this.AudioAttributes = audioAttributes;
        this.StreamId = streamId;
        this.KBitsPerSecond = (audioAttributes.CodingMode, audioAttributes.Channels) switch
        {
            (AudioCodingMode.AC3, 1) => 96,
            (AudioCodingMode.AC3, 2) => 192,
            (AudioCodingMode.AC3, 3 or 6) => 448,
            (AudioCodingMode.MPEG1 or AudioCodingMode.MPEG2, 1) => 96,
            (AudioCodingMode.MPEG1 or AudioCodingMode.MPEG2, 2) => 192,
            (AudioCodingMode.MPEG1 or AudioCodingMode.MPEG2, 6) => 576,
            (AudioCodingMode.DTS, 2) => 512,
            (AudioCodingMode.DTS, 6) => 1536,
            (AudioCodingMode.LPCM, _) => 48 * 8 * audioAttributes.Channels,
            _ => 0,
        };
    }

    public AudioAttributes AudioAttributes { get; private set; }
    public int StreamId { get; private set; }
    public int KBitsPerSecond { get; private set; }

    public int CompareTo(AudioStreamItem other)
    {
        return this.StreamId.CompareTo(other.StreamId);
    }

    public override string ToString()
    {
        return this.StreamId.ToString();
    }
}

