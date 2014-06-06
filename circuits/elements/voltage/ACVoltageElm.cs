using System;
using System.Collections;
using System.Collections.Generic;

namespace Circuits {

	public class ACVoltageElm : VoltageElm {
		
		public ACVoltageElm(int xx, int yy, CirSim s) : base(xx, yy, WF_AC, s) {
			
		}
		
	}
}