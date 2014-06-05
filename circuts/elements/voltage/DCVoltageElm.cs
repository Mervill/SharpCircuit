using System;
using System.Collections;
using System.Collections.Generic;

namespace Circuts {

	public class DCVoltageElm : VoltageElm {
		
		public DCVoltageElm(int xx, int yy, CirSim s) : base(xx, yy, WF_DC, s) {
			
		}

	}
}