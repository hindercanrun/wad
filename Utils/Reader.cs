﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using static Utils.Structs;
using static Utils.EndiannessReader;

namespace Utils
{
	internal class EndiannessReader : BinaryReader
	{
		internal enum Endianness
		{
			Little,
			Big,
		}

		private Endianness _Endianness = Endianness.Big;

		internal EndiannessReader(
			Stream Input)
			: base(Input)
		{
		}

		internal EndiannessReader(
			Stream Input, Encoding Encoding)
			: base(Input, Encoding)
		{
		}

		internal EndiannessReader(
			Stream Input, Encoding Encoding,
			bool LeaveOpen)
			: base(Input, Encoding, LeaveOpen)
		{
		}

		internal EndiannessReader(
			Stream Input,
			Endianness Endianness)
			: base(Input)
		{
			_Endianness = Endianness;
		}

		internal EndiannessReader(
			Stream Input, Encoding Encoding,
			Endianness Endianness)
			: base(Input, Encoding)
		{
			_Endianness = Endianness;
		}

		internal EndiannessReader(
			Stream Input, Encoding Encoding, bool LeaveOpen,
			Endianness Endianness)
			: base(Input, Encoding, LeaveOpen)
		{
			_Endianness = Endianness;
		}

		internal void SetEndianness(Endianness Endianness)
		{
			_Endianness = Endianness;
		}

		public override short ReadInt16()
			=> ReadInt16(_Endianness);

		public override int ReadInt32()
			=> ReadInt32(_Endianness);

		public override long ReadInt64()
			=> ReadInt64(_Endianness);

		public override ushort ReadUInt16()
			=> ReadUInt16(_Endianness);

		public override uint ReadUInt32()
			=> ReadUInt32(_Endianness);

		public override ulong ReadUInt64()
			=> ReadUInt64(_Endianness);

		public ushort ReadUShort()
			=> ReadUShort(_Endianness);

		public ulong ReadULong()
			=> ReadULong(_Endianness);

		public override bool ReadBoolean()
			=> ReadBoolean(_Endianness);

		public override byte ReadByte()
			=> base.ReadByte();

		public override string ReadString()
			=> ReadString(0, _Endianness);//uhh idk

		public override char ReadChar()
			=> ReadChar(_Endianness);

		public float ReadFloat()
			=> ReadFloat(_Endianness);

		public override double ReadDouble()
			=> ReadDouble(_Endianness);

		public short ReadShort()
			=> ReadShort(_Endianness);

		public long ReadLong()
			=> ReadLong(_Endianness);

		public short ReadInt16(Endianness Endianness)
			=> BitConverter.ToInt16(
				ReadForEndianness(sizeof(short),
				Endianness), 0);

		public int ReadInt32(Endianness Endianness)
			=> BitConverter.ToInt32(
				ReadForEndianness(sizeof(int),
				Endianness), 0);

		public long ReadInt64(Endianness Endianness)
			=> BitConverter.ToInt64(
				ReadForEndianness(sizeof(long),
				Endianness), 0);

		public ushort ReadUInt16(Endianness Endianness)
			=> BitConverter.ToUInt16(
				ReadForEndianness(sizeof(ushort),
				Endianness), 0);

		public uint ReadUInt32(Endianness Endianness)
			=> BitConverter.ToUInt32(
				ReadForEndianness(sizeof(uint),
				Endianness), 0);

		public ulong ReadUInt64(Endianness Endianness)
			=> BitConverter.ToUInt64(
				ReadForEndianness(sizeof(ulong),
				Endianness), 0);

		public ushort ReadUShort(Endianness Endianness)
			=> BitConverter.ToUInt16(
				ReadForEndianness(sizeof(ushort),
				Endianness), 0);

		public ulong ReadULong(Endianness Endianness)
			=> BitConverter.ToUInt64(
				ReadForEndianness(sizeof(ulong),
				Endianness), 0);

		public bool ReadBoolean(Endianness Endianness)
			=> BitConverter.ToBoolean(
				ReadForEndianness(sizeof(bool),
				Endianness), 0);

		public string ReadString(int Length, Endianness Endianness)
			=> BitConverter.ToString(
				ReadForEndianness(Length,
				Endianness));

		public char ReadChar(Endianness Endianness)
			=> BitConverter.ToChar(
				ReadForEndianness(sizeof(char),
				Endianness), 0);

		public float ReadFloat(Endianness Endianness)
			=> BitConverter.ToSingle(
				ReadForEndianness(sizeof(float),
				Endianness), 0);

		public double ReadDouble(Endianness Endianness)
			=> BitConverter.ToDouble(
				ReadForEndianness(sizeof(double),
				Endianness), 0);

		public short ReadShort(Endianness Endianness)
			=> BitConverter.ToInt16(
				ReadForEndianness(sizeof(short),
				Endianness), 0);

		public long ReadLong(Endianness Endianness)
			=> BitConverter.ToInt64(
				ReadForEndianness(sizeof(long),
				Endianness), 0);

		private byte[] ReadForEndianness(int BytesToRead, Endianness Endianness)
		{
			var BytesRead = ReadBytes(BytesToRead);
			if ((Endianness == Endianness.Little && !BitConverter.IsLittleEndian)
				|| (Endianness == Endianness.Big && BitConverter.IsLittleEndian))
			{
				Array.Reverse(BytesRead);
			}

			return BytesRead;
		}

		public static UInt32 ReverseEndianUInt32(UInt32 Value)
		{
			return ((Value & 0x000000FF) << 24) |
				((Value & 0x0000FF00) << 8) |
				((Value & 0x00FF0000) >> 8) |
				((Value & 0xFF000000) >> 24);
		}
	}

	internal class Reader
	{
		//
		// processes the inputted .wad file
		//
		internal static List<WADEntry> ProcessOnlineWAD(byte[] Bytes)
		{
			WADHeader Header = ReadWADHeader(Bytes);
			// check the wad magic
			if (Header.magic != 0x543377AB) // T3w«
			{
				Print.WriteError(
					$"WAD has incorrect magic!");
				Print.WriteError(
					$"Expecting: 0x543377AB, got: 0x{Header.magic:X8}");
				return null;
			}

			//convert the timestamp to a readable format
			DateTimeOffset TimeOffset = DateTimeOffset.FromUnixTimeSeconds(Header.timestamp);
			DateTime Time = TimeOffset.UtcDateTime;

			//some misc .wad information
			Print.WriteMiscMessage(
				$"WAD Information:");
			Print.WriteMiscMessage(
				$"Magic: 0x{Header.magic:X8}");
			Print.WriteMiscMessage(
				$"Timestamp: {Time:HH:mm:ss, dd/MM/yyyy} ({Header.timestamp:X8})");
			Print.WriteMiscMessage(
				$"Entries: {Header.numEntries}");
			Print.WriteMiscMessage(
				$"FFOTD: {Header.ffotdVersion}");

			Print.WriteMessage($"\nExtracting files..\n");

			//time to read the entries
			List<WADEntry> Entries = new List<WADEntry>();
			for (int Index = 0; Index < Header.numEntries; Index++)
			{
				Entries.Add(ReadWADEntry(Bytes, Index));
			}

			return Entries;
		}

		//
		// reads the header
		//
		internal static WADHeader ReadWADHeader(byte[] Bytes)
		{
			using (var Stream = new MemoryStream(Bytes))
			using (var Reader = new EndiannessReader(Stream))
			{
				WADHeader Header = new WADHeader
				{
					magic = Reader.ReadUInt32(),
					timestamp = Reader.ReadUInt32(),
					numEntries = Reader.ReadUInt32(),
					ffotdVersion = Reader.ReadUInt32()
				};

				return Header;
			}
		}

		//
		// reads the .wad entries
		//
		internal static WADEntry ReadWADEntry(byte[] Bytes, int Index)
		{
			const int EntryDataSize = 44;// WADEntry struct size

			byte[] Data = new byte[EntryDataSize];
			Array.Copy(Bytes, 16 + (EntryDataSize * Index), Data, 0, Data.Length);

			using (var Stream = new MemoryStream(Data))
			using (var Reader = new BinaryReader(Stream))
			{
				WADEntry Entry = new WADEntry
				{
					name = ReadEntryName(Reader),
					compressedSize = ReverseEndianUInt32(Reader.ReadUInt32()),
					size = ReverseEndianUInt32(Reader.ReadUInt32()),
					offset = ReverseEndianUInt32(Reader.ReadUInt32())
				};

				Entry.compressedBuf = new byte[Entry.compressedSize];
				return Entry;
			}
		}

		//
		// reads a 32-bit integer from the reader
		//
		internal static string ReadEntryName(BinaryReader Reader)
		{
			return Encoding.ASCII.GetString(Reader.ReadBytes(32)).Trim('\0');
		}

		//
		// grabs the specified file name
		//
		internal static string GetFilename(string FileName)
		{
			return Path.GetFileName(FileName);
		}
	}
}