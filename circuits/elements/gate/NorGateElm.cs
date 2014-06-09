using System;
using System.Collections;
using System.Collections.Generic;

namespace Circuits {

	public class NorGateElm : OrGateElm {
		
		public NorGateElm( CirSim s) : base(s) {
			
		}
		
		public override String getGateName() {
			return "NOR gate";
		}

		public override bool isInverting() {
			return true;
		}

	}
}