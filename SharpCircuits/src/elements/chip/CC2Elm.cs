using System;
using System.Collections;
using System.Collections.Generic;

namespace SharpCircuit {

	public class CC2Elm : Chip {

		private double gain;

		public CC2Elm() : base() {
			gain = 1;
		}

		public CC2Elm(int g) : base() {
			gain = g;
		}

		public override String getChipName() {
			return "CC2";
		}

		public override void setupPins() {
			pins = new Pin[3];
			pins[0] = new Pin("X");
			pins[0].output = true;
			pins[1] = new Pin("Y");
			pins[2] = new Pin("Z");
		}

		/*public override void getInfo(String[] arr) {
			arr[0] = (gain == 1) ? "CCII+" : "CCII-";
			arr[1] = "X,Y = " + getVoltageText(lead_volt[0]);
			arr[2] = "Z = " + getVoltageText(lead_volt[2]);
			arr[3] = "I = " + getCurrentText(pins[0].current);
		}*/

		// boolean nonLinear() { return true; }
		public override void stamp(Circuit sim) {
			// X voltage = Y voltage
			sim.stampVoltageSource(0, lead_node[0], pins[0].voltSource);
			sim.stampVCVS(0, lead_node[1], 1, pins[0].voltSource);
			// Z current = gain * X current
			sim.stampCCCS(0, lead_node[2], pins[0].voltSource, gain);
		}

		public override int getLeadCount() {
			return 3;
		}

		public override int getVoltageSourceCount() {
			return 1;
		}

	}

	class CC2NegElm : CC2Elm {

		public CC2NegElm() : base() {

		}

	}
}