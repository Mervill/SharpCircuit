using System;
using System.Collections;
using System.Collections.Generic;

namespace SharpCircuit {

	// Unfinished

	public class AnalogSwitch2 : AnalogSwitch {

		public override int getLeadCount() {
			return 4;
		}

		public override void calculateCurrent() {
			if(open) {
				current = (lead_volt[0] - lead_volt[2]) / r_on;
			} else {
				current = (lead_volt[0] - lead_volt[1]) / r_on;
			}
		}

		public override void stamp(Circuit sim) {
			sim.stampNonLinear(lead_node[0]);
			sim.stampNonLinear(lead_node[1]);
			sim.stampNonLinear(lead_node[2]);
		}

		public override void step(Circuit sim) {
			open = (lead_volt[3] < 2.5);
			if(invert) open = !open;
			if(open) {
				sim.stampResistor(lead_node[0], lead_node[2], r_on);
				sim.stampResistor(lead_node[0], lead_node[1], r_off);
			} else {
				sim.stampResistor(lead_node[0], lead_node[1], r_on);
				sim.stampResistor(lead_node[0], lead_node[2], r_off);
			}
		}

		public override bool leadsAreConnected(int n1, int n2) {
			return !(n1 == 3 || n2 == 3);
		}

		/*public override void getInfo(String[] arr) {
			arr[0] = "analog switch (SPDT)";
			arr[1] = "I = " + getCurrentDText(current);
		}*/
	}
}