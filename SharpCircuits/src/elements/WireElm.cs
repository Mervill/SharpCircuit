using System;
using System.Collections;
using System.Collections.Generic;

namespace SharpCircuit {

	public class WireElm : CircuitElement {

		public Circuit.Lead leadIn { get { return lead0; } }
		public Circuit.Lead leadOut { get { return lead1; } }
		
		public override void stamp(Circuit sim) {
			sim.stampVoltageSource(lead_node[0], lead_node[1], voltSource, 0);
		}

		public override int getVoltageSourceCount() {
			return 1;
		}

		public override void getInfo(String[] arr) {
			arr[0] = "wire";
			arr[1] = "I = " + getCurrentDText(current);
			arr[2] = "V = " + getVoltageText(lead_volt[0]);
		}

		public override double getPower() {
			return 0;
		}

		public override double getVoltageDiff() {
			return lead_volt[0];
		}

		public override bool isWire() {
			return true;
		}

	}
}