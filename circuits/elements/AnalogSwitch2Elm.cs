using System;
using System.Collections;
using System.Collections.Generic;

namespace Circuits {

	// Unfinished

	// Initializers	[X]
	// Properties	[X]
	// Leads		[_]
	// Test Basic	[_]
	// Test Prop	[_]
	public class AnalogSwitch2Elm : AnalogSwitchElm {
		
		public AnalogSwitch2Elm(CirSim s) : base(s) {

		}

		public override int getLeadCount() {
			return 4;
		}

		public override void calculateCurrent() {
			if (open) {
				current = (volts[0] - volts[2]) / r_on;
			} else {
				current = (volts[0] - volts[1]) / r_on;
			}
		}

		public override void stamp() {
			sim.stampNonLinear(nodes[0]);
			sim.stampNonLinear(nodes[1]);
			sim.stampNonLinear(nodes[2]);
		}

		public override void doStep() {
			open = (volts[3] < 2.5);
			if ((flags & FLAG_INVERT) != 0) {
				open = !open;
			}
			if (open) {
				sim.stampResistor(nodes[0], nodes[2], r_on);
				sim.stampResistor(nodes[0], nodes[1], r_off);
			} else {
				sim.stampResistor(nodes[0], nodes[1], r_on);
				sim.stampResistor(nodes[0], nodes[2], r_off);
			}
		}

		public override bool getConnection(int n1, int n2) {
			if (n1 == 3 || n2 == 3) {
				return false;
			}
			return true;
		}

		public override void getInfo(String[] arr) {
			arr[0] = "analog switch (SPDT)";
			arr[1] = "I = " + getCurrentDText(getCurrent());
		}
	}
}