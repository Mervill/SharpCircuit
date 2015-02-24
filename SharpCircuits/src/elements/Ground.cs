using System;
using System.Collections;
using System.Collections.Generic;

namespace SharpCircuit {

	public class Ground : Output {

		public Ground() : base() {

		}

		public override void setCurrent(int x, double c) {
			current = -c;
		}

		public override void stamp(Circuit sim) {
			sim.stampVoltageSource(0, lead_node[0], voltSource, 0);
		}

		public override double getVoltageDelta() {
			return 0;
		}

		public override int getVoltageSourceCount() {
			return 1;
		}

		public override bool leadIsGround(int n1) {
			return true;
		}

	}
}