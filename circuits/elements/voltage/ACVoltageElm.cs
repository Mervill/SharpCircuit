using System;
using System.Collections;
using System.Collections.Generic;

namespace Circuits {

	public class ACVoltageElm : VoltageElm {
		
		public ACVoltageElm(CirSim s) : base(s,WaveformType.AC) {
			
		}
		
	}
}