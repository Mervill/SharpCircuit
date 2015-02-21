using System;
using System.Collections;
using System.Collections.Generic;

namespace SharpCircuit {

	public class OutputElm : CircuitElement {

		public Circuit.Lead leadIn { get { return lead0; } }

		public override int getLeadCount() {
			return 1;
		}

		public override double getVoltageDelta() {
			return lead_volt[0];
		}

	}
}
