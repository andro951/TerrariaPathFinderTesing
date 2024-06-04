using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static TerrariaPathFinderTesing.TileData.TA_TileData.BinaryHelper;

namespace TerrariaPathFinderTesing.TileData.TA_TileData {
	public static class BinaryHelper {
		public static byte BitsNeeded(this byte b) => b == 0 ? (byte)1 : (byte)(byte.Log2(b) + 1);
		public static uint BitsNeeded(this uint i) => i == 0 ? 1 : (uint.Log2(i) + 1);
		internal const int byteNum = 8;
		internal const int ushortNum = 16;
		internal const int uintNum = 32;
		private const int ulongNum = 64;
		internal static void WriteNumber(this TestWriter writer, uint num, int bits) {
			if (bits == 0)
				return;

			if (bits > uintNum)
				bits = uintNum;

			uint c = 0b1;
			for (int i = 0; i < bits; i++) {
				writer.WriteBool((num & c) != 0);
				c <<= 1;
			}
		}
		internal static uint ReadNumber(this TestReader reader, int bits) {
			if (bits == 0)
				return 0;

			if (bits > uintNum)
				bits = uintNum;

			uint result = 0;
			uint c = 0b1;
			for (int i = 0; i < bits; i++) {
				if (reader.ReadBool())
					result |= (c << i);
			}

			return result;
		}
		internal static string ToBinaryString(this uint i) => Convert.ToString(i, 2).PadLeft(uintNum, '0');
		internal static string ToBinaryString(this byte b) => Convert.ToString(b, 2).PadLeft(byteNum, '0');
	}

	internal class TestWriter {
		private uint value = 0;
		private uint bit = 1;
		public uint Value => value;
		internal void WriteBool(bool v) {
			if (v)
				value |= bit;

			bit <<= 1;
		}
		public override string ToString() {
			return $"value: ({value},  {value.ToBinaryString()}), bit: ({bit}, {bit.ToBinaryString()})";
		}
	}
	internal class TestReader {
		private uint value = 0;
		private uint bit = 0b1;
		public uint Value => value;
		internal TestReader(uint value) {
			this.value = value;
		}
		internal bool ReadBool() {
			bool result = (value & bit) != 0;
			bit <<= 1;
			return result;
		}
		public override string ToString() {
			return $"value: ({value},  {value.ToBinaryString()}), bit: ({bit}, {bit.ToBinaryString()})";
		}
	}
	public static class BinaryWriterHelper {
		private static byte value;
		private static byte bit;
		public static uint Value => value;
		static BinaryWriterHelper() {
			Reset();
		}
		public static void Finish(this BinaryWriter writer) {
			if (bit == 1)
				return;

			writer.Write(value);
			Reset();
		}
		private static void Reset() {
			value = 0;
			bit = 1;
		}
		public static void WriteBool(this BinaryWriter writer, bool v) {
			if (v)
				value |= bit;

			bit <<= 1;
			if (bit == 0) {
				writer.Write(value);
				Reset();
			}
		}
		public static void WriteNumber(this BinaryWriter writer, uint num, uint bits) {
			uint c = 0b1;
			for (uint i = 0; i < bits; i++) {
				writer.WriteBool((num & c) != 0);
				c <<= 1;
			}
		}
		public static void WriteNumber(this BinaryWriter writer, uint num, int bits) {
			uint c = 0b1;
			for (int i = 0; i < bits; i++) {
				writer.WriteBool((num & c) != 0);
				c <<= 1;
			}
		}
		public static void WriteNumber(this BinaryWriter writer, uint_b b) => writer.WriteNumber(b.value, b.Bits);
		public static string S(this BinaryWriter _) {
			return $"value: ({value},  {value.ToBinaryString()}), bit: ({bit}, {bit.ToBinaryString()})";
		}
	}
	public static class BinaryReaderHelper {
		private static byte value;
		private static byte bit;
		public static byte Value => value;
		static BinaryReaderHelper() {
			Reset();
		}
		public static void Finish(this BinaryReader _) {
			Reset();
		}
		private static void Reset() {
			value = 0;
			bit = 0;
		}
		public static bool ReadBool(this BinaryReader reader) {
			if (bit == 0) {
				value = reader.ReadByte();
				bit = 1;
			}

			bool result = (value & bit) != 0;
			bit <<= 1;

			return result;
		}
		public static uint ReadNumber(this BinaryReader reader, int bits) {
			uint result = 0;
			uint c = 0b1;
			for (int i = 0; i < bits; i++) {
				if (reader.ReadBool())
					result |= (c << i);
			}

			return result;
		}
		public static string S(this BinaryReader _) {
			return $"value: ({value},  {value.ToBinaryString()}), bit: ({bit}, {bit.ToBinaryString()})";
		}
	}
	public struct uint_b {
		public const uint MaxValue = 134217728;//2^27
		private const int significandBits = 27;
		private const uint significandMask = 0b00000111111111111111111111111111;
		private const uint bitsMask = 0b11111000000000000000000000000000;
		public uint value;
		public uint Bits => (value & bitsMask) >> significandBits;
		public uint Significand => value & significandMask;
		public uint_b(uint value, uint bits) {
			this.value = (value & significandMask) | (bits << significandBits);
		}

		public override string ToString() {
			return $"value: {value}, Significand: {Significand}, Bits: {Bits}";
		}
	}
}
