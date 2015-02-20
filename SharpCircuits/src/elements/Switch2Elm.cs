using System;
using System.Collections;
using System.Collections.Generic;

namespace SharpCircuit {

	public class Switch2Elm : SwitchElm {

		public Switch2Elm() : base() {
			posCount = 3;
		}

		public override int getLeadCount() {
			return 3;
		}

		public override void calculateCurrent() {
			if(position == 2)
				current = 0;
		}

		public override void stamp(Circuit sim) {
			if(position == 2) return; // in center?
			sim.stampVoltageSource(lead_node[0], lead_node[position + 1], voltSource, 0);
		}

		public override int getVoltageSourceCount() {
			return (position == 2) ? 0 : 1;
		}

		/*public override void getInfo(String[] arr) {
			arr[0] = "switch (SPDT)";
			arr[1] = "I = " + CircuitElement.getCurrentDText(getCurrent());
		}*/

		public override bool leadsAreConnected(int leadX, int leadY) {
			if(position == 2) return false;
			return comparePair(leadX, leadY, 0, 1 + position);
		}
	}
}
