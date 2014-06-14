using System;
using System.Collections;
using System.Collections.Generic;

namespace Circuits {

	public class PhaseCompElm : ChipElm {

		private bool ff1, ff2;

		public PhaseCompElm(CirSim s) : base(s) {

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

		public override bool nonLinear() {
			return true;
		}

		public override void stamp() {
			int vn = sim.nodeList.Count + pins[2].voltSource;
			sim.stampNonLinear(vn);
			sim.stampNonLinear(0);
			sim.stampNonLinear(nodes[2]);
		}
		
		public override  void doStep() {
			bool v1 = volts[0] > 2.5;
			bool v2 = volts[1] > 2.5;
			if (v1 && !pins[0].value) {
				ff1 = true;
			}
			if (v2 && !pins[1].value) {
				ff2 = true;
			}
			if (ff1 && ff2) {
				ff1 = ff2 = false;
			}
			double @out = (ff1) ? 5 : (ff2) ? 0 : -1;
			// System.out.println(out + " " + v1 + " " + v2);
			if (@out != -1) {
				sim.stampVoltageSource(0, nodes[2], pins[2].voltSource, @out);
			} else {
				// tie current through output pin to 0
				int vn = sim.nodeList.Count + pins[2].voltSource;
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