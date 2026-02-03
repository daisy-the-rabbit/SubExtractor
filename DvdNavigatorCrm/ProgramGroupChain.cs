/*
 * Copyright (C) 2007, 2008 Chris Meadowcroft <crmeadowcroft@gmail.com>
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
public class ProgramGroupChain
{
	bool isEntry;
	int title;
	int programCount;
	int[] programStartCell;
	int cellCount;
	List<CellInformation> cells = [];
	float playbackTime;
	float fps;
	SortedList<int, int> audioStreams = new SortedList<int, int>();
	SortedList<int, int[]> subpictureStreams = new SortedList<int, int[]>();
	int nextPGC;
	int prevPGC;
	int upPGC;
	int stillTime;
	int playbackMode;
	int[] palette = new int[16];
	UInt64[] preCommands;
	UInt64[] postCommands;
	UInt64[] cellCommands;

	public ProgramGroupChain(bool isEntry, int title)
	{
		this.isEntry = isEntry;
		this.title = title;
	}

	internal void Parse(IfoReader reader, int offset)
	{
		reader.SeekFromStart(offset + 2);
		this.programCount = reader.ReadByte();
		this.cellCount = reader.ReadByte();

		int hours, minutes, seconds, frames;
		reader.ReadTimingInfo(out hours, out minutes, out seconds, out frames, out this.fps);
		this.playbackTime = Convert.ToSingle(hours * 3600 + minutes * 60 + seconds) +
			Convert.ToSingle(frames) / this.fps;

		reader.SeekFromStart(offset + 0x0c);
		for(int audioIndex = 0; audioIndex < 8; audioIndex++)
		{
			int number = reader.ReadByte();
			if((number & 0x80) != 0)
			{
				this.audioStreams[audioIndex] = number & 0x07;
			}
			reader.SeekFromCurrent(1);
		}

		for(int subIndex = 0; subIndex < 32; subIndex++)
		{
			int number43 = reader.ReadByte();
			int numberWide = reader.ReadByte() & 0x1f;
			int numberLetterBox = reader.ReadByte() & 0x1f;
			int numberPanScan = reader.ReadByte() & 0x1f;
			if((number43 & 0x80) != 0)
			{
				this.subpictureStreams[subIndex] = new int[4] { number43 & 0x1f, 
                        numberWide, numberLetterBox, numberPanScan };
			}
		}

		this.nextPGC = reader.ReadUInt16();
		this.prevPGC = reader.ReadUInt16();
		this.upPGC = reader.ReadUInt16();
		this.playbackMode = reader.ReadByte();
		this.stillTime = reader.ReadByte();

		for(int paletteIndex = 0; paletteIndex < 16; paletteIndex++)
		{
			this.palette[paletteIndex] = (int)reader.ReadUInt32();
		}

		int commandsOffset = offset + reader.ReadUInt16();
		int programMapOffset = offset + reader.ReadUInt16();
		int cellPlaybackOffset = offset + reader.ReadUInt16();
		int cellPositionOffset = offset + reader.ReadUInt16();

		if(commandsOffset != offset)
		{
			reader.SeekFromStart(commandsOffset);
			this.preCommands = new ulong[reader.ReadUInt16()];
			this.postCommands = new ulong[reader.ReadUInt16()];
			this.cellCommands = new ulong[reader.ReadUInt16()];
			int endAddress = reader.ReadUInt16();
			//reader.SeekFromCurrent(2);
			for(int index = 0; index < this.preCommands.Length; index++)
			{
				this.preCommands[index] = ((ulong)reader.ReadUInt32() << 32) + reader.ReadUInt32();
			}
			for(int index = 0; index < this.postCommands.Length; index++)
			{
				this.postCommands[index] = ((ulong)reader.ReadUInt32() << 32) + reader.ReadUInt32();
			}
			for(int index = 0; index < this.cellCommands.Length; index++)
			{
				this.cellCommands[index] = ((ulong)reader.ReadUInt32() << 32) + reader.ReadUInt32();
			}
		}
		else
		{
			this.preCommands = new ulong[0];
			this.postCommands = new ulong[0];
			this.cellCommands = new ulong[0];
		}

		this.programStartCell = new int[this.programCount];
		if(programMapOffset != offset)
		{
			reader.SeekFromStart(programMapOffset);
			for(int startCellIndex = 0; startCellIndex < this.programCount; startCellIndex++)
			{
				this.programStartCell[startCellIndex] = reader.ReadByte();
			}
		}

		if(cellPlaybackOffset != offset)
		{
			reader.SeekFromStart(cellPlaybackOffset);
			for(int cellIndex = 0; cellIndex < this.cellCount; cellIndex++)
			{
				CellInformation cell = new CellInformation();
				cell.ParsePlayback(reader);
				this.cells.Add(cell);
			}
		}

		if(cellPositionOffset != offset)
		{
			reader.SeekFromStart(cellPositionOffset);
			for(int cellIndex = 0; cellIndex < this.cellCount; cellIndex++)
			{
				this.cells[cellIndex].ParsePosition(reader);
			}
		}
	}

	string CommandsAsString(ulong[] cmds)
	{
		StringBuilder sb = new StringBuilder("");
		int line = 1;
		foreach(ulong cmd in cmds)
		{
			int op0 = Convert.ToInt32((cmd >> (7 * 8)) & 0xff);
			int op1 = Convert.ToInt32((cmd >> (6 * 8)) & 0xff);
			sb.Append($"Opcode {(op0 >> 5) & 7} {(op0 >> 4) & 1} {op0 & 0xf} {(op1 >> 7) & 1} {(op1 >> 4) & 7} {op1 & 0xf}  ");
			for(int index = 7; index >= 0; index--)
			{
				sb.Append($" {(cmd >> (index * 8)) & 0xff:X2}");
			}
			sb.Append("\n");
			sb.Append(new OpCode(cmd, line).ToString());
			sb.Append("\n");
			line++;
		}
		return sb.ToString();
	}

	public override string ToString()
	{
		StringBuilder sb = new StringBuilder();

		sb.Append($"Entry {this.isEntry} Title {this.title} Programs {this.programCount} Cells {this.cellCount} Time {this.playbackTime:f2} FPS {this.fps}\n");

		foreach(KeyValuePair<int, int> audioItem in this.audioStreams)
		{
			sb.Append($"Audio Track {audioItem.Key + 1} ID {audioItem.Value}\n");
		}

		foreach(KeyValuePair<int, int[]> subpictureItem in this.subpictureStreams)
		{
			sb.Append($"Sub Track {subpictureItem.Key + 1} IDs: 4:3 {subpictureItem.Value[0]}, Wide {subpictureItem.Value[1]}, LetterBox {subpictureItem.Value[2]}, PanScan {subpictureItem.Value[3]}\n");
		}

		sb.Append($"PGC Next {this.nextPGC} Prev {this.prevPGC} Up {this.upPGC}\n");

		sb.Append($"StillTime {this.stillTime} Playback Mode {this.playbackMode:x}\n");

		sb.Append("Palette ");
		for(int paletteIndex = 0; paletteIndex < 16; paletteIndex++)
		{
			sb.Append($"{this.palette[paletteIndex]:X6} ");
		}
		sb.Append("\n");

		if(this.preCommands.Length != 0)
		{
			sb.Append("Pre Commands\n");
			sb.Append(CommandsAsString(this.preCommands));
			sb.Append("\n");
		}

		if(this.postCommands.Length != 0)
		{
			sb.Append("Post Commands\n");
			sb.Append(CommandsAsString(this.postCommands));
			sb.Append("\n");
		}

		if(this.cellCommands.Length != 0)
		{
			sb.Append("Cell Commands\n");
			sb.Append(CommandsAsString(this.cellCommands));
			sb.Append("\n");
		}

		sb.Append("Starting Cells ");
		for(int startCellIndex = 0; startCellIndex < this.programCount; startCellIndex++)
		{
			sb.Append($"{this.programStartCell[startCellIndex]} ");
		}
		sb.Append("\n\n");

		for(int cellIndex = 0; cellIndex < this.cellCount; cellIndex++)
		{
			CellInformation cell = this.cells[cellIndex];
			sb.Append($"Cell {cellIndex + 1}\n");
			sb.Append(cell.ToString());
			sb.Append("\n");
		}

		return sb.ToString();
	}

	public bool IsEntry { get { return this.isEntry; } }
	public int Title { get { return this.title; } }

	public int ProgramCount { get { return this.programCount; } }
	public int GetProgramStartCell(int program) { return this.programStartCell[program - 1]; }

	public int CellCount { get { return this.cellCount; } }
	public CellInformation GetCell(int cell) { return this.cells[cell - 1]; }

	public float PlaybackTime { get { return this.playbackTime; } }
	public float FPS { get { return this.fps; } }
	public SortedList<int, int> AudioStreams { get { return this.audioStreams; } }
	public SortedList<int, int[]> SubpictureStreams { get { return this.subpictureStreams; } }
	public int NextPGC { get { return this.nextPGC; } }
	public int PrevPGC { get { return this.prevPGC; } }
	public int UpPGC { get { return this.upPGC; } }
	public int GetPaletteColor(int index) { return this.palette[index]; }
	public IList<int> Palette { get { return new List<int>(this.palette); } }
	public UInt64[] PreCommands { get { return this.preCommands; } }
	public UInt64[] PostCommands { get { return this.postCommands; } }
	public UInt64[] CellCommands { get { return this.cellCommands; } }
}

