using System;
using System.Collections;
using System.Collections.Generic;

namespace SharpCircuit {
	
	public class OutputElm : CircuitElement {

		//public ElementLead leadIn { get { return lead0; }}

		public OutputElm() {

		}

		public override int getLeadCount() {
			return 1;
		}

		public override double getVoltageDiff() {
			return volts[0];
		}

		public override void getInfo(String[] arr) {
			arr[0] = "output";
			arr[1] = "V = " + getVoltageText(volts[0]);
		}

	}
}