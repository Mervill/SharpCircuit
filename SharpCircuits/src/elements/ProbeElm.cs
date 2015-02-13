using System;
using System.Collections;
using System.Collections.Generic;

namespace SharpCircuit {
	
	public class ProbeElm : CircuitElement {

		//public ElementLead leadIn 	{ get { return lead0; }}
		//public ElementLead leadOut 	{ get { return lead1; }}

		public ProbeElm() {

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