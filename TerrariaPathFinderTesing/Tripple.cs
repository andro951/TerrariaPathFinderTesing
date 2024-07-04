using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using TerrariaPathFinderTesting;

namespace TerrariaPathFinderTesing {
	public struct Triple {
		public int Exponent {
			get => exponent;
			private set {
				exponent = value;
			}
		}
		private int exponent;
		public long Significand {
			get => significand;
			private set {
				if (significand == value)
					return;

				significand = value;
				OnSetSignificand();
			}
		}
		private long significand;
		public int SignificandExponent => significandExponent;
		private int significandExponent;
		public int CombinedExponent => Exponent + significandExponent;
		public static readonly Triple Zero = new(0, 0);
		public static readonly Triple One = new(1, 0);
		public Triple(long significand, int exponent) {
			this.exponent = exponent;
			this.significand = 0;
			significandExponent = 0;
			Significand = significand;
		}
		private Triple(long significand, int exponent, int significandExponent) {
			this.exponent = exponent;
			this.significand = significand;
			this.significandExponent = significandExponent;
		}
		public Triple(float num, int exponent = 0) {
			this.significand = 0;
			significandExponent = 0;
			if (float.IsNaN(num) || float.IsInfinity(num) || num == 0f) {
				this.exponent = 0;
				significandExponent = 0;
				Significand = 0;
				return;
			}

			int sign = num < 0 ? -1 : 1;
			int bits = BitConverter.SingleToInt32Bits(num);
			int binaryExp = ((bits >> 23) & 0xFF) - 127;
			int realSignificand = (bits & 0x7FFFFF) | 0x800000;
			this.exponent = binaryExp + exponent - 23;
			Significand = realSignificand * sign;
		}
		public Triple(double num, int exponent = 0) {
			this.significand = 0;
			significandExponent = 0;
			if (double.IsNaN(num) || double.IsInfinity(num) || num == 0d) {
				this.exponent = 0;
				Significand = 0;
				return;
			}

			int sign = num < 0 ? -1 : 1;
			long bits = BitConverter.DoubleToInt64Bits(num);
			int binaryExp = (int)((bits >> 52) & 0x7FF) - 1023;
			long realSignificand = (bits & 0xFFFFFFFFFFFFF) | 0x10000000000000;
			this.exponent = binaryExp + exponent - 52;
			Significand = realSignificand * sign;
		}
		private void OnSetSignificand() {
			NormalizeSignificand();
			CalculateSignificandExponent();
		}
		private void NormalizeSignificand() {
			if (significand == 0) {
				Exponent = 0;
				return;
			}

			int trailingZeros = BitOperations.TrailingZeroCount(Significand);
			significand >>= trailingZeros;
			exponent += trailingZeros;
		}
		private void CalculateSignificandExponent() {
			if (significand == 0) {
				significandExponent = 0;
				return;
			}

			significandExponent = BitOperations.Log2((uint)Math.Abs(Significand));
		}
		public bool IsPositive => significand > 0;
		public bool IsZero => significand == 0;
		public bool IsNegative => significand < 0;
		private Triple PadSignificand(bool forAddition = false) {
			if (significand == 0)
				return this;

			int newSignificandExponent = forAddition ? 61 : 62;
			int exp = newSignificandExponent - significandExponent;

			return new Triple(significand << exp, exponent - exp, newSignificandExponent);
		}
		private Triple SetExponent(int newExponent) {
			if (exponent == newExponent)
				return new Triple(significand, exponent, significandExponent);

			int expDiff = newExponent - exponent;
			if (expDiff > 0) {
				return new Triple(significand >> expDiff, newExponent, significandExponent + expDiff);
			}
			else {
				return new Triple(significand << -expDiff, newExponent, significandExponent - expDiff);
			}
		}
		public static Triple operator -(Triple triple) => new Triple(-triple.Significand, triple.Exponent, triple.SignificandExponent);
		public static bool operator <(Triple left, Triple right) {
			if (!left.IsPositive) {
				//left is negative or zero

				if (right.IsPositive)
					return true;

				//right is negative or zero

				if (left.IsZero)
					return right.IsNegative;

				//left is negative

				if (right.IsZero)
					return true;

				//right is negative

				if (left.CombinedExponent > right.CombinedExponent)
					return true;
			}
			else {
				//left is positive

				if (!right.IsPositive)
					return false;

				//right is positive

				if (left.CombinedExponent < right.CombinedExponent)
					return true;
			}

			if (left.Exponent != right.Exponent) {
				if (left.CombinedExponent > right.CombinedExponent) {
					Triple paddedLeft = left.PadSignificand();
					Triple rightNewExponent = right.SetExponent(paddedLeft.Exponent);
					return paddedLeft.Significand < rightNewExponent.Significand;
				}
				else if (left.CombinedExponent < right.CombinedExponent) {
					Triple paddedRight = right.PadSignificand();
					Triple leftNewExponent = left.SetExponent(paddedRight.Exponent);
					return leftNewExponent.Significand < paddedRight.Significand;
				}
				else {
					Triple paddedLeft = left.PadSignificand();
					Triple paddedRight = right.PadSignificand();
					if (paddedLeft.Exponent != paddedRight.Exponent) {
						if (left.exponent > right.exponent) {
							Triple rightNewExponent = right.SetExponent(paddedLeft.Exponent);
							return paddedLeft.Significand < rightNewExponent.Significand;
						}
						else {
							Triple leftNewExponent = left.SetExponent(paddedRight.Exponent);
							return leftNewExponent.Significand < paddedRight.Significand;
						}
					}

					return paddedLeft.Significand < paddedRight.Significand;
				}
			}

			return left.Significand < right.Significand;
		}
		public static bool operator >(Triple left, Triple right) => left != right && right < left;
		public static bool operator <=(Triple left, Triple right) => left == right || left < right;
		public static bool operator >=(Triple left, Triple right) => !(left < right);
		public static bool operator ==(Triple left, Triple right) => left.Exponent == right.Exponent && left.Significand == right.Significand;
		public static bool operator !=(Triple left, Triple right) => !(left == right);
		public override bool Equals([NotNullWhen(true)] object? obj) => obj is Triple triple && this == triple;
		public override int GetHashCode() => HashCode.Combine(Significand, Exponent);
		public static Triple operator +(Triple left, Triple right) {
			if (left.Significand == 0)
				return right;

			if (right.Significand == 0)
				return left;

			if (left.Exponent != right.Exponent) {
				if (right.ABS() < left.ABS()) {
					Triple paddedLeft = left.PadSignificand(true);
					Triple rightNewExponent = right.SetExponent(paddedLeft.Exponent);
					return new(paddedLeft.Significand + rightNewExponent.Significand, paddedLeft.Exponent);
				}
				else {
					Triple paddedRight = right.PadSignificand(true);
					Triple leftNewExponent = left.SetExponent(paddedRight.Exponent);
					return new(leftNewExponent.Significand + paddedRight.Significand, leftNewExponent.Exponent);
				}
			}

			if (left.IsPositive) {
				if (right.IsPositive) {
					long max = long.MaxValue - left.Significand;
					if (right.Significand > max) {
						return new((left.Significand >> 1) + (right.Significand >> 1), left.Exponent + 1);
					}
				}
			}
			else if (right.IsNegative) {
				long min = long.MinValue - left.Significand;
				if (right.Significand < min) {
					return new((left.Significand >> 1) + (right.Significand >> 1), left.Exponent + 1);
				}
			}

			return new(left.Significand + right.Significand, left.Exponent);
		}
		public static Triple operator -(Triple left, Triple right) => left + -right;
		public static Triple operator *(Triple left, Triple right) {
			int totalSignificandExponent = left.significandExponent + right.significandExponent;
			if (totalSignificandExponent - 61 <= 0)//61 instead of 63 to round the significands up to the next power of 2
				return new(left.Significand * right.Significand, left.Exponent + right.Exponent);

			int halfExponent = totalSignificandExponent >> 1;
			if (right.Significand > left.Significand) {
				Triple leftNewExponent = left.SetExponent(halfExponent);
				Triple rightNewExponent = right.SetExponent(totalSignificandExponent - halfExponent);
				return new(leftNewExponent.Significand * rightNewExponent.Significand, leftNewExponent.Exponent + rightNewExponent.Exponent);
			}
			else {
				Triple rightNewExponent = right.SetExponent(halfExponent);
				Triple leftNewExponent = left.SetExponent(totalSignificandExponent - halfExponent);
				return new(leftNewExponent.Significand * rightNewExponent.Significand, leftNewExponent.Exponent + rightNewExponent.Exponent);
			}
		}
		public static Triple operator /(Triple left, Triple right) {
			if (right.IsZero)
				throw new DivideByZeroException($"left: {left.ToFullString()} / right: {right.ToFullString()}");

			if (left.IsZero)
				return Zero;

			Triple paddedLeft = left.PadSignificand();
			return new(paddedLeft.Significand / right.Significand, paddedLeft.Exponent - right.Exponent);
		}
		public static implicit operator Triple(double value) => new(value);
		public static implicit operator Triple(float value) => new(value);
		public static implicit operator Triple(int value) => new(value);
		public static implicit operator Triple(long value) => new(value);
		public static implicit operator Triple(uint value) => new(value);
		public double ToDouble() => (double)Significand * Math.Pow(2, Exponent);
		public float ToFloat() => (float)ToDouble();
		private static double Log2of10 = Math.Log2(10);
		private static int Base10ToBase2(int exponentBase10, out double fraction) {
			fraction = exponentBase10 * Log2of10;
			int exponentBase2 = (int)fraction;
			fraction -= exponentBase2;
			return exponentBase2;
		}
		public static Triple TriplePow10(double significand, int exponentBase10) {
			if (significand == 0)
				return Zero;

			if (exponentBase10 == 0)
				return new(significand);

			int base2Exponent = Base10ToBase2(exponentBase10, out double fraction);
			double base10FractionMult = Math.Pow(2d, fraction);
			significand *= base10FractionMult;
			return new(significand, base2Exponent);
		}
		public static Triple Min(Triple left, Triple right) => left < right ? left : right;
		public static Triple Max(Triple left, Triple right) => left > right ? left : right;
		public Triple ABS() => new(Math.Abs(Significand), Exponent, significandExponent);
		public void Write(BinaryWriter writer) {
			writer.Write(Significand);
			writer.Write(Exponent);
		}
		public static Triple Read(BinaryReader reader) => new(reader.ReadInt64(), reader.ReadInt32());
		public override string ToString() => S();
		public string ToFullString() => $"{S()} ({Significand} * 2^{Exponent})";
		public string S(int decimals = 2, bool scientific = true, bool removeTrailingZeros = true) {
			double base10SignificandExponentDouble = Math.Log10(Math.Abs(significand));
			double base10ExponentDouble = Exponent * Math.Log10(2);
			double base10CombinedExponent = base10SignificandExponentDouble + base10ExponentDouble;
			int base10CombinedExponentInt = (int)base10CombinedExponent;
			double base10CombinedExponentFraction = base10CombinedExponent - base10CombinedExponentInt;
			double base10Significand = Significand * Math.Pow(10, -(base10SignificandExponentDouble - base10CombinedExponentFraction));

			string s = base10Significand.ToString();
			int frontLen = base10Significand < 0 ? 2 : 1;
			int exp = base10CombinedExponentInt;

			int dotIndex = s.IndexOf('.');
			if (dotIndex >= 0) {
				int zerosToRemove = 0;
				for (int i = frontLen - 1; i < s.Length; i++) {
					if (s[i] != '0') {
						break;
					}

					zerosToRemove++;
				}

				exp -= zerosToRemove;

				if (s[frontLen - 1 + zerosToRemove] == '.') {
					zerosToRemove++;
					s = s.Substring(0, frontLen - 1) + s.Substring(frontLen - 1 + zerosToRemove);
				}
				else {
					int i = frontLen - 1 + zerosToRemove;
					s = s.Substring(0, frontLen - 1) + s.Substring(i, dotIndex - i) + s.Substring(dotIndex + 1);
					//s = s.Substring(zerosToRemove, dotIndex - zerosToRemove) + s.Substring(dotIndex + 1);
				}
			}

			int frontZeros = 0;
			//Check if rounding is needed
			while (true) {
				//Front Zeros are for Big Numbers that have a negative exponent
				if (exp < 0 && (exp >= -decimals || !scientific) || frontZeros > 0) {
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
								//string f = s.Substring(0, i);
								//string m = "1";
								//string e = s.Substring(i);
								//string temp = f + m + e;
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
				s = s.Substring(0, frontLen - 1) + string.Concat(Enumerable.Repeat("0", frontZeros)) + s.Substring(frontLen - 1);

			//Truncate extra
			if (s.Length > frontLen + decimals)
				s = s.Substring(0, frontLen + decimals);

			//Add 0s to end if less significant digits than needed.
			if (s.Length < frontLen + decimals)
				s = s + string.Concat(Enumerable.Repeat("0", frontLen + decimals - s.Length));

			if (scientific && exp >= 0 || exp >= 36 || exp < -decimals) {
				//Scientific
				string front = s.Substring(0, frontLen);// char n = s.Length > 0 ? s[0] : '0';
				//int dLength = Math.Min(s.Length - frontLen, decimals);
				//string d = s.Substring(frontLen, dLength) + string.Concat(Enumerable.Repeat("0", decimals - dLength));
				string d = s.Substring(frontLen);
				string result = $"{front}.{d}";
				return (removeTrailingZeros ? RemoveTrailingZeros(result) : result) + $"e{exp}";
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

					string result = $"{front}{d}";
					return (removeTrailingZeros ? RemoveTrailingZeros(result) : result) + expAbr;
				}
				else {
					string front = s.Substring(0, frontLen);
					string d = s.Substring(frontLen);
					string result = $"{front}.{d}";
					return removeTrailingZeros ? RemoveTrailingZeros(result) : result;
				}
			}
		}
		/*
		public string S(int decimals = 2, bool scientific = true, bool leaveOffExtraZeros = false) {//bool scientific = false, bool leaveOffExtraZeros = true) {
			if (Significand == 0) {
				if (leaveOffExtraZeros) {
					return "0";
				}
				else {
					return 0d.ToString($"F{decimals}");
				}
			}

			double base10SignificandExponentDouble = Math.Log10(Math.Abs(significand));
			double base10ExponentDouble = Exponent * Math.Log10(2);
			double base10CombinedExponent = base10SignificandExponentDouble + base10ExponentDouble;
			int base10CombinedExponentInt = (int)base10CombinedExponent;
			double base10CombinedExponentFraction = base10CombinedExponent - base10CombinedExponentInt;
			double base10Significand = Significand * Math.Pow(10, -(base10SignificandExponentDouble - base10CombinedExponentFraction));

			string s = base10Significand.ToString($"F{decimals}");
			int frontLen = base10Significand < 0d ? 2 : 1;
			int dotIndex = s.IndexOf('.');
			if (dotIndex >= 0) {
				int expectedRoundedIndex = 1 + frontLen;
				//If rounded up to 10, move the decimal
				if (dotIndex == expectedRoundedIndex) {
					base10CombinedExponentInt++;
				}

				s = s.Substring(0, dotIndex) + s.Substring(dotIndex + 1);
				//s.LogSimple();
			}

			int exp = base10CombinedExponentInt;

			int frontZeros = 0;
			if (!scientific && exp < 0 || frontZeros > 0) {
				if (-exp <= decimals) {
					frontZeros = -exp;
					//s = string.Concat(Enumerable.Repeat("0", -exp)) + s;
				}
			}

			//Add 0s to front when less than 1.
			if (frontZeros > 0)
				s = string.Concat(Enumerable.Repeat("0", frontZeros)) + s;

			//Truncate extra
			if (s.Length > frontLen + decimals)
				s = s.Substring(0, frontLen + decimals);

			//Add 0s to end if less significant digits than needed.
			if (s.Length < frontLen + decimals)
				s = s + string.Concat(Enumerable.Repeat("0", frontLen + decimals - s.Length));

			if (scientific || exp >= 36 || exp < -decimals) {
				string front = s.Substring(0, frontLen);
				string d = s.Substring(frontLen);
				return $"{front}.{d}e{exp}";
			}

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
						//$"s: {s}, abrGroup: {abrGroup}, groupExp: {groupExp}, Significand: {Significand}, exponent: {exponent}, significandExponent: {significandExponent}".LogError();
						break;
				}

				string result = $"{front}{d}{expAbr}";
				return leaveOffExtraZeros ? RemoveTrailingZeros(result) : result;
			}
			else {
				string front = s.Substring(0, frontLen);
				string d = s.Substring(frontLen);
				string result = $"{front}.{d}";
				return leaveOffExtraZeros ? RemoveTrailingZeros(result) : result;
			}
		}
		*/
		private const char after9 = (char)('9' + 1);
		public static string IncChar(string s, int index, bool trunc = false) => $"{s.Substring(0, index)}{(char)(s[index] + 1)}{(trunc ? "" : s.Substring(index + 1))}";
		public static string SetChar(string s, int index, char c) => $"{s.Substring(0, index)}{c}{s.Substring(index + 1)}";
		public static string RemoveTrailingZeros(string s) {
			int dotIndex = s.IndexOf('.');
			if (dotIndex < 0)
				return s;

			for (int i = s.Length - 1; i > dotIndex; i--) {
				if (s[i] != '0') {
					if (s[i] == '.') {
						return s.Substring(0, i);
					}
					else {
						return s.Substring(0, i + 1);
					}
				}
			}

			return s.Substring(0, dotIndex);
		}
	}
}
