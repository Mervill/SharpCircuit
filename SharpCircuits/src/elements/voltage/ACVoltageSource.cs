using System;
using System.Collections;
using System.Collections.Generic;

namespace SharpCircuit {
	
	public class ACVoltageSource : Voltage {

		public Circuit.Lead leadPos { get { return lead0; } }
		public Circuit.Lead leadNeg { get { return lead1; } }

		public ACVoltageSource() : base(WaveType.AC) {

		}

	}
}