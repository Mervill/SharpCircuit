using System;
using System.Collections;
using System.Collections.Generic;

namespace SharpCircuit {

	public class NandGateElm : AndGateElm {

		public override bool isInverting() { return true; }

	}
}