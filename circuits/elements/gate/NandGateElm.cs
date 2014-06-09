using System;
using System.Collections;
using System.Collections.Generic;

namespace Circuits {

	public class NandGateElm : AndGateElm {

		public NandGateElm(CirSim s) : base(s) {

		}

		public override bool isInverting() {
			return true;
		}

		public override String getGateName() {
			return "NAND gate";
		}

	}
}