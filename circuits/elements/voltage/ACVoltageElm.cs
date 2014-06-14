using System;
using System.Collections;
using System.Collections.Generic;

namespace Circuits {

	// Initializers	[X]
	// Properties	[X]
	// Leads		[X]
	// Test Basic	[_]
	// Test Prop	[_]
	public class ACVoltageElm : VoltageElm {
		
		public ACVoltageElm(CirSim s) : base(s,WaveType.AC) {
			
		}
		
	}
}