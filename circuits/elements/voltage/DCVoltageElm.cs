using System;
using System.Collections;
using System.Collections.Generic;

namespace Circuits {

	public class DCVoltageElm : VoltageElm {
		
		public DCVoltageElm( CirSim s) : base(WF_DC, s) {
			
		}

	}
}