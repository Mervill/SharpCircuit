using System;
using System.Collections;
using System.Collections.Generic;

namespace SharpCircuit {

	public class NorGateElm : OrGateElm {

		public override bool isInverting() { return true; }

	}
}