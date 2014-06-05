using System;
using System.Collections;
using System.Collections.Generic;

namespace Circuts {

	public class NorGateElm : OrGateElm {
		
		public NorGateElm(int xx, int yy, CirSim s) : base(xx, yy, s) {
			
		}
		
		public override String getGateName() {
			return "NOR gate";
		}

		public override bool isInverting() {
			return true;
		}

		public override int getDumpType() {
			return 153;
		}

		public override int getShortcut() {
			return '#';
		}
	}
}