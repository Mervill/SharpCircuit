using System;
using System.Collections;
using System.Collections.Generic;

namespace Circuts {

	public class OpAmpSwapElm : OpAmpElm {
		
		public OpAmpSwapElm(int xx, int yy, CirSim s) : base (xx,yy,s) {
			flags |= FLAG_SWAP;
		}
		
	}
}