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

namespace DvdNavigatorCrm;
public enum CellType
{
	Normal = 0,
	FirstAngleBlock = 1,
	MiddleAngleBlock = 2,
	LastAngleBlock = 3,
}

public class CellInformation
{
	CellType cellType;
	bool isAngleBlock;
	bool isSeamless;
	bool isInterleaved;
	bool isStcDiscontinuity;
	bool isSeamlessAngleDsi;
	int cellStillTime;
	int cellCommandNumber;
	float playbackTime;
	float fps;
	int firstVobuStartSector;
	int firstIlvuEndSector;
	int lastVobuStartSector;
	int lastVobuEndSector;
	int vobId;
	int cellId;

	public CellInformation()
	{
	}

	public override string ToString()
	{
		StringBuilder sb = new StringBuilder();

		sb.Append($"Type {this.cellType} ");
		if(this.isAngleBlock)
		{
			sb.Append("AngleBlock ");
		}
		if(this.isSeamless)
		{
			sb.Append("Seamless ");
		}
		if(this.isInterleaved)
		{
			sb.Append("Interleaved ");
		}
		if(this.isStcDiscontinuity)
		{
			sb.Append("StcDiscontinuity ");
		}
		if(this.isSeamlessAngleDsi)
		{
			sb.Append("SeamlessAngleDsi ");
		}

		sb.Append($"\nStillTime {this.cellStillTime} Command # {this.cellCommandNumber}");

		sb.Append($"\nPlaybackTime {this.playbackTime:f2} FPS {this.fps}");

		sb.Append($"\n1stVob {this.firstVobuStartSector:X8} IlvuEnd {this.firstIlvuEndSector:X8} LastVobStart {this.lastVobuStartSector:X8} LastVobEnd {this.lastVobuEndSector:X8}");

		sb.Append($"\nVobId {this.vobId} CellId {this.cellId}\n");

		return sb.ToString();
	}

	internal void ParsePlayback(IfoReader reader)
	{
		int cellTypeByte = reader.ReadByte();
		this.cellType = (CellType)((cellTypeByte & 0xc0) >> 6);
		this.isAngleBlock = (cellTypeByte & 0x30) == 0x10;
		this.isSeamless = (cellTypeByte & 0x08) == 0x08;
		this.isInterleaved = (cellTypeByte & 0x04) == 0x04;
		this.isStcDiscontinuity = (cellTypeByte & 0x02) == 0x02;
		this.isSeamlessAngleDsi = (cellTypeByte & 0x01) == 0x01;
		reader.SeekFromCurrent(1);

		this.cellStillTime = reader.ReadByte();
		this.cellCommandNumber = reader.ReadByte();

		int cellHours, cellMinutes, cellSeconds, cellFrames;
		reader.ReadTimingInfo(out cellHours, out cellMinutes, out cellSeconds, out cellFrames, out this.fps);
		this.playbackTime = Convert.ToSingle(cellHours * 3600 + cellMinutes * 60 + cellSeconds) +
			Convert.ToSingle(cellFrames) / this.fps;

		this.firstVobuStartSector = Convert.ToInt32(reader.ReadUInt32());
		this.firstIlvuEndSector = Convert.ToInt32(reader.ReadUInt32());
		this.lastVobuStartSector = Convert.ToInt32(reader.ReadUInt32());
		this.lastVobuEndSector = Convert.ToInt32(reader.ReadUInt32());
	}

	internal void ParsePosition(IfoReader reader)
	{
		this.vobId = reader.ReadUInt16();
		reader.SeekFromCurrent(1);
		this.cellId = reader.ReadByte();
	}

	public CellType CellType { get { return this.cellType; } }
	public bool IsAngleBlock { get { return this.isAngleBlock; } }
	public bool IsSeamless { get { return this.IsSeamless; } }
	public bool IsInterleaved { get { return this.isInterleaved; } }
	public bool IsStcDiscontinuity { get { return this.isStcDiscontinuity; } }
	public bool IsSeamlessAngleDsi { get { return this.isSeamlessAngleDsi; } }
	public int CellStillTime { get { return this.cellStillTime;}}
	public int CellCommandNumber { get { return this.cellCommandNumber; } }
	public float PlaybackTime { get { return this.playbackTime; } }
	public float FPS { get { return this.fps; } }
	public int FirstVobuStartSector { get { return this.firstVobuStartSector; } }
	public int FirstIlvuEndSector { get { return this.firstIlvuEndSector; } }
	public int LastVobuStartSector { get { return this.lastVobuStartSector; } }
	public int LastVobuEndSector { get { return this.lastVobuEndSector; } }
	public int VobId { get { return this.vobId; } }
	public int CellId { get { return this.cellId; } }
}

