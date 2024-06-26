﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TerrariaPathFinderTesting {
	public static class LargeNumberTesting {
		private static bool logAllInfo => false;
		public static void Test() {
			long sqrt = (long)Math.Sqrt(long.MaxValue);//3037000499, 1011_0101_0000_0100_1111_0011_0011_0011
			long max = 0b0111_1111_1111_1111_1111_1111_1111_1111_1111_1111_1111_1111_1111_1111_1111_1111;
			long max2 = 0x7FFFFFFFFFFFFFFF;
			$"max: {max}, max2: {max2}, long.MaxValue: {long.MaxValue}".LogSimple();
			long sqr = sqrt * sqrt;
			long min = long.MaxValue - sqr;
			long div = min / long.MaxValue;
			$"sqrt: {sqrt}, sqr: {sqr}, min: {min}, div: {div}".LogSimple();
			$"sqrt in binary: {Convert.ToString(sqrt, 2)}".LogSimple();

			long SquareRootOfLong = 3037000499;
			long test = (SquareRootOfLong + 1) * (SquareRootOfLong + 1);
			$"test: {test}".LogSimple();

			//BigNumber left = new(684127, 100);
			//BigNumber right = new(25, 1);
			//BigNumber mult = left * right;
			//$"left: {left}, right: {right}, mult: {mult}".LogSimple();
			//BigNumber add = left + right;
			//$"left: {left}, right: {right}, add: {add}".LogSimple();

			//BigNumber divide = left / right;
			//$"left: {left}, right: {right}, divide: {divide}".LogSimple();
			//BigNumber subtract = left - right;
			//$"left: {left}, right: {right}, subtract: {subtract}".LogSimple();

			//BigNumber divide2 = right / left;
			//$"left: {right}, right: {left}, divide2: {divide2}".LogSimple();
			//BigNumber subtract2 = right - left;
			//$"left: {right}, right: {left}, subtract2: {subtract2}".LogSimple();

			BigNumber left2 = new(684127, 100);
			BigNumber right2 = new(25, 103);

			BigNumber mult2 = left2 * right2;
			$"left2: {left2}, right2: {right2}, mult2: {mult2}".LogSimple();
			BigNumber add2 = left2 + right2;
			$"left2: {left2}, right2: {right2}, add2: {add2}".LogSimple();
			BigNumber div2 = left2 / right2;
			$"left2: {left2}, right2: {right2}, div2: {div2}".LogSimple();
			BigNumber sub2 = left2 - right2;
			$"left2: {left2}, right2: {right2}, sub2: {sub2}".LogSimple();

			BigNumber mul3 = right2 * left2;
			$"left2: {right2}, right2: {left2}, mul3: {mul3}".LogSimple();
			BigNumber add3 = right2 + left2;
			$"left2: {right2}, right2: {left2}, add3: {add3}".LogSimple();
			BigNumber div3 = right2 / left2;
			$"left2: {right2}, right2: {left2}, div3: {div3}".LogSimple();
			BigNumber sub3 = right2 - left2;
			$"left2: {right2}, right2: {left2}, sub3: {sub3}".LogSimple();

			BigNumber left3 = new(-153231, -2);
			BigNumber right3 = new(2648, 0);

			BigNumber mult4 = left3 * right3;
			$"left3: {left3}, right3: {right3}, mult4: {mult4}".LogSimple();
			BigNumber add4 = left3 + right3;
			$"left3: {left3}, right3: {right3}, add4: {add4}".LogSimple();
			BigNumber div4 = left3 / right3;
			$"left3: {left3}, right3: {right3}, div4: {div4}".LogSimple();
			BigNumber sub4 = left3 - right3;
			$"left3: {left3}, right3: {right3}, sub4: {sub4}".LogSimple();
			BigNumber mul5 = right3 * left3;
			$"left3: {right3}, right3: {left3}, mul5: {mul5}".LogSimple();
			BigNumber add5 = right3 + left3;
			$"left3: {right3}, right3: {left3}, add5: {add5}".LogSimple();
			BigNumber div5 = right3 / left3;
			$"left3: {right3}, right3: {left3}, div5: {div5}".LogSimple();
			BigNumber sub5 = right3 - left3;
			$"left3: {right3}, right3: {left3}, sub5: {sub5}".LogSimple();

			int testInt = -3;
			int testInt2 = testInt >> 1;
			$"testInd: {testInt} ({Convert.ToString(testInt, 2)}), testInt >> 1 = {testInt2} ({Convert.ToString(testInt2, 2)})".LogSimple();

			float f3 = 11f;
			BigNumber floatNumber3 = new(f3);
			$"floatNumber3: {f3} => {floatNumber3}".LogSimple();

			float f4 = 12f;
			BigNumber floatNumber4 = new(f4);
			$"floatNumber4: {f4} => {floatNumber4}".LogSimple();

			float f = -1532.31f;
			BigNumber floatNumber = new(f);
			$"floatNumber: {f} => {floatNumber}".LogSimple();

			float f2 = 161345683213540000000000000000000000f;
			BigNumber floatNumber2 = new(f2);
			$"floatNumber2: {f2} => {floatNumber2}".LogSimple();

			double d = -1532.31;
			BigNumber doubleNumber = new(d);
			$"doubleNumber: {d} => {doubleNumber}".LogSimple();

			double d2 = 161345683213540000000000000000000000.0;
			BigNumber bigNumber2 = new(d2);
			$"bigNumber2: {d2} => {bigNumber2}".LogSimple();

			double d3 = 16134568321354e290;
			BigNumber bigNumber3 = new(d3);
			$"bigNumber3: {d3} => {bigNumber3}".LogSimple();

			Test_CalculateSignificandExponent();
		}

		private struct TestData_CalculateSignificandExponent {
			public long Significand;
			public int Exponent;
			public int ExpectedExponent;
			public int ExpectedSignificandExponent;
			public string ExpectedToStringScientific2;
			public string ExpectedToStringScientific3;
			public string ExpectedToString2;
			public string ExpectedToString3;
			public TestData_CalculateSignificandExponent(long significand, int exponent, int expectedSignificandExponent, string expectedToStringScientific2, string expectedToStringScienfitic3, string expectedToString2 = "", string expectedToString3 = "", int expectedExponent = int.MinValue) {
				Significand = significand;
				Exponent = exponent;
				ExpectedSignificandExponent = expectedSignificandExponent;
				ExpectedToStringScientific2 = expectedToStringScientific2;
				ExpectedToStringScientific3 = expectedToStringScienfitic3;
				ExpectedExponent = expectedExponent == int.MinValue ? Exponent : expectedExponent;
				ExpectedToString2 = expectedToString2 == "" ? ExpectedToStringScientific2 : expectedToString2;
				ExpectedToString3 = expectedToString3 == "" ? ExpectedToStringScientific3 : expectedToString3;
			}
		}
		private static List<TestData_CalculateSignificandExponent> testData_CalculateSignificandExponents = new() {
			new(684127, 100, 5, "6.84e105", "6.841e105"),
			new(10, 0, 1, "1.00e1", "1.000e1", "10.0", "10.00"),
			new(1, 1000, 0, "1.00e1000", "1.000e1000"),
			new(0, 0, 0, "0.00e0", "0.000e0", "0.00", "0.000"),
			//new(0, 100, 0, 0),//Not correct.  The exponent should be maintained so that the math can work as expected when the significand is 0
			new(-1023, 100, 3, "-1.02e103", "-1.023e103"),
			new(-1, 2, 0, "-1.00e2", "-1.000e2", "-100", "-100.0"),
			new(521, -2, 2, "5.21e0", "5.210e0", "5.21", "5.210"),
			new(5217, -3, 3, "5.22e0", "5.217e0", "5.22", "5.217"),
			new(5299, 4, 3, "5.30e7", "5.299e7", "53.0m", "52.99m"),
			new(9999, 2, 3, "1.00e6", "9.999e5", "1.00m", "999.9k"),
			new(599999, 30, 5, "6.00e35", "6.000e35", "600d", "600.0d"),
			new(999999, 30, 5, "1.00e36", "1.000e36"),
			new(1, -1, 0, "1.00e-1", "1.000e-1", "0.10", "0.100"),
			new(1, -2, 0, "1.00e-2", "1.000e-2", "0.01", "0.010"),
			new(1, -3, 0, "1.00e-3", "1.000e-3", expectedToString3: "0.001"),
			new(9999, -7, 3, "1.00e-3", "9.999e-4"/*, expectedToString3: "0.001"*/),//0.0009999
			new(9999, -6, 3, "1.00e-2", "9.999e-3", "0.01", "0.010"),
			new(9999, -5, 3, "1.00e-1", "9.999e-2", "0.10", "0.100"),
			new(9999, -4, 3, "1.00e0", "9.999e-1", "1.00", "1.000"),
			new(9999, -3, 3, "1.00e1", "9.999e0", "10.0", "9.999"),
			new(9999, -2, 3, "1.00e2", "9.999e1", "100", "99.99"),
			new(9999, -1, 3, "1.00e3", "9.999e2", "1.00k", "999.9"),
			new(9999, 0, 3, "1.00e4", "9.999e3", "10.0k", "9.999k"),
			new(9999, 1, 3, "1.00e5", "9.999e4", "100k", "99.99k"),
			new(9999, 2, 3, "1.00e6", "9.999e5", "1.00m", "999.9k"),
			new(9999, 3, 3, "1.00e7", "9.999e6", "10.0m", "9.999m"),
			new(9999, 4, 3, "1.00e8", "9.999e7", "100m", "99.99m"),
			new(9999, 5, 3, "1.00e9", "9.999e8", "1.00b", "999.9m"),
			new(9999, 6, 3, "1.00e10", "9.999e9", "10.0b", "9.999b"),
			new(9999, 7, 3, "1.00e11", "9.999e10", "100b", "99.99b"),
			new(9999, 8, 3, "1.00e12", "9.999e11", "1.00t", "999.9b"),
			new(9999, 9, 3, "1.00e13", "9.999e12", "10.0t", "9.999t"),
			new(9999, 10, 3, "1.00e14", "9.999e13", "100t", "99.99t"),
			new(9999, 31, 3, "1.00e35", "9.999e34", "100d", "99.99d"),
			new(9999, 32, 3, "1.00e36", "9.999e35", expectedToString3: "999.9d")
		};

		private static bool logAllInfo_CalculateSignificandExponent => true || logAllInfo;
		public static void Test_CalculateSignificandExponent() {
			$"Test_CalculateSignificandExponent".LogSimple();
			foreach (TestData_CalculateSignificandExponent testData in testData_CalculateSignificandExponents) {
				BigNumber testNum = new(testData.Significand, testData.Exponent);

				if (logAllInfo_CalculateSignificandExponent)
					$"testNum.Significand: {testNum.Significand}, testData.Significand: {testData.Significand}, testNum: {testNum}".LogSimple();

				if (testNum.Significand != testData.Significand) {
					$"testNum.Significand != testData.Significand, testNum.Significand: {testNum.Significand}, testData.Significand: {testData.Significand}, testNum: {testNum}".LogError();
				}


				if (logAllInfo_CalculateSignificandExponent)
					$"testNum.Exponent: {testNum.Exponent}, testData.ExpectedExponent: {testData.ExpectedExponent}, testNum: {testNum}".LogSimple();

				if (testNum.Exponent != testData.ExpectedExponent) {
					$"testNum.Exponent != testData.ExpectedExponent, testNum.Exponent: {testNum.Exponent}, testData.ExpectedExponent: {testData.ExpectedExponent}, testNum: {testNum}".LogError();
				}


				if (logAllInfo_CalculateSignificandExponent)
					$"testNum.SignificandExponent: {testNum.SignificandExponent}, testData.ExpectedSignificandExponent: {testData.ExpectedSignificandExponent}, testNum: {testNum}".LogSimple();

				if (testNum.SignificandExponent != testData.ExpectedSignificandExponent) {
					$"testNum.SignificandExponent != testData.ExpectedSignificandExponent, testNum.SignificandExponent: {testNum.SignificandExponent}, testData.ExpectedSignificandExponent: {testData.ExpectedSignificandExponent}, testNum: {testNum}".LogError();
				}


				if (logAllInfo_CalculateSignificandExponent)
					$"testNum.ToString(): {testNum.ToString()}, testData.ExpectedToStringScientific2: {testData.ExpectedToStringScientific2}".LogSimple();

				if (testNum.ToString() != testData.ExpectedToStringScientific2) {
					$"testNum.ToString() != testData.ExpectedToStringScientific2, testNum.ToString(): {testNum.ToString()}, testData.ExpectedToStringScientific2: {testData.ExpectedToStringScientific2}".LogError();
				}


				int d = 3;
				if (logAllInfo_CalculateSignificandExponent)
					$"testNum.S({d}): {testNum.S(d)}, testData.ExpectedToStringScientific3: {testData.ExpectedToStringScientific3}".LogSimple();

				if (testNum.S(d) != testData.ExpectedToStringScientific3) {
					$"testNum.S({d}) != testData.ExpectedToStringScientific3, testNum.S({d}): {testNum.S(d)}, testData.ExpectedToStringScientific3: {testData.ExpectedToStringScientific3}".LogError();
				}


				d = 2;
				if (logAllInfo_CalculateSignificandExponent)
					$"testNum.S({d}, false): {testNum.S(d, false)}, testData.ExpectedToString2: {testData.ExpectedToString2}".LogSimple();

				if (testNum.S(d, false) != testData.ExpectedToString2) {
					$"testNum.S({d}, false) != testData.ExpectedToString2, testNum.S({d}, false): {testNum.S(d, false)}, testData.ExpectedToString2: {testData.ExpectedToString2}".LogError();
				}

				d = 3;
				if (logAllInfo_CalculateSignificandExponent)
					$"testNum.S({d}, false): {testNum.S(d, false)}, testData.ExpectedToString3: {testData.ExpectedToString3}".LogSimple();

				if (testNum.S(d, false) != testData.ExpectedToString3) {
					$"testNum.S({d}, false) != testData.ExpectedToString3, testNum.S({d}, false): {testNum.S(d, false)}, testData.ExpectedToString3: {testData.ExpectedToString3}".LogError();
				}


				if (logAllInfo_CalculateSignificandExponent)
					"".LogSimple();//Space
			}
		}
	}
}
