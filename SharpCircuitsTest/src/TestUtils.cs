using System;
using System.Collections.Generic;
using System.Linq;

using NUnit.Framework;

namespace SharpCircuitTest {

	public static class TestUtils {

		public static void Compare(double a, double b, int tolerance) {
			Assert.That(Round(a, tolerance), Is.EqualTo(Round(b, tolerance)).Within(Math.Pow(10, -tolerance)));
		}

		public static double Round(double val, int places) {
			if(places < 0) throw new ArgumentException("places");
			return Math.Round(val - (0.5 / Math.Pow(10, places)), places);
		}
	}
}
