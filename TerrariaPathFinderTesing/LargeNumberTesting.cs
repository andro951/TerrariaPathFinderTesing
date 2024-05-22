using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TerrariaPathFinderTesing {
	public static class LargeNumberTesting {
		public static void Test() {
			long sqrt = (long)Math.Sqrt(long.MaxValue);//3037000499, 1011_0101_0000_0100_1111_0011_0011_0011
			long max = 0b0111_1111_1111_1111_1111_1111_1111_1111_1111_1111_1111_1111_1111_1111_1111_1111;
			long max2 = 0x7FFFFFFFFFFFFFFF;
			$"max: {max}, max2: {max2}, long.MaxValue: {long.MaxValue}".Log();
			long sqr = sqrt * sqrt;
			long min = long.MaxValue - sqr;
			long div = min / long.MaxValue;
			$"sqrt: {sqrt}, sqr: {sqr}, min: {min}, div: {div}".Log();
			$"sqrt in binary: {Convert.ToString(sqrt, 2)}".Log();

			long SquareRootOfLong = 3037000499;
			long test = (SquareRootOfLong + 1) * (SquareRootOfLong + 1);
			$"test: {test}".Log();

			num left = new(684127, 100);
			num right = new(25, 1);
			num mult = left * right;
			$"left: {left}, right: {right}, mult: {mult}".Log();
		}
	}
}
