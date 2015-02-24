using System;
using System.Collections;
using System.Collections.Generic;

namespace SharpCircuit {

	public class NandGate : AndGate {

		public override bool isInverting() { return true; }

	}
}