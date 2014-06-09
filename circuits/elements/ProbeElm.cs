using System;
using System.Collections;
using System.Collections.Generic;

namespace Circuits {

	public class ProbeElm : CircuitElement {

		public ProbeElm( CirSim s) : base(s) {
			
		}

		public override void getInfo(String[] arr) {
			arr[0] = "scope probe";
			arr[1] = "Vd = " + getVoltageText(getVoltageDiff());
		}

		public override bool getConnection(int n1, int n2) {
			return false;
		}

	}
}