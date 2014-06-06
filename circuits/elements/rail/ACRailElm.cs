using System;
using System.Collections;
using System.Collections.Generic;

namespace Circuits {

	public class ACRailElm : RailElm {
		
		public ACRailElm(int xx, int yy, CirSim s) : base(xx, yy, WF_AC, s) {
			
		}

	}
}