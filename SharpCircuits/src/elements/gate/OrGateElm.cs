using System;
using System.Collections;
using System.Collections.Generic;

namespace SharpCircuit {

	public class OrGateElm : GateElm {

		public OrGateElm() {

		}

		public override String getGateName() {
			return "OR gate";
		}

		public override bool calcFunction() {
			int i;
			bool f = false;
			for(i = 0; i != inputCount; i++)
				f |= getInput(i);
			return f;
		}

	}
}