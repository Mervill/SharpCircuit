using System;
using System.Collections;
using System.Collections.Generic;

namespace Circuits {

	// Initializers	[X]
	// Properties	[X]
	// Leads		[X]
	// Test Basic	[_]
	// Test Prop	[_]
	public class DCVoltageElm : VoltageElm {
		
		public DCVoltageElm(CirSim s) : base(s,WaveType.DC) {
			
		}

	}
}