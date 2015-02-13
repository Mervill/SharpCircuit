using System;
using System.Collections;
using System.Collections.Generic;

namespace SharpCircuit {

	public class NandGateElm : AndGateElm {

		public NandGateElm() {

		}

		public override bool isInverting() {
			return true;
		}

		public override String getGateName() {
			return "NAND gate";
		}

	}
}