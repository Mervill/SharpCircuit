using System;
using System.Collections;
using System.Collections.Generic;

namespace SharpCircuit {

	public class AndGateElm : GateElm {

		public AndGateElm() {

		}

		public override String getGateName() {
			return "AND gate";
		}

		public override bool calcFunction() {
			int i;
			bool f = true;
			for(i = 0; i != inputCount; i++)
				f &= getInput(i);
			return f;
		}

	}
}