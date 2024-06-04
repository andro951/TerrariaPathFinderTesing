using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TerrariaPathFinderTesing.TileData {
	public static class TileDataPacking {
		public static int Unpack(int bits, int offset, int width)
			=> bits >> offset & ((1 << width) - 1);

		// we also & the incoming value with the bit mask as a high performance safeguard against invalid values spilling over into adjacent bits
		public static int Pack(int value, int bits, int offset, int width)
			=> bits & ~(((1 << width) - 1) << offset) | (value & ((1 << width) - 1)) << offset;

		public static bool GetBit(int bits, int offset)
			=> (bits & 1 << offset) != 0;

		public static int SetBit(bool value, int bits, int offset)
			=> value ? bits | 1 << offset : bits & ~(1 << offset);
	}
}
