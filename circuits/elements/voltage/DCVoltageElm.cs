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

		public ElementLead leadIn 	{ get { return leads[0]; }}
		public ElementLead leadOut 	{ get { return leads[1]; }}

		public DCVoltageElm(CirSim s) : base(s,WaveType.DC) {
			
		}

	}
}