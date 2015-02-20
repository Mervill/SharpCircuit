using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpCircuit {

	public class SIUnits {

		public const double pico  = 1E12;// 0.000000000001 -> 1p
		public const double nano  = 1E9; // 0.000000001 -> 1n
		public const double micro = 1E6; // 0.000001 -> 1u
		public const double milli = 1E3; // 0.001 -> 1m
		/*public const double hecto = 1E2;
		public const double deca = 1E1;
		public const double deci = 1E-1;
		public const double centi = 1E-2;*/
		public const double kilo = 1E-3; // 1000 -> 1K
		public const double mega = 1E-6; // 1000000 -> 1M
		public const double giga = 1E-9; // 1000000000 -> 1G
		public const double tera = 1E-12;// 1000000000000 -> 1T

		/*public static double Convert(double v) {
			double va = Math.Abs(v);
			if(va < 1E-14) return 0;
			if(va < 1E-9 ) return v * 1E12; // pico
			if(va < 1E-6 ) return v * 1E9;  // nano
			if(va < 1E-3 ) return v * 1E6;  // micro
			if(va < 1    ) return v * 1E3;  // milli
			if(va < 1E3  ) return v;
			if(va < 1E6  ) return v * 1E-3; // kilo
			if(va < 1E9  ) return v * 1E-6; // mega
			if(va < 1E12 ) return v * 1E-9; // giga
			return v * 1E-12;// tera
		}*/

		static string Normalize(double v) {
			double va = Math.Abs(v);
			if(va < 1E-14) return "0";
			if(va < 1E-9 ) return v * 1E12 + "p"; // pico
			if(va < 1E-6 ) return v * 1E9 + "n";  // nano
			if(va < 1E-3 ) return v * 1E6 + "u";  // micro
			if(va < 1    ) return v * 1E3 + "m";  // milli
			if(va < 1E3  ) return v.ToString();
			if(va < 1E6  ) return v * 1E-3 + "K"; // kilo
			if(va < 1E9  ) return v * 1E-6 + "M"; // mega
			if(va < 1E12 ) return v * 1E-9 + "G"; // giga
			return v * 1E-12 + "T";               // tera
		}

		public static string Normalize(double v, string u) {
			return Normalize(v) + u;
		}

		public static string NormalizeRounded(double v, int d) {
			return Normalize(Math.Round(v, d));
		}

		public static string NormalizeRounded(double v, int d, string u) {
			return Normalize(Math.Round(v, d), u);
		}

		public static string Voltage(double v) {
			return Normalize(v, "V");
		}

		public static string VoltageRounded(double v, int d) {
			return NormalizeRounded(v, d, "V");
		}

		public static string VoltageABS(double v) {
			return Normalize(Math.Abs(v), "V");
		}

		public static string Current(double i) {
			return Normalize(i, "A");
		}

		public static string CurrentRounded(double i, int d) {
			return NormalizeRounded(i, d, "A");
		}

		public static string CurrentABS(double i) {
			return Normalize(Math.Abs(i), "A");
		}

	}
}
