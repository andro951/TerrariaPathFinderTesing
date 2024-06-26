﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using TerrariaPathFinderTesing.TileData.TA_TileData;

namespace TerrariaPathFinderTesting {
	
	public struct BigNumber {
		private const long SquareRootOfLongMaxValue = 3037000499;//Square root of long.MaxValue rounded down.  Prevents overflow when multiplying two significands.
		private long significand;
		public long Significand {
			get => significand;
			private set {
				significand = value;
				OnSetSignificand();
			}
		}
		public readonly int Exponent => exponent;
		private int exponent;
		public readonly int SignificandExponent => significandExponent;
		private int significandExponent = 0;
		public readonly int CombinedExponent => exponent + significandExponent;
		public BigNumber (long significand, int exponent) {
			this.exponent = exponent;
			this.Significand = significand;
		}
		public BigNumber(float num) {
			if (float.IsNaN(num) || float.IsInfinity(num) || num == 0f) {
				this.exponent = 0;
				Significand = 0;
				return;
			}

			int sign = num < 0 ? -1 : 1;
			int exponent = (int)MathF.Floor(MathF.Log10(MathF.Abs(num)));
			float significand = num / MathF.Pow(10, exponent);
			int bits = BitConverter.SingleToInt32Bits(significand);
			long significandDivisor = 0b10000000_00000000_00000000;//2^23
			
			long realSignificand = (long)((bits & 0x7FFFFF) | 0x800000);

			PadSignificand(ref realSignificand, ref exponent);
			this.exponent = exponent;
			Significand = realSignificand / significandDivisor * sign;
		}
		public BigNumber(double num) {
			if (double.IsNaN(num) || double.IsInfinity(num) || num == 0d) {
				this.exponent = 0;
				Significand = 0;
				return;
			}

			int sign = num < 0 ? -1 : 1;
			int exponent = (int)Math.Floor(Math.Log10(Math.Abs(num)));
			double significand = num / Math.Pow(10, exponent);
			long significandDivisor = 0b10000_00000000_00000000_00000000_00000000_00000000_00000000;//2^53
			long bits = BitConverter.DoubleToInt64Bits(significand);
			long realSignificand = (bits & 0xFFFFFFFFFFFFF) | 0x10000000000000;
			PadSignificand(ref realSignificand, ref exponent);
			this.exponent = exponent;
			Significand = realSignificand / significandDivisor * sign;
		}
		public static BigNumber operator +(BigNumber left, BigNumber right) {
			if (left.exponent != right.exponent) {
				if (left.CombinedExponent > right.CombinedExponent) {
					right.SetExponent(left.exponent);
				}
				else {
					left.SetExponent(right.exponent);
				}
			}

			return new BigNumber(left.Significand + right.Significand, left.exponent);
		}
		public static BigNumber operator -(BigNumber left, BigNumber right) => left + new BigNumber(-right.Significand, right.exponent);
		public static BigNumber operator *(BigNumber left, BigNumber right) {
			if (left.Significand > SquareRootOfLongMaxValue || right.Significand > SquareRootOfLongMaxValue) {
				long max = long.MaxValue / left.Significand;
				if (right.Significand > max) {
					int exp = (int)right.Significand.CeilingDivide(max);
					Reduce(ref left, ref right, exp);
				}
			}

			long mult = left.Significand * right.Significand;

			return new BigNumber(mult, left.exponent + right.exponent);
		}
		public static BigNumber operator /(BigNumber left, BigNumber right) {
			left.PadSignificand();
			long div = left.Significand / right.Significand;

			return new BigNumber(div, left.exponent - right.exponent);
		}
		private static void PadSignificand(ref long significand, ref int exponent) {
			long div = long.MaxValue / significand;
			if (div < 10)
				return;

			int exponentSpace = (int)Math.Log10(div);
			if (exponentSpace <= 0)
				return;

			significand *= (long)Math.Pow(10, exponentSpace);
			exponent -= exponentSpace;
		}
		private void PadSignificand() => PadSignificand(ref significand, ref exponent);

		/// <summary>
		/// Use carefully.  Accuracy will be lost by using this function.
		/// Designed to be used to raise an exponent of the lower value before addition/subtraction
		/// </summary>
		private void SetExponent(int newExponent) {
			int diff = newExponent - exponent;
			if (diff == 0)
				return;

			if (diff > 0) {
				long div = (long)Math.Pow(10, diff);
				significand /= div;
			}
			else {
				long mult = (long)Math.Pow(10, -diff);
				significand *= mult;
			}

			exponent = newExponent;
		}
		private void OnSetSignificand() {
			NormalizeSignificand();
			CalculateSignificandExponent();
		}
		private static int CalculateSignificandExponent(long significand) => significand != 0 ? (int)Math.Floor(Math.Log10(Math.Abs(significand))) : 0;
		private void CalculateSignificandExponent() {
			significandExponent = CalculateSignificandExponent(Significand);
		}
		private void NormalizeSignificand() {
			while (Significand > 0 && Significand % 10 == 0) {
				significand /= 10;
				exponent++;
			}
		}
		private static void Reduce(ref BigNumber left, ref BigNumber right, int exp) {//TODO: Check what happens when exp is negative
			int leftRed;
			int rightRed;
			//long expAbs = Math.Abs(exp);
			if (left.Significand > right.Significand) {
				long div = left.Significand.CeilingDivide(right.Significand);
				int expDiff = div.CeilingLog10();
				rightRed = Math.Max(0, (exp - expDiff) / 2);
				leftRed = exp - rightRed;
			}
			else {
				long div = right.Significand.CeilingDivide(left.Significand);
				int expDiff = div.CeilingLog10();
				leftRed = Math.Max(0, (exp - expDiff) / 2);
				rightRed = exp - leftRed;
			}

			if (leftRed != 0) {
				left.Significand /= (long)Math.Pow(10, leftRed);
				left.exponent += leftRed;
			}

			if (rightRed != 0) {
				right.Significand /= (long)Math.Pow(10, rightRed);
				right.exponent += rightRed;
			}
		}

		public override string ToString() => S(2);

		private const char after9 = (char)('9' + 1);
		public string S(int decimals = 4, bool scientific = true) {
			string s = Significand.ToString();
			int frontLen = Significand < 0 ? 2 : 1;
			int exp = CombinedExponent;

			int frontZeros = 0;
			//Check if rounding is needed
			while (true) {
				//Front Zeros are for Big Numbers that have a negative exponent
				if (!scientific && exp < 0 || frontZeros > 0) {
					if (-exp <= decimals) {
						frontZeros = -exp;
						//s = string.Concat(Enumerable.Repeat("0", -exp)) + s;
					}
				}

				//Try round to decimal place
				if (s.Length + frontZeros > frontLen + decimals) {
					int endIndex = frontLen + decimals - frontZeros;
					char end = s[endIndex];
					//Needs to round up
					if (end >= '5' && end <= '9') {
						s = IncChar(s, endIndex - 1, true);

						int firstNumIndex = frontLen - 1;
						for (int i = endIndex - 1; i >= firstNumIndex; i--) {
							char c = s[i];
							if (c != after9)
								break;

							s = SetChar(s, i, '0');
							if (i == firstNumIndex) {
								string f = s.Substring(0, i);
								string m = "1";
								string e = s.Substring(i);
								string temp = f + m + e;
								s = $"{s.Substring(0, i)}1{s.Substring(i)}";
								exp++;
							}
							else {
								s = IncChar(s, i - 1);
							}
						}

						//Truncate extra
						if (s.Length + frontZeros > frontLen + decimals)
							s = s.Substring(0, frontLen + decimals - frontZeros);
					}
					else {
						break;
					}
				}
				else {
					break;
				}
			}

			//Add 0s to front when less than 1.
			if (frontZeros > 0)
				s = string.Concat(Enumerable.Repeat("0", frontZeros)) + s;

			//Truncate extra
			if (s.Length > frontLen + decimals)
				s = s.Substring(0, frontLen + decimals);

			//int frontZeros = 0;
			//if (!scientific && exp < 0) {
			//	if (-exp <= decimals) {
			//		frontZeros = -exp;
			//		//s = string.Concat(Enumerable.Repeat("0", -exp)) + s;
			//	}
			//}

			////Try round to decimal place
			//if (s.Length + frontZeros > frontLen + decimals) {
			//	int endIndex = frontLen + decimals - frontZeros;
			//	char end = s[endIndex];
			//	//Needs to round up
			//	if (end >= '5' && end <= '9') {
			//		s = IncChar(s, endIndex - 1, true);

			//		int firstNumIndex = frontLen - 1;
			//		for (int i = endIndex - 1; i >= firstNumIndex; i--) {
			//			char c = s[i];
			//			if (c != after9)
			//				break;

			//			s = SetChar(s, i, '0');
			//			if (i == firstNumIndex) {
			//				string f = s.Substring(0, i);
			//				string m = "1";
			//				string e = s.Substring(i);
			//				string temp = f + m + e;
			//				s = $"{s.Substring(0, i)}1{s.Substring(i)}";
			//				exp++;
			//			}
			//			else {
			//				s = IncChar(s, i - 1);
			//			}
			//		}
			//	}

			//	//Truncate extra
			//	if (s.Length > frontLen + decimals)
			//		s = s.Substring(0, frontLen + decimals);
			//}

			//Add 0s to end if less significant digits than needed.
			if (s.Length < frontLen + decimals)
				s = s + string.Concat(Enumerable.Repeat("0", frontLen + decimals - s.Length));

			if (scientific || exp >= 36 || exp < -decimals) {
				//Scientific
				string front = s.Substring(0, frontLen);// char n = s.Length > 0 ? s[0] : '0';
				//int dLength = Math.Min(s.Length - frontLen, decimals);
				//string d = s.Substring(frontLen, dLength) + string.Concat(Enumerable.Repeat("0", decimals - dLength));
				string d = s.Substring(frontLen);
				return $"{front}.{d}e{exp}";
			}
			else {
				//Abbreviations
				if (exp > 0) {
					int abrGroup = exp / 3;
					int groupExp = abrGroup * 3;
					int frontExp = exp - groupExp;
					int frontLen2 = Math.Min(frontLen + frontExp, s.Length);
					string front = s.Substring(0, frontLen2);
					string d = frontLen2 < frontLen + decimals ? "." + s.Substring(frontLen2) : "";
					string expAbr;
					switch (abrGroup) {
						case 0:
							expAbr = "";
							break;
						case 1:
							expAbr = "k";
							break;
						case 2:
							expAbr = "m";
							break;
						case 3:
							expAbr = "b";
							break;
						case 4:
							expAbr = "t";
							break;
						case 5:
							expAbr = "qa";
							break;
						case 6:
							expAbr = "qu";
							break;
						case 7:
							expAbr = "sx";
							break;
						case 8:
							expAbr = "sp";
							break;
						case 9:
							expAbr = "o";
							break;
						case 10:
							expAbr = "n";
							break;
						case 11:
							expAbr = "d";
							break;
						default:
							expAbr = "error";
							$"s: {s}, abrGroup: {abrGroup}, groupExp: {groupExp}, Significand: {Significand}, exponent: {exponent}, significandExponent: {significandExponent}".LogError();
							break;
					}

					return $"{front}{d}{expAbr}";
				}
				else {
					string front = s.Substring(0, frontLen);
					string d = s.Substring(frontLen);
					return $"{front}.{d}";
				}
			}
		}
		public static string IncChar(string s, int index, bool trunc = false) => $"{s.Substring(0, index)}{(char)(s[index] + 1)}{(trunc ? "" : s.Substring(index + 1))}";
		public static string SetChar(string s, int index, char c) => $"{s.Substring(0, index)}{c}{s.Substring(index + 1)}";
		//public static string IncChar(string s, int index, bool trunc = false) {
		//	string front = s.Substring(0, index);
		//	string middle = $"{(char)(s[index] + 1)}";
		//	string end = trunc ? "" : s.Substring(index + 1);

		//	return front + middle + end;
		//}
	}

	public static class MathLib {
		public static int CeilingDivide(this int num, int denom) => (num - 1) / denom + 1;
		public static long CeilingDivide(this long num, long denom) => (num - 1) / denom + 1;
		public static int CeilingLog10(this long num) {
			if (num <= 0)
				return 0;

			return (int)Math.Ceiling(Math.Log10(num));
		}
	}
}
