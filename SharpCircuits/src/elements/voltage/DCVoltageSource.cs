using System;
using System.Collections;
using System.Collections.Generic;

namespace SharpCircuit {
	
	public class DCVoltageSource : Voltage {

		public Circuit.Lead leadPos { get { return lead0; } }
		public Circuit.Lead leadNeg { get { return lead1; } }

		public DCVoltageSource() : base(WaveType.DC) {

		}

	}
}