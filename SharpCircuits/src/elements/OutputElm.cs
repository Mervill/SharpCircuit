using System;
using System.Collections;
using System.Collections.Generic;

namespace SharpCircuit {

	public class OutputElm : CircuitElement {

		public Circuit.Lead leadIn { get { return lead0; } }

		public override int getLeadCount() {
			return 1;
		}

		public override double getVoltageDiff() {
			return lead_volt[0];
		}

		public override void getInfo(String[] arr) {
			arr[0] = "output";
			arr[1] = "V = " + getVoltageText(lead_volt[0]);
		}

	}
}