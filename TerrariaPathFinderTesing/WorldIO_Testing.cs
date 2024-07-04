using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
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
			//SaveWorld();
			//LoadWorld(false);
			SaveLoadWorldTesting();
		}
		private static string taWorldPathName => Main.worldPathName?.Replace(".wld", ".tawld");
		public static void SaveWorld() {
			try {
				SaveWorld(false);
			}
			catch (Exception exception) {
				$"Failed to save world; exception: {exception}".Log();
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
				s = $" ticks: {stopwatch?.ElapsedTicks}, ms: {stopwatch?.ElapsedMilliseconds}, bytes: {new FileInfo(pathName).Length}";
			}

			$"Completed Saving Andro Mod World Data.{s}".Log();

			byte[] array2 = null;
			if (FileUtilities.Exists(pathName, useCloudSaving))
				array2 = FileUtilities.ReadAllBytes(pathName, useCloudSaving);

			FileUtilities.Write(pathName, array, num, useCloudSaving);
			array = FileUtilities.ReadAllBytes(pathName, useCloudSaving);


			//for (int i = 0; i < array.Length; i++) {
			//	byte b = array[i];
			//	$"b{i}: {b.ToBinaryString()}".LogSimple();
			//}
		}
		private static byte[] _tile = new byte[Main.maxTilesX * Main.maxTilesY];
		private static void SaveWorld_Version2(BinaryWriter writer) {
			
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
					$"failed to load world. {lastThrownLoadException}".Log();
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
				$"failed to load world. {lastThrownLoadException2}".Log();
				return;
			}
		}
		private static void LoadWorld_Version2(BinaryReader reader) {
			
		}
		private static void LoadWorldTiles(BinaryReader reader) {
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

			TilePipeData[] tilePipeData = Main.tile.GetData<TilePipeData>();
			for (uint z = 0; z < count;) {
				byte pipeData = pipeIds[reader.ReadByte()];
				uint tileCount = reader.ReadUInt32();
				uint zEnd = z + tileCount;
				if (pipeData == TilePipeData.Empty) {
					z = zEnd;
				}
				else {
					while (z < zEnd) {
						tilePipeData[z++].PipeData = pipeData;
					}
				}
			}
		}
		private static void SaveLoadWorldTesting() {
			Action<BinaryWriter>[] saveWorldFunctions = [/*SaveWorld0, SaveWorld1, SaveWorld2, */Save];
			Action<BinaryReader>[] loadWorldFunctions = [/*LoadWorld0, LoadWorld1, LoadWorld2, */Load];
			Action[] tileConfigurations = [SetTiles1, SetTiles2, SetTiles3];
			for (int j = 0; j < tileConfigurations.Length; j++) {
				for (int i = 0; i < saveWorldFunctions.Length; i++) {
					//Save
					try {
						if (Main.worldName == "")
							Main.worldName = "World";

						try {
							FileUtilities.ProtectedInvoke(delegate {
								Utils.TryCreatingDirectory(Main.WorldPath);

								string pathName = taWorldPathName;
								if (pathName == null)
									return;

								//Stopwatch stopwatch = null;
								//if (Debugger.IsAttached)
								//	stopwatch = Stopwatch.StartNew();

								tileConfigurations[j]();
								//UpdateSaveString();

								//int num;
								//byte[] array;
								//using (MemoryStream memoryStream = new MemoryStream(7000000)) {
								//	using (DeflateStream deflateStream = new DeflateStream(memoryStream, CompressionMode.Compress, true)) {
								//		using (BinaryWriter writer = new BinaryWriter(deflateStream)) {
								//			TestSaveWorld(saveWorldFunctions[i], writer);
								//		}
								//	}

								//	array = memoryStream.ToArray();
								//	num = array.Length;
								//}

								int num;
								byte[] array;
								using (MemoryStream memoryStream = new MemoryStream(30000000)) {
									using (BinaryWriter writer = new BinaryWriter(memoryStream)) {
										TestSaveWorld(saveWorldFunctions[i], writer);
									}


									array = memoryStream.ToArray();
									num = array.Length;
								}

								//string s = "";
								//if (Debugger.IsAttached) {
								//	stopwatch?.Stop();
								//	s = $" ticks: {stopwatch?.ElapsedTicks}, ms: {stopwatch?.ElapsedMilliseconds}, bytes: {new FileInfo(pathName).Length}";
								//}

								//$"Completed Saving Andro Mod World Data.{s}".LogSimple();

								byte[] array2 = null;
								if (FileUtilities.Exists(pathName, false))
									array2 = FileUtilities.ReadAllBytes(pathName, false);

								FileUtilities.Write(pathName, array, num, false);
								array = FileUtilities.ReadAllBytes(pathName, false);


								string s = "";
								if (Debugger.IsAttached) {
									SaveStopWatch?.Stop();
									s = $" ticks: {SaveStopWatch?.ElapsedTicks}, ms: {SaveStopWatch?.ElapsedMilliseconds}, bytes: {new FileInfo(pathName).Length}";
								}

								$"Completed Saving Andro Mod World Data.{s}".Log();
							});
						}
						finally {

						}
					}
					catch (Exception exception) {
						$"Failed to save world; exception: {exception}".Log();
						//FancyErrorPrinter.ShowFileSavingFailError(exception, Main.WorldPath);
						throw;
					}

					//Load
					bool flag = false /*&& SocialAPI.Cloud != null*/;
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

						Main.tile.ClearEverything();
						SaveWorld();
					}

					try {
						using MemoryStream memoryStream = new MemoryStream(FileUtilities.ReadAllBytes(pathName, flag));
						//using DeflateStream deflateStream = new DeflateStream(memoryStream, CompressionMode.Decompress, true);
						//using BinaryReader binaryReader = new BinaryReader(deflateStream);
						using BinaryReader binaryReader = new BinaryReader(memoryStream);
						try {
							TestLoadWorld(loadWorldFunctions[i], binaryReader);
							binaryReader.Close();
							memoryStream.Close();
							//UpdateLoadString();

							string s = "";
							if (Debugger.IsAttached) {
								LoadStopWatch?.Stop();
								s = $" ticks: {LoadStopWatch?.ElapsedTicks}, ms: {LoadStopWatch?.ElapsedMilliseconds}, bytes: {new FileInfo(pathName).Length}";
							}

							$"Completed Loading Andro Mod World Data.{s}".Log();

							//if (SaveString != LoadString)
							//	$"SaveString != LoadStting!!!!!".LogSimple();
						}
						catch (Exception lastThrownLoadException) {
							$"failed to load world. {lastThrownLoadException}".Log();
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
						$"failed to load world. {lastThrownLoadException2}".Log();
						return;
					}

					CheckLoadVsBeforeSave();
				}
			}
		}
		private static byte[,] worldDataBeforeSave = new byte[Main.maxTilesX, Main.maxTilesY];
		private static void SetTiles1() {
			TilePipeData[] tilePipeData = Main.tile.GetData<TilePipeData>();
			for (int x = 0; x < Main.maxTilesX; x++) {
				for (int y = 0; y < Main.maxTilesY; y++) {
					int z = x * Main.tile.Height + y;
					tilePipeData[z].HasPipe = true;
				}
			}

			Main.tile[10, 5].PipeType(13);
			Main.tile[10, 10].PipeData(0);

			worldDataBeforeSave = new byte[Main.maxTilesX, Main.maxTilesY];
			for (int x = 0; x < Main.maxTilesX; x++) {
				for (int y = 0; y < Main.maxTilesY; y++) {
					int z = x * Main.tile.Height + y;
					worldDataBeforeSave[x, y] = tilePipeData[z].PipeData;
				}
			}
		}
		private static void SetTiles2() {
			TilePipeData[] tilePipeData = Main.tile.GetData<TilePipeData>();
			worldDataBeforeSave = new byte[Main.maxTilesX, Main.maxTilesY];
			for (int x = 0; x < Main.maxTilesX; x++) {
				for (int y = 0; y < Main.maxTilesY; y++) {
					int z = x * Main.tile.Height + y;
					tilePipeData[z].PipeData = (byte)rand.Next(256);

					worldDataBeforeSave[x, y] = tilePipeData[z].PipeData;
				}
			}
		}
		private static void SetTiles3() {
			TilePipeData[] tilePipeData = Main.tile.GetData<TilePipeData>();
			worldDataBeforeSave = new byte[Main.maxTilesX, Main.maxTilesY];
			for (int x = 0; x < Main.maxTilesX; x++) {
				for (int y = 0; y < Main.maxTilesY; y++) {
					int z = x * Main.tile.Height + y;
					if (rand.Next(10000) == 1) {
						tilePipeData[z].PipeData = (byte)rand.Next(256);
					}

					worldDataBeforeSave[x, y] = tilePipeData[z].PipeData;
				}
			}
		}
		private static void CheckLoadVsBeforeSave() {
			TilePipeData[] tilePipeData = Main.tile.GetData<TilePipeData>();
			List<uint> diff = new();
			for (int x = 0; x < Main.maxTilesX; x++) {
				for (int y = 0; y < Main.maxTilesY; y++) {
					int z = x * Main.tile.Height + y;
					if (worldDataBeforeSave[x, y] != tilePipeData[z].PipeData) {
						//$"LoadVsBeforeSave failed at {x}, {y}".LogSimple();
						diff.Add((uint)z);
					}
				}
			}

			if (diff.Count > 0) {
				$"LoadVsBeforeSave failed at {diff.Count} locations".Log();
			}
			else {
				$"LoadVsBeforeSave passed".Log();
			}
		}
		private static Random rand = new Random();
		private static Stopwatch SaveStopWatch;
		private static Stopwatch LoadStopWatch;
		private static void TestSaveWorld(Action<BinaryWriter> saveWorld, BinaryWriter writer) {
			SaveStopWatch = Stopwatch.StartNew();
			saveWorld(writer);
			SaveStopWatch.Stop();
		}
		private static void TestLoadWorld(Action<BinaryReader> loadWorld, BinaryReader reader) {
			LoadStopWatch = Stopwatch.StartNew();
			loadWorld(reader);
			LoadStopWatch.Stop();
		}
		//private static void SaveWorld0(BinaryWriter writer) {
		//	writer.Write((ushort)Main.maxTilesX);
		//	writer.Write((ushort)Main.maxTilesY);
		//	uint count = (uint)Main.maxTilesX * (uint)Main.maxTilesY;
		//	TilePipeData[] tilePipeData = Main.tile.GetData<TilePipeData>();
		//	for (uint z = 0; z < count; z++) {
		//		writer.Write(tilePipeData[z].PipeData);
		//	}
		//}
		//private static void LoadWorld0(BinaryReader reader) {
		//	uint maxTilesX = reader.ReadUInt16();
		//	uint maxTilesY = reader.ReadUInt16();
		//	uint count = maxTilesX * maxTilesY;
		//	TilePipeData[] tilePipeData = Main.tile.GetData<TilePipeData>();
		//	for (uint z = 0; z < count;) {
		//		tilePipeData[z++].PipeData = reader.ReadByte();
		//	}
		//}
		//private static void SaveWorld1(BinaryWriter writer) {
		//	writer.Write((ushort)Main.maxTilesX);
		//	writer.Write((ushort)Main.maxTilesY);
		//	SortedSet<byte> pipeTypesSet = new();
		//	uint count = (uint)Main.maxTilesX * (uint)Main.maxTilesY;
		//	TilePipeData[] tilePipeData = Main.tile.GetData<TilePipeData>();
		//	for (uint z = 0; z < count; z++) {
		//		pipeTypesSet.Add(tilePipeData[z].PipeData);
		//	}

		//	byte pipeTypes = (byte)(pipeTypesSet.Count - 1);
		//	writer.Write(pipeTypes);
		//	uint numBitsNeededForPipeId = pipeTypes.BitsNeeded();
		//	uint maxBitsInTileCountRepresentation = count.BitsNeeded();

		//	byte[] pipeTypesArr = pipeTypesSet.ToArray();
		//	Dictionary<byte, byte> pipeIds = new();
		//	for (uint i = 0; i <= pipeTypes; i++) {
		//		byte pipeData = pipeTypesArr[i];
		//		pipeIds.Add(pipeData, (byte)i);

		//		writer.Write(pipeData);
		//	}

		//	for (uint z = 0; z < count;) {
		//		//Tile tile = z.GetTile();
		//		byte pipeData = tilePipeData[z].PipeData;
		//		if (pipeIds.ContainsKey(pipeData)) {
		//			writer.Write(pipeIds[pipeData]);
		//		}
		//		else {
		//			$"Failed to find {pipeData} in pipeIDs. z: {z}".LogSimple();
		//		}

		//		//write a maxBitsInTileCountRepresentation to tell how many bits should be read for the number of tiles
		//		uint startZ = z;
		//		while (++z < count) {
		//			byte otherData = tilePipeData[z].PipeData;
		//			if (otherData != pipeData)
		//				break;
		//		}

		//		uint tileCount = z - startZ;//Make sure z is decremented after the -
		//		writer.Write(tileCount);
		//		//write tileCount as a maxBitsInTileCountRepresentation.//TODO:
		//	}
		//}
		//private static void LoadWorld1(BinaryReader reader) {
		//	uint maxTilesX = reader.ReadUInt16();
		//	uint maxTilesY = reader.ReadUInt16();

		//	byte pipeTypes = reader.ReadByte();
		//	uint count = maxTilesX * maxTilesY;
		//	uint numBitsNeededForPipeId = pipeTypes.BitsNeeded();//TODO:
		//	uint maxBitsInTileCountRepresentation = count.BitsNeeded();//TODO:

		//	Dictionary<byte, byte> pipeIds = new();//reverse of reader
		//	for (uint i = 0; i <= pipeTypes; i++) {
		//		pipeIds.Add((byte)i, reader.ReadByte());
		//	}

		//	TilePipeData[] tilePipeData = Main.tile.GetData<TilePipeData>();
		//	for (uint z = 0; z < count;) {
		//		byte pipeData = pipeIds[reader.ReadByte()];
		//		uint tileCount = reader.ReadUInt32();
		//		uint zEnd = z + tileCount;
		//		if (pipeData == TilePipeData.Empty) {
		//			z = zEnd;
		//		}
		//		else {
		//			while (z < zEnd) {
		//				tilePipeData[z++].PipeData = pipeData;
		//			}
		//		}
		//	}
		//}
		//private static void SaveWorld2(BinaryWriter writer) {
		//	//SaveWorldTesting(writer);
		//	//return;
		//	writer.Write((ushort)Main.maxTilesX);
		//	writer.Write((ushort)Main.maxTilesY);

		//	//determine pipeTypes byte(bits needed to represent largest number)
		//	//write Main.maxTilesX
		//	//write Main.maxTilesY
		//	SortedSet<byte> pipeTypesSet = new();
		//	uint count = (uint)Main.maxTilesX * (uint)Main.maxTilesY;
		//	uint inARowCounter = 0;
		//	uint inARowHighest = 0;
		//	byte tileBeingCounted = 0;//This may end up 1 higher than it is, but that's insignificant.
		//	TilePipeData[] tilePipeData = Main.tile.GetData<TilePipeData>();
		//	for (uint z = 0; z < count; z++) {
		//		byte tileData = tilePipeData[z].PipeData;
		//		pipeTypesSet.Add(tileData);
		//		if (tileData == tileBeingCounted) {
		//			inARowCounter++;
		//		}
		//		else {
		//			if (inARowHighest < inARowCounter)
		//				inARowHighest = inARowCounter;

		//			tileBeingCounted = tileData;
		//			inARowCounter = 1;
		//		}
		//	}

		//	if (inARowHighest < inARowCounter)
		//		inARowHighest = inARowCounter;

		//	byte pipeTypes = (byte)(pipeTypesSet.Count - 1);
		//	writer.Write(pipeTypes);
		//	uint numBitsNeededForPipeId = pipeTypes.BitsNeeded();
		//	uint maxBitsInTileCountRepresentation = inARowHighest.BitsNeeded();
		//	writer.Write(maxBitsInTileCountRepresentation);

		//	byte[] pipeTypesArr = pipeTypesSet.ToArray();
		//	bool usesIdLookup = numBitsNeededForPipeId < 8;
		//	writer.Write(usesIdLookup);
		//	//$"SavingWorld; numBitsNeededForPipeId: {numBitsNeededForPipeId}, maxBitsInTileCountRepresentation: {maxBitsInTileCountRepresentation}, usesIdLookup: {usesIdLookup}, pipeTypes: {pipeTypes}, count: {count}".LogSimple();
		//	if (usesIdLookup) {
		//		Dictionary<byte, uint_b> pipeIds = new();
		//		for (uint i = 0; i <= pipeTypes; i++) {
		//			byte pipeType = pipeTypesArr[i];
		//			pipeIds.Add(pipeType, new(i, numBitsNeededForPipeId));
		//			writer.Write(pipeType);
		//			//$"pipeIds[{pipeType} ({pipeType.ToBinaryString()})]: {i} ({i.ToBinaryString(numBitsNeededForPipeId)})".LogSimple();
		//		}

		//		for (uint z = 0; z < count;) {
		//			byte pipeData = tilePipeData[z].PipeData;
		//			writer.WriteNum(pipeIds[pipeData]);

		//			uint startZ = z;
		//			while (++z < count) {
		//				byte otherData = tilePipeData[z].PipeData;
		//				if (otherData != pipeData)
		//					break;
		//			}

		//			uint tileCount = z - startZ;
		//			writer.WriteNum(tileCount, maxBitsInTileCountRepresentation);//Can be taken 1 step further by using 5 bits to determining how long the current number is instead and writing those 5 and the number.
		//																		 //$"pipeData: {pipeData.ToBinaryString()}, pipeID: {pipeIds[pipeData]}, tileCount: {tileCount} ({tileCount.ToBinaryString(maxBitsInTileCountRepresentation)}), z: {z}".LogSimple();
		//		}
		//	}
		//	else {
		//		for (uint z = 0; z < count;) {
		//			byte pipeData = tilePipeData[z].PipeData;
		//			writer.WriteNum(pipeData, numBitsNeededForPipeId);

		//			uint startZ = z;
		//			while (++z < count) {
		//				byte otherData = tilePipeData[z].PipeData;
		//				if (otherData != pipeData)
		//					break;
		//			}

		//			uint tileCount = z - startZ;
		//			writer.WriteNum(tileCount, maxBitsInTileCountRepresentation);//Can be taken 1 step further by using 5 bits to determining how long the current number is instead and writing those 5 and the number.
		//		}
		//	}

		//	writer.Finish();
		//}
		//private static void LoadWorld2(BinaryReader reader) {
		//	uint maxTilesX = reader.ReadUInt16();
		//	uint maxTilesY = reader.ReadUInt16();

		//	byte pipeTypes = reader.ReadByte();
		//	uint count = maxTilesX * maxTilesY;
		//	uint numBitsNeededForPipeId = pipeTypes.BitsNeeded();
		//	uint maxBitsInTileCountRepresentation = reader.ReadUInt32();
		//	bool usesIdLookup = reader.ReadBoolean();
		//	TilePipeData[] tilePipeData = Main.tile.GetData<TilePipeData>();
		//	//$"\n\nLoadingWorld; numBitsNeededForPipeId: {numBitsNeededForPipeId}, maxBitsInTileCountRepresentation: {maxBitsInTileCountRepresentation}, usesIdLookup: {usesIdLookup}, pipeTypes: {pipeTypes}, count: {count}".LogSimple();
		//	if (usesIdLookup) {
		//		Dictionary<uint, byte> pipeIds = new();//reverse of reader
		//		for (uint i = 0; i <= pipeTypes; i++) {
		//			byte pipeData = reader.ReadByte();
		//			pipeIds.Add(i, pipeData);
		//			//$"pipeIds[{i} ({i.ToBinaryString(numBitsNeededForPipeId)})]: {pipeData} ({pipeData.ToBinaryString()})".LogSimple();
		//		}

		//		reader.Start();
		//		for (uint z = 0; z < count;) {
		//			uint pipeId = reader.ReadNum(numBitsNeededForPipeId);
		//			//if (!pipeIds.TryGetValue(pipeId, out _)) {
		//			//	$"pipeIds doesn't contain {pipeId}".LogSimple();
		//			//}

		//			byte pipeData = pipeIds[pipeId];
		//			uint tileCount = reader.ReadNum(maxBitsInTileCountRepresentation);
		//			uint zEnd = z + tileCount;
		//			//if (pipeData == TilePipeData.Empty) {
		//			//	z = zEnd;
		//			//}
		//			//else {
		//			//	while (z < zEnd) {
		//			//		//if (z >= tilePipeData.Length)
		//			//		//	$"z >= tilePipeData.Length".LogSimple();

		//			//		tilePipeData[z++].PipeData = pipeData;
		//			//	}
		//			//}
		//			while (z < zEnd) {
		//				//if (z >= tilePipeData.Length)
		//				//	$"z >= tilePipeData.Length".LogSimple();

		//				tilePipeData[z++].PipeData = pipeData;
		//			}

		//			//$"pipeData: {pipeData.ToBinaryString()}, pipeID: {pipeId} ({pipeId.ToBinaryString(numBitsNeededForPipeId)}), tileCount: {tileCount} ({tileCount.ToBinaryString(maxBitsInTileCountRepresentation)}), z: {z}".LogSimple();
		//		}
		//	}
		//	else {
		//		reader.Start();
		//		for (uint z = 0; z < count;) {
		//			byte pipeData = (byte)reader.ReadNum(numBitsNeededForPipeId);
		//			uint tileCount = reader.ReadNum(maxBitsInTileCountRepresentation);
		//			uint zEnd = z + tileCount;
		//			if (pipeData == TilePipeData.Empty) {
		//				z = zEnd;
		//			}
		//			else {
		//				while (z < zEnd) {
		//					//if (z >= tilePipeData.Length) {
		//					//	CheckLoadVsBeforeSave();
		//					//	$"z >= tilePipeData.Length".LogSimple();
		//					//}

		//					tilePipeData[z++].PipeData = pipeData;
		//				}
		//			}
		//		}
		//	}

		//	reader.Finish();
		//}
		/*
		private static void Save(BinaryWriter writer) {
			//SaveWorldTesting(writer);
			//return;
			writer.Write((ushort)Main.maxTilesX);
			writer.Write((ushort)Main.maxTilesY);
			writer.Write((ushort)Main.tile.Height);

			//determine pipeTypes byte(bits needed to represent largest number)
			//write Main.maxTilesX
			//write Main.maxTilesY
			SortedSet<byte> pipeTypesSet = new();
			uint inARowCounter = 0;
			uint inARowHighest = 0;
			byte tileBeingCounted = 0;//This may end up 1 higher than it is, but that's insignificant.
			TilePipeData[] tilePipeData = Main.tile.GetData<TilePipeData>();
			for (uint x = 0; x < Main.maxTilesX; x++) {
				for (uint y = 0; y < Main.maxTilesY; y++) {
					uint z = x * Main.tile.Height + y;
					byte tileData = tilePipeData[z].PipeData;
					pipeTypesSet.Add(tileData);
					if (tileData == tileBeingCounted) {
						inARowCounter++;
					}
					else {
						if (inARowHighest < inARowCounter)
							inARowHighest = inARowCounter;

						tileBeingCounted = tileData;
						inARowCounter = 1;
					}
				}
			}

			if (inARowHighest < inARowCounter)
				inARowHighest = inARowCounter;

			byte pipeTypes = (byte)(pipeTypesSet.Count - 1);
			writer.Write(pipeTypes);
			uint numBitsNeededForPipeId = pipeTypes.BitsNeeded();
			uint maxBitsInTileCountRepresentation = inARowHighest.BitsNeeded();
			writer.Write(maxBitsInTileCountRepresentation);

			byte[] pipeTypesArr = pipeTypesSet.ToArray();
			bool usesIdLookup = numBitsNeededForPipeId < 8;
			writer.Write(usesIdLookup);
			$"SavingWorld; numBitsNeededForPipeId: {numBitsNeededForPipeId}, maxBitsInTileCountRepresentation: {maxBitsInTileCountRepresentation}, usesIdLookup: {usesIdLookup}, pipeTypes: {pipeTypes}".LogSimple();
			if (usesIdLookup) {
				Dictionary<byte, uint_b> pipeIds = new();
				for (uint i = 0; i <= pipeTypes; i++) {
					byte pipeType = pipeTypesArr[i];
					pipeIds.Add(pipeType, new(i, numBitsNeededForPipeId));
					writer.Write(pipeType);
					$"pipeIds[{pipeType} ({pipeType.ToBinaryString()})]: {i} ({i.ToBinaryString(numBitsNeededForPipeId)})".LogSimple();
				}

				for (uint x = 0; x < Main.maxTilesX;) {
					for (uint y = 0; y < Main.maxTilesY;) {
						uint z = x * Main.tile.Height + y;
						byte pipeData = tilePipeData[z].PipeData;
						writer.WriteNum(pipeIds[pipeData]);

						uint tileCount = 1;
						while (y < Main.maxTilesY) {
							z = x * Main.tile.Height + y++;
							byte otherData = tilePipeData[z].PipeData;
							if (y == Main.maxTilesY) {
								x++;
								if (x == Main.maxTilesX) {
									$"pipeData: {pipeData.ToBinaryString()}, pipeID: {pipeIds[pipeData]}, tileCount: {tileCount} ({tileCount.ToBinaryString(maxBitsInTileCountRepresentation)}), z: {z}, x: {x}, y: {y}".LogSimple();
									break;
								}

								y = 0;
							}

							if (otherData != pipeData) {
								$"pipeData: {pipeData.ToBinaryString()}, pipeID: {pipeIds[pipeData]}, tileCount: {tileCount} ({tileCount.ToBinaryString(maxBitsInTileCountRepresentation)}), z: {z}, x: {x}, y: {y}".LogSimple();
								break;
							}

							tileCount++;
						}

						writer.WriteNum(tileCount, maxBitsInTileCountRepresentation);//Can be taken 1 step further by using 5 bits to determining how long the current number is instead and writing those 5 and the number.
						$"pipeData: {pipeData.ToBinaryString()}, pipeID: {pipeIds[pipeData]}, tileCount: {tileCount} ({tileCount.ToBinaryString(maxBitsInTileCountRepresentation)}), z: {z}, x: {x}, y: {y}".LogSimple();
					}
				}
			}
			else {
				for (uint x = 0; x < Main.maxTilesX;) {
					for (uint y = 0; y < Main.maxTilesY;) {
						uint z = x * Main.tile.Height + y;
						byte pipeData = tilePipeData[z].PipeData;
						writer.WriteNum(pipeData, numBitsNeededForPipeId);

						uint tileCount = 1;
						while (y < Main.maxTilesY) {
							z = x * Main.tile.Height + y++;
							if (z >= tilePipeData.Length)
								$"z out of index error; z: {z}, x: {x}, y: {y}".LogSimple();

							byte otherData = tilePipeData[z].PipeData;
							if (y == Main.maxTilesY) {
								x++;
								if (x == Main.maxTilesX) {
									$"pipeData: {pipeData.ToBinaryString()}, pipeData: {pipeData}, tileCount: {tileCount} ({tileCount.ToBinaryString(maxBitsInTileCountRepresentation)}), z: {z}, x: {x}, y: {y}".LogSimple();
									break;
								}

								y = 0;
							}

							if (otherData != pipeData) {
								$"pipeData: {pipeData.ToBinaryString()}, pipeData: {pipeData}, tileCount: {tileCount} ({tileCount.ToBinaryString(maxBitsInTileCountRepresentation)}), z: {z}, x: {x}, y: {y}".LogSimple();
								break;
							}

							tileCount++;
						}

						writer.WriteNum(tileCount, maxBitsInTileCountRepresentation);//Can be taken 1 step further by using 5 bits to determining how long the current number is instead and writing those 5 and the number.
						$"pipeData: {pipeData.ToBinaryString()}, pipeData: {pipeData}, tileCount: {tileCount} ({tileCount.ToBinaryString(maxBitsInTileCountRepresentation)}), z: {z}, x: {x}, y: {y}".LogSimple();
					}
				}
			}

			writer.Finish();
		}
		private static void Load(BinaryReader reader) {
			uint maxTilesX = reader.ReadUInt16();
			uint maxTilesY = reader.ReadUInt16();
			uint tileHeight = reader.ReadUInt16();
			byte pipeTypes = reader.ReadByte();
			uint numBitsNeededForPipeId = pipeTypes.BitsNeeded();
			uint maxBitsInTileCountRepresentation = reader.ReadUInt32();
			bool usesIdLookup = reader.ReadBoolean();
			TilePipeData[] tilePipeData = Main.tile.GetData<TilePipeData>();
			$"\n\nLoadingWorld; numBitsNeededForPipeId: {numBitsNeededForPipeId}, maxBitsInTileCountRepresentation: {maxBitsInTileCountRepresentation}, usesIdLookup: {usesIdLookup}, pipeTypes: {pipeTypes}".LogSimple();
			if (usesIdLookup) {
				Dictionary<uint, byte> pipeIds = new();//reverse of reader
				for (uint i = 0; i <= pipeTypes; i++) {
					byte pipeData = reader.ReadByte();
					pipeIds.Add(i, pipeData);
					$"pipeIds[{i} ({i.ToBinaryString(numBitsNeededForPipeId)})]: {pipeData} ({pipeData.ToBinaryString()})".LogSimple();
				}

				reader.Start();
				for (uint x = 0; x < maxTilesX;) {
					for (uint y = 0; y < maxTilesY;) {
						uint z = x * tileHeight + y;
						uint pipeId = reader.ReadNum(numBitsNeededForPipeId);
						if (!pipeIds.TryGetValue(pipeId, out _)) {
							$"pipeIds doesn't contain {pipeId}".LogSimple();
						}

						byte pipeData = pipeIds[pipeId];
						uint tileCount = reader.ReadNum(maxBitsInTileCountRepresentation);
						uint initialTileCount = tileCount;
						while (tileCount > 0) {
							//if (z >= tilePipeData.Length)
							//	$"z >= tilePipeData.Length".LogSimple();

							tilePipeData[z].PipeData = pipeData;
							z = x * tileHeight + y++;
							if (y == maxTilesY) {
								x++;
								if (x == maxTilesX) {
									$"pipeData: {pipeData.ToBinaryString()}, pipeID: {pipeId} ({pipeId.ToBinaryString(numBitsNeededForPipeId)}), initialTileCount: {initialTileCount}, tileCount: {tileCount} ({tileCount.ToBinaryString(maxBitsInTileCountRepresentation)}), z: {z}, x: {x}, y: {y}".LogSimple();
									break;
								}

								y = 0;
							}

							tileCount--;
						}

						$"pipeData: {pipeData.ToBinaryString()}, pipeID: {pipeId} ({pipeId.ToBinaryString(numBitsNeededForPipeId)}), initialTileCount: {initialTileCount}, tileCount: {tileCount} ({tileCount.ToBinaryString(maxBitsInTileCountRepresentation)}), z: {z}, x: {x}, y: {y}".LogSimple();
					}
				}
			}
			else {
				reader.Start();
				for (uint x = 0; x < maxTilesX;) {
					for (uint y = 0; y < maxTilesY;) {
						uint z = x * tileHeight + y;
						//if (!pipeIds.TryGetValue(pipeId, out _)) {
						//	$"pipeIds doesn't contain {pipeId}".LogSimple();
						//}
						byte pipeData = (byte)reader.ReadNum(numBitsNeededForPipeId);
						uint tileCount = reader.ReadNum(maxBitsInTileCountRepresentation);
						uint initialTileCount = tileCount;
						while (tileCount > 0) {
							//if (z >= tilePipeData.Length)
							//	$"z >= tilePipeData.Length".LogSimple();

							tilePipeData[z].PipeData = pipeData;
							z = x * tileHeight + y++;
							if (y == maxTilesY) {
								x++;
								if (x == maxTilesX) {
									$"pipeData: {pipeData.ToBinaryString()}, pipeData: {pipeData} ({pipeData.ToBinaryString(numBitsNeededForPipeId)}), initialTileCount: {initialTileCount}, tileCount: {tileCount} ({tileCount.ToBinaryString(maxBitsInTileCountRepresentation)}), z: {z}, x: {x}, y: {y}".LogSimple();
									break;
								}

								y = 0;
							}

							tileCount--;
						}

						$"pipeData: {pipeData.ToBinaryString()}, pipeData: {pipeData} ({pipeData.ToBinaryString(numBitsNeededForPipeId)}), initialTileCount: {initialTileCount}, tileCount: {tileCount} ({tileCount.ToBinaryString(maxBitsInTileCountRepresentation)}), z: {z}, x: {x}, y: {y}".LogSimple();
					}
				}
			}

			reader.Finish();
		}
		*/
		private static void Save(BinaryWriter writer) {
			writer.Write((ushort)Main.maxTilesX);
			writer.Write((ushort)Main.maxTilesY);
			writer.Write((ushort)Main.tile.Height);

			SortedSet<byte> pipeTypesSet = new();
			uint inARowCounter = 0;
			uint inARowHighest = 0;
			byte tileBeingCounted = 0;
			TilePipeData[] tilePipeData = Main.tile.GetData<TilePipeData>();
			for (uint x = 0; x < Main.maxTilesX; x++) {
				for (uint y = 0; y < Main.maxTilesY; y++) {
					uint z = x * Main.tile.Height + y;
					byte tileData = tilePipeData[z].PipeData;
					pipeTypesSet.Add(tileData);
					if (tileData == tileBeingCounted) {
						inARowCounter++;
					}
					else {
						if (inARowHighest < inARowCounter)
							inARowHighest = inARowCounter;

						tileBeingCounted = tileData;
						inARowCounter = 1;
					}
				}
			}

			if (inARowHighest < inARowCounter)
				inARowHighest = inARowCounter;

			byte pipeTypes = (byte)(pipeTypesSet.Count - 1);
			writer.Write(pipeTypes);
			uint numBitsNeededForPipeId = pipeTypes.BitsNeeded();
			uint maxBitsInTileCountRepresentation = inARowHighest.BitsNeeded();
			writer.Write(maxBitsInTileCountRepresentation);

			byte[] pipeTypesArr = pipeTypesSet.ToArray();
			bool usesIdLookup = numBitsNeededForPipeId < 8;
			writer.Write(usesIdLookup);
			//$"SavingWorld; numBitsNeededForPipeId: {numBitsNeededForPipeId}, maxBitsInTileCountRepresentation: {maxBitsInTileCountRepresentation}, usesIdLookup: {usesIdLookup}, pipeTypes: {pipeTypes}".LogSimple();
			if (true || usesIdLookup) {
				Dictionary<byte, uint_b> pipeIds = new();
				for (uint i = 0; i <= pipeTypes; i++) {
					byte pipeType = pipeTypesArr[i];
					pipeIds.Add(pipeType, new(i, numBitsNeededForPipeId));
					writer.Write(pipeType);
					//$"pipeIds[{pipeType} ({pipeType.ToBinaryString()})]: {i} ({i.ToBinaryString(numBitsNeededForPipeId)})".LogSimple();
				}

				uint tileCount = 1;
				byte pipeData = tilePipeData[0].PipeData;
				//$"pipeData: {pipeData.ToBinaryString()}, tileCount: {tileCount} ({tileCount.ToBinaryString(maxBitsInTileCountRepresentation)}), z: 0, x: 0, y: 0".LogSimple();
				uint x = 0;
				uint y = 1;
				uint z = 0;
				for (; x < Main.maxTilesX; x++) {
					for (; y < Main.maxTilesY; y++) {
						z = x * Main.tile.Height + y;
						byte otherPipeData = tilePipeData[z].PipeData;
						if (otherPipeData == pipeData) {
							tileCount++;
						}
						else {
							//$"-pipeData: {pipeData.ToBinaryString()}, pipeID: {pipeIds[pipeData]}, tileCount: {tileCount} ({tileCount.ToBinaryString(maxBitsInTileCountRepresentation)}), z: {z - 1}, x: {(y == 0 ? x - 1 : x)}, y: {(y == 0 ? Main.maxTilesY - 1 : y - 1)}".LogSimple();
							writer.WriteNum(pipeIds[pipeData]);
							writer.WriteNum(tileCount, maxBitsInTileCountRepresentation);
							pipeData = otherPipeData;
							tileCount = 1;
							//$"pipeData: {pipeData.ToBinaryString()}, pipeID: {pipeIds[pipeData]}, tileCount: {tileCount} ({tileCount.ToBinaryString(maxBitsInTileCountRepresentation)}), z: {z}, x: {x}, y: {y}".LogSimple();
						}

						//if (x == Main.maxTilesX - 1 && y == Main.maxTilesY - 1) $"-pipeData: {pipeData.ToBinaryString()}, pipeID: {pipeIds[pipeData]}, tileCount: {tileCount} ({tileCount.ToBinaryString(maxBitsInTileCountRepresentation)}), z: {z}, x: {x}, y: {y}".LogSimple();
					}

					y = 0;
				}

				writer.WriteNum(pipeIds[pipeData]);
				writer.WriteNum(tileCount, maxBitsInTileCountRepresentation);
			}
			else {
				uint tileCount = 1;
				byte pipeData = tilePipeData[0].PipeData;
				//$"pipeData: {pipeData.ToBinaryString()}, tileCount: {tileCount} ({tileCount.ToBinaryString(maxBitsInTileCountRepresentation)}), z: 0, x: 0, y: 0".LogSimple();
				uint x = 0;
				uint y = 1;
				uint z = 0;
				for (; x < Main.maxTilesX; x++) {
					for (; y < Main.maxTilesY; y++) {
						z = x * Main.tile.Height + y;
						byte otherPipeData = tilePipeData[z].PipeData;
						if (otherPipeData == pipeData) {
							tileCount++;
						}
						else {
							//$"-pipeData: {pipeData.ToBinaryString()}, pipeID: {pipeIds[pipeData]}, tileCount: {tileCount} ({tileCount.ToBinaryString(maxBitsInTileCountRepresentation)}), z: {z - 1}, x: {(y == 0 ? x - 1 : x)}, y: {(y == 0 ? Main.maxTilesY - 1 : y - 1)}".LogSimple();
							writer.WriteNum(pipeData, numBitsNeededForPipeId);
							writer.WriteNum(tileCount, maxBitsInTileCountRepresentation);
							pipeData = otherPipeData;
							tileCount = 1;
							//$"pipeData: {pipeData.ToBinaryString()}, pipeID: {pipeIds[pipeData]}, tileCount: {tileCount} ({tileCount.ToBinaryString(maxBitsInTileCountRepresentation)}), z: {z}, x: {x}, y: {y}".LogSimple();
						}

						//if (x == Main.maxTilesX - 1 && y == Main.maxTilesY - 1) $"-pipeData: {pipeData.ToBinaryString()}, pipeID: {pipeIds[pipeData]}, tileCount: {tileCount} ({tileCount.ToBinaryString(maxBitsInTileCountRepresentation)}), z: {z}, x: {x}, y: {y}".LogSimple();
					}

					y = 0;
				}

				writer.WriteNum(pipeData, numBitsNeededForPipeId);
				writer.WriteNum(tileCount, maxBitsInTileCountRepresentation);
			}

			writer.Finish();
		}
		private static void Load(BinaryReader reader) {
			uint maxTilesX = reader.ReadUInt16();
			uint maxTilesY = reader.ReadUInt16();
			uint tileHeight = reader.ReadUInt16();
			byte pipeTypes = reader.ReadByte();
			uint numBitsNeededForPipeId = pipeTypes.BitsNeeded();
			uint maxBitsInTileCountRepresentation = reader.ReadUInt32();
			bool usesIdLookup = reader.ReadBoolean();
			TilePipeData[] tilePipeData = Main.tile.GetData<TilePipeData>();
			//$"\n\nLoadingWorld; numBitsNeededForPipeId: {numBitsNeededForPipeId}, maxBitsInTileCountRepresentation: {maxBitsInTileCountRepresentation}, usesIdLookup: {usesIdLookup}, pipeTypes: {pipeTypes}".LogSimple();
			if (true || usesIdLookup) {
				Dictionary<uint, byte> pipeIds = new();//reverse of reader
				for (uint i = 0; i <= pipeTypes; i++) {
					byte pipeId2 = reader.ReadByte();
					pipeIds.Add(i, pipeId2);
					//$"pipeIds[{i} ({i.ToBinaryString(numBitsNeededForPipeId)})]: {pipeId2} ({pipeId2.ToBinaryString()})".LogSimple();
				}

				byte pipeData = 0;
				uint tileCount = 0;
				uint pipeId = 0;
				for (uint x = 0; x < maxTilesX; x++) {
					for (uint y = 0; y < maxTilesY; y++) {
						uint z = x * tileHeight + y;
						if (tileCount == 0) {
							pipeId = reader.ReadNum(numBitsNeededForPipeId);
							pipeData = pipeIds[pipeId];
							tileCount = reader.ReadNum(maxBitsInTileCountRepresentation);
							//$"pipeData: {pipeData.ToBinaryString()}, pipeID: {pipeId} ({pipeId.ToBinaryString(numBitsNeededForPipeId)}), tileCount: {tileCount} ({tileCount.ToBinaryString(maxBitsInTileCountRepresentation)}), z: {z}, x: {x}, y: {y}".LogSimple();
						}

						tilePipeData[z].PipeData = pipeData;
						tileCount--;

						//if (tileCount == 0) $"-pipeData: {pipeData.ToBinaryString()}, pipeID: {pipeId} ({pipeId.ToBinaryString(numBitsNeededForPipeId)}), tileCount: {tileCount} ({tileCount.ToBinaryString(maxBitsInTileCountRepresentation)}), z: {z}, x: {x}, y: {y}".LogSimple();
					}
				}
			}
			else {
				byte pipeData = 0;
				uint tileCount = 0;
				for (uint x = 0; x < maxTilesX; x++) {
					for (uint y = 0; y < maxTilesY; y++) {
						uint z = x * tileHeight + y;
						if (tileCount == 0) {
							pipeData = (byte)reader.ReadNum(numBitsNeededForPipeId);
							tileCount = reader.ReadNum(maxBitsInTileCountRepresentation);
							//$"pipeData: {pipeData.ToBinaryString()}, pipeID: {pipeId} ({pipeId.ToBinaryString(numBitsNeededForPipeId)}), tileCount: {tileCount} ({tileCount.ToBinaryString(maxBitsInTileCountRepresentation)}), z: {z}, x: {x}, y: {y}".LogSimple();
						}

						tilePipeData[z].PipeData = pipeData;
						tileCount--;

						//if (tileCount == 0) $"-pipeData: {pipeData.ToBinaryString()}, pipeID: {pipeId} ({pipeId.ToBinaryString(numBitsNeededForPipeId)}), tileCount: {tileCount} ({tileCount.ToBinaryString(maxBitsInTileCountRepresentation)}), z: {z}, x: {x}, y: {y}".LogSimple();
					}
				}
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
				$"Failed to create directory; exception: {exception}, folderPath: {folderPath}".Log();
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
		public static int maxTilesX = 4200;// (int)rightWorld / 16 + 1;
		public static int maxTilesY = 1200;//  (int)bottomWorld / 16 + 1;
		//public static int maxTilesX = 30;
		//public static int maxTilesY = 20;
		public static Tilemap tile = new((ushort)maxTilesX, (ushort)maxTilesY);
		public static bool autoGen = true;
	}
}
