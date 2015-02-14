using System;
using System.Collections;
using System.Collections.Generic;

namespace SharpCircuit {
	
	public class WireElm : CircuitElement {

		//public ElementLead leadIn 	{ get { return lead0; }}
		//public ElementLead leadOut	{ get { return lead1; }}

		public WireElm() {

		}

		public override void stamp(CirSim sim) {
			sim.stampVoltageSource(nodes[0], nodes[1], voltSource, 0);
		}

		public override int getVoltageSourceCount() {
			return 1;
		}

		public override void getInfo(String[] arr) {
			arr[0] = "wire";
			arr[1] = "I = " + getCurrentDText(current);
			arr[2] = "V = " + getVoltageText(volts[0]);
		}

		public override double getPower() {
			return 0;
		}

		public override double getVoltageDiff() {
			return volts[0];
		}

		public override bool isWire() {
			return true;
		}

	}
}