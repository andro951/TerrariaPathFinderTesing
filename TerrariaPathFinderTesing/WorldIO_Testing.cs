using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TerrariaPathFinderTesing.TileData;
using TerrariaPathFinderTesing.TileData.TA_TileData;
using TerrariaPathFinderTesting;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TerrariaPathFinderTesing {
	internal static class WorldIO_Testing {
		public static void TileDataAccessTesting() {
			SaveWorld();
			LoadWorld(false);
		}
		private static string taWorldPathName => Main.worldPathName?.Replace(".wld", ".tawld");
		public static void SaveWorld() {
			try {
				SaveWorld(false);
			}
			catch (Exception exception) {
				$"Failed to save world; exception: {exception}".LogSimple();
				//FancyErrorPrinter.ShowFileSavingFailError(exception, Main.WorldPath);
				throw;
			}
		}
		public static void SaveWorld(bool useCloudSaving, bool resetTime = false) {
			//if (useCloudSaving && SocialAPI.Cloud == null)
			//	return;

			if (Main.worldName == "")
				Main.worldName = "World";

			//while (WorldGen.IsGeneratingHardMode) {
			//	Main.statusText = Lang.gen[48].Value;
			//}

			//if (!Monitor.TryEnter(IOLock))
			//	return;

			try {
				FileUtilities.ProtectedInvoke(delegate {
					InternalSaveWorld(useCloudSaving, resetTime);
				});
			}
			finally {
				//Monitor.Exit(IOLock);
			}
		}
		private static void InternalSaveWorld(bool useCloudSaving, bool resetTime) {
			Utils.TryCreatingDirectory(Main.WorldPath);

			string pathName = taWorldPathName;
			if (pathName == null)
				return;

			Stopwatch stopwatch = null;
			if (Debugger.IsAttached)
				stopwatch = Stopwatch.StartNew();

			int num;
			byte[] array;
			using (MemoryStream memoryStream = new MemoryStream(7000000)) {
				using (BinaryWriter writer = new BinaryWriter(memoryStream)) {
					SaveWorld_Version2(writer);
				}

				array = memoryStream.ToArray();
				num = array.Length;
			}

			string s = "";
			if (Debugger.IsAttached) {
				stopwatch?.Stop();
				s = $" ticks: {stopwatch?.ElapsedTicks}, ms: {stopwatch?.ElapsedMilliseconds}";
			}

			$"Completed Saving Terraria Automations World Data.{s}".LogSimple();

			byte[] array2 = null;
			if (FileUtilities.Exists(pathName, useCloudSaving))
				array2 = FileUtilities.ReadAllBytes(pathName, useCloudSaving);

			FileUtilities.Write(pathName, array, num, useCloudSaving);
			array = FileUtilities.ReadAllBytes(pathName, useCloudSaving);


			for (int i = 0; i < array.Length; i++) {
				byte b = array[i];
				$"b{i}: {b.ToBinaryString()}".LogSimple();
			}
		}
		private static byte[] _tile = new byte[Main.maxTilesX * Main.maxTilesY];
		private static void SaveWorld_Version2(BinaryWriter writer) {
			for (uint z = 0; z < (uint)Main.maxTilesX * (uint)Main.maxTilesY; z++) {
				z.GetTile().HasPipe(true);
			}

			Main.tile[10, 5].PipeType(13);

			//SaveWorldTesting(writer);
			//return;
			writer.Write((ushort)Main.maxTilesX);
			writer.Write((ushort)Main.maxTilesY);

			//determine pipeTypes byte(bits needed to represent largest number)
			//write Main.maxTilesX
			//write Main.maxTilesY
			SortedSet<byte> pipeTypesSet = new();
			uint count = (uint)Main.maxTilesX * (uint)Main.maxTilesY;
			for (uint z = 0; z < count; z++) {
				Tile tile = z.GetTile();
				pipeTypesSet.Add(tile.PipeData());
			}

			byte pipeTypes = (byte)pipeTypesSet.Count;
			writer.Write(pipeTypes);
			uint numBitsNeededForPipeId = pipeTypes.BitsNeeded();//TODO:
			uint maxBitsInTileCountRepresentation = count.BitsNeeded();//TODO:
																		//writer.Write((byte)maxBitsInTileCountRepresentation);//Can be calculated since saving Main.maxTiles

			byte[] pipeTypesArr = pipeTypesSet.ToArray();
			Dictionary<byte, byte> pipeIds = new();//Try to store i as the bits value instead of having to convert it every time.//TODO:
			for (byte i = 0; i < pipeTypes; i++) {
				byte pypeType = pipeTypesArr[i];
				pipeIds.Add(pypeType, i);
				writer.Write(pypeType);// write pipeTypesArr[i] as a byte!!!
			}

			for (uint z = 0; z < count;) {
				Tile tile = z.GetTile();
				byte pipeData = tile.PipeData();
				writer.Write(pipeIds[pipeData]);

				//write a maxBitsInTileCountRepresentation to tell how many bits should be read for the number of tiles
				uint startZ = z;
				while (++z < count) {
					byte otherData = z.GetTile().PipeData();
					if (otherData != pipeData)
						break;
				}

				uint tileCount = z - startZ;//Make sure z is decremented after the -
				writer.Write(tileCount);
				//write tileCount as a maxBitsInTileCountRepresentation.//TODO:
			}
		}
		public static void LoadWorld(bool loadFromCloud) {
			bool flag = loadFromCloud /*&& SocialAPI.Cloud != null*/;
			string pathName = taWorldPathName;
			if (!FileUtilities.Exists(pathName, flag) && Main.autoGen) {
				if (!flag) {
					for (int num = pathName.Length - 1; num >= 0; num--) {
						if (pathName.Substring(num, 1) == (Path.DirectorySeparatorChar.ToString() ?? "")) {
							string temp = pathName.Substring(0, num);//TODO: delete
							Utils.TryCreatingDirectory(pathName.Substring(0, num));
							break;
						}
					}
				}

				Main.tile.ClearEverything();//Don't include this.  Just here for testing.
				//WorldGen.clearWorld();
				//Main.ActiveWorldFileData = CreateMetadata((Main.worldName == "") ? "World" : Main.worldName, flag, Main.GameMode);
				//string text = (Main.AutogenSeedName ?? "").Trim();
				//if (text.Length == 0)
				//	Main.ActiveWorldFileData.SetSeedToRandom();
				//else
				//	Main.ActiveWorldFileData.SetSeed(text);

				//UIWorldCreation.ProcessSpecialWorldSeeds(text);
				//WorldGen.GenerateWorld(Main.ActiveWorldFileData.Seed, Main.AutogenProgress);
				SaveWorld();
			}

			try {
				using MemoryStream memoryStream = new MemoryStream(FileUtilities.ReadAllBytes(pathName, flag));
				using BinaryReader binaryReader = new BinaryReader(memoryStream);
				try {
					LoadWorld_Version2(binaryReader);
					binaryReader.Close();
					memoryStream.Close();

				}
				catch (Exception lastThrownLoadException) {
					$"failed to load world. {lastThrownLoadException}".LogSimple();
					try {
						binaryReader.Close();
						memoryStream.Close();
						return;
					}
					catch {
						return;
					}
				}
			}
			catch (Exception lastThrownLoadException2) {
				$"failed to load world. {lastThrownLoadException2}".LogSimple();
				return;
			}
		}
		private static void LoadWorld_Version2(BinaryReader reader) {
			//LoadWorldTiles(reader);
			//return;
			uint maxTilesX = reader.ReadUInt16();
			uint maxTilesY = reader.ReadUInt16();

			byte pipeTypes = reader.ReadByte();
			uint count = maxTilesX * maxTilesY;
			uint numBitsNeededForPipeId = pipeTypes.BitsNeeded();//TODO:
			uint maxBitsInTileCountRepresentation = count.BitsNeeded();//TODO:

			Dictionary<byte, byte> pipeIds = new();//reverse of reader
			for (byte i = 0; i < pipeTypes; i++) {
				pipeIds.Add(i, reader.ReadByte());
			}

			for (uint z = 0; z < count;) {
				byte pipeData = pipeIds[reader.ReadByte()];
				uint tileCount = reader.ReadUInt32();
				uint zEnd = z + tileCount;
				if (pipeData == TilePipeData.Empty) {
					z = zEnd;
				}
				else {
					while (z < zEnd) {
						z.GetTile().PipeData(pipeData);
						z++;
					}
				}
			}
		}
		private static void LoadWorldTiles(BinaryReader reader) {
			LoadWorldTesting(reader);
		}
		private static (uint num, int bits)[] nums = [
			(5, 7),
			(1, 1),
			(1, 1),
			(1, 2),
			(3, 8),
			(10, 4),
			(13, 8),
			(1, 1),
		];
		private static TestWriter testWriter = new();
		private static void SaveWorldTesting(BinaryWriter writer) {
			testWriter = new();
			foreach ((uint num, int bits) n in nums) {
				testWriter.WriteNumber(n.num, n.bits);
			}

			$"writer: {testWriter}".LogSimple();

			foreach ((uint num, int bits) n in nums) {
				writer.WriteNumber(n.num, n.bits);
			}

			writer.Finish();
		}
		private static void LoadWorldTesting(BinaryReader reader) {
			TestReader testReader = new(testWriter.Value);
			foreach ((uint num, int bits) n in nums) {
				uint result = testReader.ReadNumber(n.bits);
				$"({result}, {n.bits}),".LogSimple();
			}

			$"reader: {testReader}".LogSimple();

			foreach ((uint num, int bits) n in nums) {
				uint result = reader.ReadNumber(n.bits);
				$"({result}, {n.bits}),".LogSimple();
			}

			reader.Finish();
		}
	}
	public static class Utils {
		public static bool TryCreatingDirectory(string folderPath) {
			if (Directory.Exists(folderPath))
				return true;

			try {
				Directory.CreateDirectory(folderPath);
				return true;
			}
			catch (Exception exception) {
				$"Failed to create directory; exception: {exception}, folderPath: {folderPath}".LogSimple();
				//FancyErrorPrinter.ShowDirectoryCreationFailError(exception, folderPath);
				return false;
			}
		}
	}
	public static class FileUtilities {
		private static Regex FileNameRegex = new Regex("^(?<path>.*[\\\\\\/])?(?:$|(?<fileName>.+?)(?:(?<extension>\\.[^.]*$)|$))", RegexOptions.IgnoreCase | RegexOptions.Compiled);
		public static bool Exists(string path, bool cloud) {
			//if (cloud && SocialAPI.Cloud != null)
			//	return SocialAPI.Cloud.HasFile(path);

			return File.Exists(path);
		}
		public static byte[] ReadAllBytes(string path, bool cloud) {
			//if (cloud && SocialAPI.Cloud != null)
			//	return SocialAPI.Cloud.Read(path);

			return File.ReadAllBytes(path);
		}
		public static void Write(string path, byte[] data, int length, bool cloud) {
			//if (cloud && SocialAPI.Cloud != null) {
			//	SocialAPI.Cloud.Write(path, data, length);
			//	return;
			//}

			string parentFolderPath = GetParentFolderPath(path);
			if (parentFolderPath != "")
				Utils.TryCreatingDirectory(parentFolderPath);

			RemoveReadOnlyAttribute(path);
			using FileStream fileStream = File.Open(path, FileMode.Create);
			while (fileStream.Position < length) {
				fileStream.Write(data, (int)fileStream.Position, Math.Min(length - (int)fileStream.Position, 2048));
			}
		}
		public static string GetParentFolderPath(string path, bool includeExtension = true) {
			Match match = FileNameRegex.Match(path);
			if (match == null || match.Groups["path"] == null)
				return "";

			return match.Groups["path"].Value;
		}
		public static void RemoveReadOnlyAttribute(string path) {
			if (!File.Exists(path))
				return;

			try {
				FileAttributes attributes = File.GetAttributes(path);
				if ((attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly) {
					attributes &= ~FileAttributes.ReadOnly;
					File.SetAttributes(path, attributes);
				}
			}
			catch (Exception) {
			}
		}
		public static void ProtectedInvoke(Action action) {
			bool isBackground = Thread.CurrentThread.IsBackground;
			try {
				Thread.CurrentThread.IsBackground = false;
				action();
			}
			finally {
				Thread.CurrentThread.IsBackground = isBackground;
			}
		}
	}
	internal static class Main {
		//internal static void Load() {
		//	tile = new((ushort)maxTilesX, (ushort)maxTilesY);
		//	byte data = tile[0, 0].GetPipeData();
		//	TilePipeData[] tilePipeDatas = tile.GetData<TilePipeData>();
		//}

		internal static string worldName = "TA_Dev";
		internal static string worldPathName = @"E:\Source\Repos\TerrariaPathFinderTesing\WorldIO_Testing\TA_Dev.wld";
		internal static string WorldPath = @"E:\Source\Repos\TerrariaPathFinderTesing\WorldIO_Testing";
		public static float rightWorld = 134400f;
		public static float bottomWorld = 38400f;
		//public static int maxTilesX = (int)rightWorld / 16 + 1;
		//public static int maxTilesY = (int)bottomWorld / 16 + 1;
		public static int maxTilesX = 30;
		public static int maxTilesY = 20;
		public static Tilemap tile = new((ushort)maxTilesX, (ushort)maxTilesY);
		public static bool autoGen = true;
	}
}
