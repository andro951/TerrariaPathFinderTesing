using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace TerrariaPathFinderTesing.TileData {
	public readonly struct Tile {
		private readonly uint TileId;
		[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
#if TILE_X_Y
	internal Tile(ushort x, ushort y, uint tileId) {
		X = x;
		Y = y;
		TileId = tileId;
	}
#else
		internal Tile(uint tileId) {
			TileId = tileId;
		}
#endif

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe ref T Get<T>() where T : unmanaged, ITileData
			=> ref TileData<T>.ptr[TileId];
	}
}
