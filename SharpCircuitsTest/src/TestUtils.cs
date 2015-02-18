using System;
using System.Collections.Generic;
using System.Linq;

using NUnit.Framework;

namespace SharpCircuitTest {

	public static class TestUtils {

		public static void Compare(double a, double b, int tolerance) {
			System.Func<double,int,double> Round = (double val, int places) => {
				return Math.Round(val - (0.5 / Math.Pow(10, places)), places);
			};
			Assert.That(Round(a, tolerance), Is.EqualTo(Round(b, tolerance)).Within(Math.Pow(10, -tolerance)));
		}

	}
}
