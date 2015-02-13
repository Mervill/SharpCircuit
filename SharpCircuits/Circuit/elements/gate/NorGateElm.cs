using System;
using System.Collections;
using System.Collections.Generic;

namespace SharpCircuit {

	public class NorGateElm : OrGateElm {

		public NorGateElm() {

		}

		public override String getGateName() {
			return "NOR gate";
		}

		public override bool isInverting() {
			return true;
		}

	}
}