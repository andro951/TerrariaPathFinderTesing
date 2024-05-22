using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TerrariaPathFinderTesing {
	
	public struct num {
		private const long SquareRootOfLongMaxValue = 3037000499;//Square root of long.MaxValue rounded down.  Prevents overflow when multiplying two significands.
		private long significand;
		private int exponent;
		public num (long significand, int exponent) {
			this.significand = significand;
			this.exponent = exponent;
		}
		//public static num operator +(num left, num right) {

		//}
		public static num operator *(num left, num right) {
			if (left.significand > SquareRootOfLongMaxValue || right.significand > SquareRootOfLongMaxValue) {
				long max = long.MaxValue / left.significand;
				if (right.significand > max) {
					int exp = (int)right.significand.CeilingDivide(max);
					Reduce(ref left, ref right, exp);
				}
			}

			int leftExp = left.RealizeSignificand();
			int rightExp = right.RealizeSignificand();
			long mult = left.significand * right.significand;
			int expToAdd = (int)Math.Log10(mult);

			return new num(mult, left.exponent + right.exponent + expToAdd);
		}
		private int RealizeSignificand() {
			int reduction = (int)Math.Log10(significand);
			exponent -= reduction;

			return reduction;
		}
		private static void Reduce(ref num left, ref num right, int exp) {//TODO: Check what happens when exp is negative
			int leftRed;
			int rightRed;
			//long expAbs = Math.Abs(exp);
			if (left.significand > right.significand) {
				long div = left.significand.CeilingDivide(right.significand);
				int expDiff = div.CeilingLog10();
				rightRed = Math.Max(0, (exp - expDiff) / 2);
				leftRed = exp - rightRed;
			}
			else {
				long div = right.significand.CeilingDivide(left.significand);
				int expDiff = div.CeilingLog10();
				leftRed = Math.Max(0, (exp - expDiff) / 2);
				rightRed = exp - leftRed;
			}

			if (leftRed != 0) {
				left.significand /= (long)Math.Pow(10, leftRed);
				left.exponent += leftRed;
			}

			if (rightRed != 0) {
				right.significand /= (long)Math.Pow(10, rightRed);
				right.exponent += rightRed;
			}
		}

		public override string ToString() => S(2);

		public string S(int decimals = 4) {
			string s = significand.ToString();
			char n = s.Length > 0 ? s[0] : '0';
			int dLength = Math.Min(s.Length - 1, decimals - 1);
			string d = s.Substring(1, dLength) + string.Concat(Enumerable.Repeat("0", decimals - dLength));
			return $"{s[0]}.{d}e{exponent}";
		}
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
