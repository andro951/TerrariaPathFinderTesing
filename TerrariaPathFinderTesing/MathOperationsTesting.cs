using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TerrariaPathFinderTesting;

namespace TerrariaPathFinderTesing {
	internal static class MathOperationsTesting {
		internal static void Test() {
			List<(string, double)> strings = new() {
				("5.00 + 3.00 * 2.00", 5 + 3 * 2),
				("5.00 * 3.00 + 2.00", 5 * 3 + 2),
				("(5.00 + 3.00) * 2.00", (5 + 3) * 2),
				("2.00 * (5.00 + 3.00)", 2 * (5 + 3)),
				($"{nameof(ExampleVariable1)} + 3.00 * 2.00", ExampleVariable1 + 3 * 2),
				($"({nameof(ExampleVariable1)} + 3.00) * 2.00", (ExampleVariable1 + 3) *2),
				("((((5.00))))", 5d),
				("(((12.50 + 10.00) - 2.00^x) * 10.00)", (((12.5 + 10d) - Math.Pow(2, x)) * 10)),
				("log(200.00, 10.00)", Math.Log(200, 10)),
				("log(200.00 * 2.00, 10.00 + 1.00)", Math.Log(200 * 2, 10 + 1)),
				("(log(200.00, 10.00))", Math.Log(200, 10)),
				("(log(log((200.00 + 10.00), 10.00), 10.00))", Math.Log(Math.Log(200 + 10, 10), 10)),
				("(log((12.50 + 10.00) - 2.00^x, 10.00))", (Math.Log((12.5d + 10d) - Math.Pow(2, x), 10d))),
				("(log(log(12.50, 10.00) + 2.00^x, 10.00))", (Math.Log(Math.Log(12.5d, 10d) + Math.Pow(2, x), 10d))),
				("5.00 * log(2.00, 10.00)", 5.00 * Math.Log(2.00, 10.00)),
				("pi * (π + e^2.00)", Math.PI * (Math.PI + Math.Pow(Math.E,2))),
			};

			foreach ((string s, double v) p in strings) {
				Value? sValue = p.s.GetEquationValue();
				$"{p.s} => {sValue} = {sValue?.GetValue()} (Expected value: {p.v})".Log();
				if (p.s != sValue?.ToString())
					$"Strings don't match; Expected: {p.s} != Result: {sValue}".LogError();

				if (p.v != sValue?.GetValue())
					$"Values don't match; Expected: {p.v} != Result: {sValue?.GetValue()}".LogError();
			}
		}
		private static double ExampleVariable1 = 5;
		private static double x = 2.14d;
		internal static Func<double> StringHandler(string s) {
			if (Variables.TryGetValue(s, out Func<double>? value)) {
				return value;
			}

			throw new Exception($"Failed to find a function for s: {s}");
		}
		private static Dictionary<string, Func<double>> Variables = new() {
			{ nameof(ExampleVariable1), () => ExampleVariable1 },
			{ nameof(x), () => x }
		};
	}
}
