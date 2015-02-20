using System;
using System.Collections;
using System.Collections.Generic;

namespace SharpCircuit {

	// Zener code contributed by J. Mike Rollins
	// http://www.camotruck.net/rollins/simulator.html
	public class ZenerElm : DiodeElm {

		public static readonly double default_zvoltage = 5.6;

		public ZenerElm() : base() {
			diode.leakage = 5e-6; // 1N4004 is 5.0 uAmp
			zvoltage = default_zvoltage;
			setup();
		}

		/*public override void getInfo(String[] arr) {
			base.getInfo(arr);
			arr[0] = "Zener diode";
			arr[5] = "Vz = " + getVoltageText(zvoltage);
		}*/

	}
}