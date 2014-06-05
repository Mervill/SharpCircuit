using System;
using System.Collections;
using System.Collections.Generic;

namespace Circuts {

	public class NandGateElm : AndGateElm {

		public NandGateElm(int xx, int yy, CirSim s) : base(xx, yy, s) {

		}

		public override bool isInverting() {
			return true;
		}

		public override String getGateName() {
			return "NAND gate";
		}

		public override int getDumpType() {
			return 151;
		}

		public override int getShortcut() {
			return '@';
		}
	}
}