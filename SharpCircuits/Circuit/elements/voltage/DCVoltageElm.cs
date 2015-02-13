using System;
using System.Collections;
using System.Collections.Generic;

namespace SharpCircuit {
	
	public class DCVoltageElm : VoltageElm {

		//public ElementLead leadIn 	{ get { return lead0; }}
		//public ElementLead leadOut 	{ get { return lead1; }}

		public DCVoltageElm() : base(WaveType.DC) {

		}

	}
}