using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TerrariaPathFinderTesting {
	public static class TerrariaPathFinderTesting {
		private static bool IsPath(int x, int y) {
			int tileType = MainT.tile[x, y].TileType;
			return tileType == 1;
		}
		private static bool IsTarget(int x, int y) {
			int tileType = MainT.tile[x, y].TileType;
			return tileType == 3;
		}
		public static void Test() {
			"test".LogSimple();
			MainT.tile.WriteAll();
			//C.HasPath(0, 9, 6, 6, 20, (x, y) => MainT.tile[x, y].TileType == 1, MainT.tileMaxX - 1, MainT.tileMaxY - 1);
			C.HasPath2(9, 0, 20, IsPath, IsTarget, MainT.tileMaxX - 1, MainT.tileMaxY - 1);
		}
	}

	public static class TileID {
		public const int Air = 0;
		public const int MudBlock = 1;
		public const int Other = 2;
	}

	public struct Tile2 {
		public int TileType;
		public bool HasTile;
		public Tile2(int type) {
			TileType = type;
			HasTile = TileType != TileID.Air;
		}
	}

	public static class C {
		public static double Distance(int x, int y, int x2, int y2) {
			return Math.Sqrt(Math.Pow(x2 - x, 2) + Math.Pow(y2 - y, 2));
		}
		//
		public static bool HasPathVariable = true;
		//public static bool HasPath(int x, int y, int TargetX, int TargetY, int MaxDistance, Func<int, int, bool> CountsAsPath, int XMax, int YMax, int XMin = 0, int YMin = 0) {
		//	if (x == targetX && y == targetY)
		//		return true;

		//	targetX = TargetX;
		//	targetY = TargetY;
		//	countsAsPath = CountsAsPath;
		//	maxDistance = MaxDistance;
		//	xMax = XMax;
		//	xMin = XMin;
		//	yMax = YMax;
		//	yMin = YMin;
		//	int gridSize = (maxDistance + 1) * 2;
		//	PathGrid = new bool[gridSize, gridSize];
		//	int currentDist = Math.Abs(targetX - x) + Math.Abs(targetY - y);

		//	bool hasPath = FindPath(x, y, currentDist);

		//	PathGrid = null;
		//	resultPath.Log();
		//	resultPath = null;

		//	return hasPath;
		//}
		public class Element<K, T> where K : IComparable {
			public Element(K key, T value, Element<K, T> prev = null, Element<K, T> next = null) {
				this.key = key;
				this.value = value;
				this.prev = prev;
				this.next = next;
			}

			public K key;
			public T value;
			public Element<K, T> next;
			public Element<K, T> prev;
		}
		public class OrderList<K, T> where K : IComparable {
			public OrderList() { }
			public Element<K, T> first = null;
			public Element<K, T> last = null;
			public void Add(K key, T value) {
				if (first == null) {
					first = new(key, value);
					last = first;
					return;
				}

				for (Element<K, T> current = first; current != null; current = current.next) {
					if (current.key.CompareTo(key) < 0) {
						Element<K, T> newElement = new(key, value, current.prev, current);
						current.prev = newElement;
						if (newElement.prev != null) {
							newElement.prev.next = newElement;
						}
						else {
							first = newElement;
						}

						return;
					}
				}

				last.next = new(key, value, last);
				last = last.next;
			}
		}
		private static void GetBoundaries(int x, int y, int xMin, int xMax, int yMin, int yMax, int radius, out int left, out int up, out int right, out int down) {
			left = Math.Max(-radius, xMin - x);
			up = Math.Max(-radius, yMin - y);
			right = Math.Min(radius, xMax - x);
			down = Math.Min(radius, yMax - y);
		}
		public static bool HasPath2(int x, int y, int MaxDistance, Func<int, int, bool> CountsAsPath, Func<int, int, bool> CountsAsTarget, int XMax, int YMax, int XMin = 0, int YMin = 0) {
			countsAsPath = CountsAsPath;
			countsAsTarget = CountsAsTarget;
			maxDistance = MaxDistance;
			GetBoundaries(x, y, XMin, XMax, YMin, YMax, maxDistance, out int left, out int up, out int right, out int down);
			int gridSizeX = -left + right + 1;
			int gridSizeY = -up + down + 1;
			int centerX = -left;
			int centerY = -up;
			xStart = x - centerX;
			yStart = y - centerY;
			xMax = gridSizeX - 1;
			xMin = 0;
			yMax = gridSizeY - 1;
			yMin = 0;
			UtilityMethods.FillArray(ref PathGrid, gridSizeX, gridSizeY, int.MaxValue);
			PathGrid[centerX, centerY] = 0;

			bool hasPath = FindPath2(centerX, centerY, 0);

			PathGrid = null;
			resultPath.LogSimple();
			resultPath = null;

			return hasPath;
		}
		//public delegate bool FindPathDelegate(int xStart, int yStart, int x, int y, int targetX, int targetY, float maxDistance, Func<Tile, bool> countsAsPath);
		private static int[,] PathGrid;
		private static string resultPath;
		//private static int targetX;
		//private static int targetY;
		private static int xStart;
		private static int yStart;
		private static Func<int, int, bool> countsAsPath;
		private static Func<int, int, bool> countsAsTarget;
		private static int maxDistance;
		private static int xMax;
		private static int yMax;
		private static int xMin;
		private static int yMin;
		//private static bool FindPath(int x, int y, int currentDistance, int fromDirection = -1) {
		//	$"x: {x}, y: {y}, currentDistance: {currentDistance}, from: {fromDirection}".Log();
		//	OrderList<int, int> order = new();
		//	int xSign = x > targetX ? -1 : 1;
		//	int ySign = y > targetY ? -1 : 1;
		//	//int directionID = -1;
		//	int opposite = fromDirection + (fromDirection % 2 == 0 ? 1 : -1);
		//	for (int directionID = 0; directionID < 4; directionID++) {
		//		int i = directionID % 2;
		//		int j = 1 - i;
		//		if (directionID > 1) {
		//			i *= -1;
		//			j *= -1;
		//		}

		//		//}
		//		//for (int i = -1; i <= 1; i += 2) {
		//		//for (int j = -1; j <= 1; j += 2) {
		//		$"i: {i}, j: {j}, directionID: {directionID}, opposite: {opposite}".Log();
		//		if (opposite == directionID)
		//			continue;
		//		//$"i: {i}, j: {j} 2".Log();
		//		int x2 = x + i;
		//		//$"x2: {x2}, xMin: {xMin}, xMax: {xMax}".Log();
		//		if (x2 < xMin || x2 > xMax)
		//			continue;
		//		//$"i: {i}, j: {j} 3".Log();
		//		int y2 = y + j;
		//		if (y2 < yMin || y2 > yMax)
		//			continue;

		//		$"x2: {x2}, y2: {y2} Before PathGrid[]".Log();
		//		if (PathGrid[x2, y2])
		//			continue;

		//		//Base case
		//		if (x == x2 && y2 == y)
		//			return true;

		//		PathGrid[x2, y2] = true;
		//		int distance = currentDistance + i * xSign + y * ySign;
		//		if (distance > maxDistance)
		//			continue;

		//		if (!countsAsPath(x2, y2))
		//			continue;

		//		if (FindPath(x2, y2, distance, directionID)) {
		//			if (resultPath == null) {
		//				resultPath = $"({x2}, {y2})";
		//			}
		//			else {
		//				resultPath += $", ({x2}, {y2})";
		//			}

		//			return true;
		//		}
		//		//}
		//	}

		//	$"x: {x}, y: {y} return false".Log();
		//	return false;
		//}
		private static void GetDirection(int x0, int y0, int directionID, out int x, out int y) {
			int i = directionID % 2;
			int j = 1 - i;
			if (directionID > 1) {
				i *= -1;
				j *= -1;
			}

			x = x0 + i;
			y = y0 + j;
		}
		private static void GetPreviousDirection(int x0, int y0, int fromDirection, int previousFrom, out int x, out int y) {
			//0: 0, 1
			//1: 1, 0
			//2: 0, -1
			//3: -1, 0
			x = x0;
			y = y0;
			bool fromPositiveY = fromDirection == 0 || previousFrom == 0;
			bool fromPositiveX = fromDirection == 1 || previousFrom == 1;
			if (fromPositiveY) {
				y--;
			}
			else {
				y++;
			}

			if (fromPositiveX) {
				x--;
			}
			else {
				x++;
			}
		}
		private static bool FindPath2(int x, int y, int currentDistance, int fromDirection = -1, int previousFrom = -1) {
			$"x: {x}, y: {y}, rX: {x + xStart}, rY: {y + yStart}, currentDistance: {currentDistance}, from: {fromDirection}, prevFrom: {previousFrom}".LogSimple();
			int opposite = fromDirection >= 0 ? (fromDirection + 2) % 4 : -1;
			int previousOpposite = previousFrom >= 0 ? (previousFrom + 2) % 4 : -1;
			for (int directionID = 0; directionID < 4; directionID++) {
				int i = directionID % 2;
				int j = 1 - i;
				if (directionID > 1) {
					i *= -1;
					j *= -1;
				}

				//$"i: {i}, j: {j}, directionID: {directionID}, opposite: {opposite}".Log();
				if (opposite == directionID)
					continue;

				//$"i: {i}, j: {j} 2".Log();
				int x2 = x + i;
				//$"x2: {x2}, xMin: {xMin}, xMax: {xMax}".Log();
				if (x2 < xMin || x2 > xMax)
					continue;
				//$"i: {i}, j: {j} 3".Log();
				int y2 = y + j;
				if (y2 < yMin || y2 > yMax)
					continue;

				//$"x2: {x2}, y2: {y2} Before PathGrid[]".Log();
				int distance = currentDistance + 1;
				if (PathGrid[x2, y2] <= distance)
					continue;

				if (distance > maxDistance)
					continue;

				//Base case
				int realX = x2 + xStart;
				int realY = y2 + yStart;
				if (countsAsTarget(realX, realY))
					return true;

				PathGrid[x2, y2] = distance;

				if (!countsAsPath(realX, realY))
					continue;

				if (previousOpposite == directionID) {
					GetPreviousDirection(x + xStart, y + yStart, fromDirection, previousFrom, out int previousX, out int previousY);
					if (countsAsPath(previousX, previousY))
						continue;
				}

				if (FindPath2(x2, y2, distance, directionID, fromDirection == directionID ? previousFrom : fromDirection)) {
					if (resultPath == null) {
						resultPath = $"({x2}, {y2})";
					}
					else {
						resultPath += $", ({x2}, {y2})";
					}

					return true;
				}
			}

			//$"x: {x}, y: {y} return false".Log();
			"".LogSimple();
			return false;
		}
	}

	public static class DirectionID {
		public const int None = 0;
		public const int Left = 1;
		public const int Right = 2;
		public const int Up = 3;
		public const int Down = 4;
		public const int Count = 5;

		public static void ApplyDirection(ref int x, ref int y, int direction) {
			switch (direction) {
				case Up:
					y--;
					break;
				case Down:
					y++;
					break;
				case Left:
					x--;
					break;
				case Right:
					x++;
					break;
			}
		}

		public static int GetOppositeDirection(int direction) {
			switch (direction) {
				case Up:
					return Down;
				case Down:
					return Up;
				case Left:
					return Right;
				case Right:
					return Left;
				default:
					return None;
			}
		}

		public static (int, int)[] GetDirections(int x, int y) {
			(int, int)[] directions = new (int, int)[5];
			directions[None] = (x, y);
			directions[Left] = (x - 1, y);
			directions[Right] = (x + 1, y);
			directions[Up] = (x, y - 1);
			directions[Down] = (x, y + 1);

			return directions;
		}
		/// <summary>
		/// Not Safe.  Need to check if out of world before using.
		/// </summary>
		public static Tile2[] GetTiles(int x, int y) {
			Tile2[] tiles = new Tile2[5];
			tiles[None] = MainT.tile[x, y];
			tiles[Left] = MainT.tile[x - 1, y];
			tiles[Right] = MainT.tile[x + 1, y];
			tiles[Up] = MainT.tile[x, y - 1];
			tiles[Down] = MainT.tile[x, y + 1];

			return tiles;
		}
	}

	public static class MainT {
		public static int tileMaxX;
		public static int tileMaxY;
		public static Tile2[,] tile {
			get {
				if (_tile == null)
					SetupTile();

				return _tile;
			}
		}
		public static void WriteAll(this Tile2[,] tiles) {
			string s = "";
			int xLen = tileMaxX;
			int yLen = tileMaxY;
			for (int y = 0; y < yLen; y++) {
				bool first = true;
				for (int x = 0; x < xLen; x++) {
					if (first) {
						first = false;
					}
					else {
						s += ", ";
					}
					s += $"{tiles[x, y].TileType}";
				}
				s += "\n";
			}
			s += "\n";
			s.LogSimple();
		}
		private static Tile2[,] _tile = null;
		private static void SetupTile() {
			_tile = new Tile2[10, 10]
			{
		{ new Tile2(1), new Tile2(2), new Tile2(2), new Tile2(2), new Tile2(1), new Tile2(0), new Tile2(2), new Tile2(0), new Tile2(1), new Tile2(2) },
		{ new Tile2(1), new Tile2(1), new Tile2(2), new Tile2(0), new Tile2(2), new Tile2(1), new Tile2(0), new Tile2(2), new Tile2(0), new Tile2(1) },
		{ new Tile2(1), new Tile2(0), new Tile2(1), new Tile2(2), new Tile2(0), new Tile2(2), new Tile2(1), new Tile2(0), new Tile2(2), new Tile2(0) },
		{ new Tile2(1), new Tile2(2), new Tile2(1), new Tile2(1), new Tile2(2), new Tile2(1), new Tile2(2), new Tile2(1), new Tile2(0), new Tile2(2) },
		{ new Tile2(1), new Tile2(0), new Tile2(2), new Tile2(1), new Tile2(1), new Tile2(1), new Tile2(0), new Tile2(2), new Tile2(1), new Tile2(0) },
		{ new Tile2(1), new Tile2(1), new Tile2(1), new Tile2(1), new Tile2(0), new Tile2(1), new Tile2(2), new Tile2(0), new Tile2(2), new Tile2(1) },
		{ new Tile2(1), new Tile2(2), new Tile2(1), new Tile2(0), new Tile2(2), new Tile2(1), new Tile2(3), new Tile2(2), new Tile2(0), new Tile2(2) },
		{ new Tile2(1), new Tile2(1), new Tile2(1), new Tile2(1), new Tile2(1), new Tile2(2), new Tile2(0), new Tile2(1), new Tile2(2), new Tile2(0) },
		{ new Tile2(1), new Tile2(1), new Tile2(2), new Tile2(0), new Tile2(2), new Tile2(0), new Tile2(2), new Tile2(0), new Tile2(2), new Tile2(0) },
		{ new Tile2(1), new Tile2(1), new Tile2(0), new Tile2(1), new Tile2(0), new Tile2(1), new Tile2(0), new Tile2(1), new Tile2(0), new Tile2(1) }
			};

			tileMaxX = _tile.GetLength(0);
			tileMaxY = _tile.GetLength(1);
		}
	}
	public static class UtilityMethods {
		public static void LogSimple(this string s) => Console.WriteLine(s);
		public static void LogError(this string s) {
			ConsoleColor originalColor = Console.ForegroundColor;
			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine("Error: " + s);
			Console.ForegroundColor = originalColor;
		}
		public static void FillArray<T>(ref T[,] arr, int xLen, int yLen, T value) {
			arr = new T[xLen, yLen];
			for (int y = 0; y < yLen; y++) {
				for (int x = 0; x < xLen; x++) {
					arr[x, y] = value;
				}
			}
		}
	}
}
