using System;
using System.Collections;
using System.Collections.Generic;

namespace SharpCircuit {
	
	public class GroundElm : CircuitElement {

		//public ElementLead leadIn { get { return lead0; }}

		public GroundElm() {

		}

		public override int getLeadCount() {
			return 1;
		}

		public override void setCurrent(int x, double c) {
			current = -c;
		}

		public override void stamp() {
			sim.stampVoltageSource(0, nodes[0], voltSource, 0);
		}

		public override double getVoltageDiff() {
			return 0;
		}

		public override int getVoltageSourceCount() {
			return 1;
		}

		public override void getInfo(String[] arr) {
			arr[0] = "ground";
			arr[1] = "I = " + getCurrentText(current);
		}

		public override bool hasGroundConnection(int n1) {
			return true;
		}

	}
}