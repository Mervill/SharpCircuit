using System;
using System.Collections;
using System.Collections.Generic;

namespace SharpCircuit {

	public class AndGateElm : GateElm {
		
		public override bool calcFunction() {
			bool f = true;
			for(int i = 0; i != inputCount; i++)
				f &= getInput(i);
			return f;
		}

	}
}