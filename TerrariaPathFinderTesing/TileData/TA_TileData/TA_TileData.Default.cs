using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using static TerrariaPathFinderTesing.TileData.TileDataPacking;

namespace TerrariaPathFinderTesing.TileData.TA_TileData {
	public struct TilePipeData : ITileData {
		public const byte Empty = 0;
		private byte pipeData;
		private static int hasPipeBit = 0;
		private static int pipeTypeOffset = 1;
		private static int pipeTypeLength = 7;

		private static int realLength = 8;
		public bool HasPipe { get => GetBit(pipeData, hasPipeBit); set => pipeData = (byte)SetBit(value, pipeData, hasPipeBit); }
		public byte PipeType {
			get => (byte)Unpack(pipeData, pipeTypeOffset, pipeTypeLength);
			set {
				pipeData = (byte)Pack(value, pipeData, pipeTypeOffset, pipeTypeLength);
			}
		}
		public byte PipeData { get => pipeData; set => pipeData = value; }
	}
	public static class ES_TileDataStaticMethods {
		public static bool HasPipe(this Tile tile) => tile.Get<TilePipeData>().HasPipe;
		public static void HasPipe(this Tile tile, bool value) => tile.Get<TilePipeData>().HasPipe = value;
		public static byte PipeType(this Tile tile) => tile.Get<TilePipeData>().PipeType;
		public static void PipeType(this Tile tile, byte pipeType) => tile.Get<TilePipeData>().PipeType = pipeType;
		public static byte PipeData(this Tile tile) => tile.Get<TilePipeData>().PipeData;
		public static void PipeData(this Tile tile, byte pipeData) => tile.Get<TilePipeData>().PipeData = pipeData;
		public static Tile GetTile(this uint tileId) {
			Tile tile = new();
			unsafe {
				*(uint*)&tile = tileId;
			}

			return tile;
		}
	}
}
