//using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TerrariaPathFinderTesing;
using TerrariaPathFinderTesing.TileData.TA_TileData;
using TerrariaPathFinderTesting;

class Program {
	public static void Main(string[] args) {
		//TerrariaPathFinderTesting.Test();

		//LargeNumberTesting.Test();

		TripleTesting.Test();

		//ByteLog2Testing();

		//WorldIO_Testing.TileDataAccessTesting();

		//TestReadWriteNumber();

		//TestUShortOverflow();

		//ElementsEnergyMath.Testing();
	}
	private static void TestUShortOverflow() {
		ushort id = 1;
		for (int i = 0; i < 5; i++) {
			for (ushort j = 0; j < ushort.MaxValue; j++) {
				id = (ushort)(id % ushort.MaxValue + 1);
				if (id == 0)
					$"id: {id}, j: {j}".Log();
			}
		}
	}
	private static void TestReadWriteNumber() {
		TestWriter writer = new();
		(uint num, int bits)[] nums = [
			(5, 7),
			(1, 1),
			(1, 1),
			(1, 2),
			(3, 8),
			(10, 4),
			(13, 8),
			(1, 1),
		];

		foreach ((uint num, int bits) n in nums) {
			writer.WriteNumber(n.num, n.bits);
		}

		$"writer: {writer}".Log();

		TestReader reader = new(writer.Value);
		foreach ((uint num, int bits) n in nums) {
			uint result = reader.ReadNumber(n.bits);
			$"({result}, {n.bits}),".Log();
		}

		$"reader: {reader}".Log();


	}
	private static void ByteLog2Testing() {
		int num = 100;
		Stopwatch stopwatch = new();
		Stopwatch stopwatch1 = new();
		Stopwatch stopwatch2 = new();
		Stopwatch stopwatch3 = new();

		stopwatch.Start();
		for (int i = 0; i < num; i++) {
			for (byte b = 0; b < byte.MaxValue; b++) {
				int r = b.BitsNeeded_Old();
			}
		}

		stopwatch.Stop();

		stopwatch1.Start();
		for (int i = 0; i < num; i++) {
			for (byte b = 0; b < byte.MaxValue; b++) {
				int r = b.BitsNeeded_1();
			}
		}
		
		stopwatch1.Stop();

		stopwatch2.Start();
		for (int i = 0; i < num; i++) {
			for (byte b = 0; b < byte.MaxValue; b++) {
				int r = b.BitsNeeded_2();
			}
		}

		stopwatch2.Stop();

		stopwatch3.Start();
		for (int i = 0; i < num; i++) {
			for (byte b = 0; b < byte.MaxValue; b++) {
				int r = b.BitsNeeded_3();
			}
		}

		stopwatch3.Stop();

		$"0; ticks: {stopwatch.ElapsedTicks}, ms: {stopwatch.ElapsedMilliseconds}".Log();
		$"1; ticks: {stopwatch1.ElapsedTicks}, ms: {stopwatch1.ElapsedMilliseconds}".Log();
		$"2; ticks: {stopwatch2.ElapsedTicks}, ms: {stopwatch2.ElapsedMilliseconds}".Log();
		$"3; ticks: {stopwatch3.ElapsedTicks}, ms: {stopwatch3.ElapsedMilliseconds}".Log();

		for (byte b = 0; b < byte.MaxValue; b++) {
			int r = b.BitsNeeded_Old();
			int r1 = b.BitsNeeded_1();
			int r2 = b.BitsNeeded_2();
			int r3 = b.BitsNeeded_3();
			$"{b}; r: {r}, r1: {r1}, r2: {r2}, r3: {r3}{(r != r1 || r != r2 || r != r3 ? " NOT EQUAL!!!!" : "")}".Log();
		}
	}

}

public static class MethodsBeingTested {
	public static int BitsNeeded_Old(this byte b) => byte.Log2(b) + 1;// (byte)(b > 1 ? (byte.Log2(b) + 1) : b == 1 ? 1 : 0);
	public static int BitsNeeded_1(this byte b) {
		if (b == 0)
			return 1;

		int result = 0;
		if ((b & 0b11110000) != 0) { result += 4; b >>= 4; }
		if ((b & 0b00001100) != 0) { result += 2; b >>= 2; }
		if ((b & 0b00000010) != 0) { result += 1; }

		return result + 1;
	}
	public static int BitsNeeded_2(this byte b) => (b & 0b10000000) > 0 ? 8 : (b & 0b01000000) > 0 ? 7 : (b & 0b00100000) > 0 ? 6 : (b & 0b00010000) > 0 ? 5 : (b & 0b00001000) > 0 ? 4 : (b & 0b00000100) > 0 ? 3 : (b & 0b00000010) > 0 ? 2 : 1;
	public static int BitsNeeded_3(this byte b) {
		byte c = 0b10000000;
		for (byte i = 8; i > 0; i--) {
			if ((b & c) != 0)
				return i;

			c >>= 1;
		}

		return 1;
	} 
}

