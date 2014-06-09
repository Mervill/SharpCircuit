using System;
using System.Collections;
using System.Collections.Generic;

namespace Circuits {

	public class AndGateElm : GateElm {
	
		public AndGateElm(CirSim s) : base(s) {
			
		}

		public override String getGateName() {
			return "AND gate";
		}

		public override bool calcFunction() {
			int i;
			bool f = true;
			for (i = 0; i != inputCount; i++) {
				f &= getInput(i);
			}
			return f;
		}

	}
}