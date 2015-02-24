using System;
using System.Collections;
using System.Collections.Generic;

namespace SharpCircuit {

	public class PhaseCompElm : Chip {

		private bool ff1, ff2;

		public PhaseCompElm() : base() {

		}

		public override String getChipName() {
			return "phase comparator";
		}

		public override void setupPins() {
			pins = new Pin[3];
			pins[0] = new Pin("I1");
			pins[1] = new Pin("I2");
			pins[2] = new Pin("O");
			pins[2].output = true;
		}

		public override bool nonLinear() { return true; }

		public override void stamp(Circuit sim) {
			int vn = sim.nodeCount + pins[2].voltSource;
			sim.stampNonLinear(vn);
			sim.stampNonLinear(0);
			sim.stampNonLinear(lead_node[2]);
		}

		public override void step(Circuit sim) {
			bool v1 = lead_volt[0] > 2.5;
			bool v2 = lead_volt[1] > 2.5;
			if(v1 && !pins[0].value)
				ff1 = true;
			if(v2 && !pins[1].value)
				ff2 = true;
			if(ff1 && ff2)
				ff1 = ff2 = false;
			double @out = (ff1) ? 5 : (ff2) ? 0 : -1;
			// System.out.println(out + " " + v1 + " " + v2);
			if(@out != -1) {
				sim.stampVoltageSource(0, lead_node[2], pins[2].voltSource, @out);
			} else {
				// tie current through output pin to 0
				int vn = sim.nodeCount + pins[2].voltSource;
				sim.stampMatrix(vn, vn, 1);
			}
			pins[0].value = v1;
			pins[1].value = v2;
		}

		public override int getLeadCount() {
			return 3;
		}

		public override int getVoltageSourceCount() {
			return 1;
		}

	}
}