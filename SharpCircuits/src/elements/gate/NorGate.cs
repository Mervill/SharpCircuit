using System;
using System.Collections;
using System.Collections.Generic;

namespace SharpCircuit {

	public class NorGate : OrGate {

		public override bool isInverting() { return true; }

	}
}