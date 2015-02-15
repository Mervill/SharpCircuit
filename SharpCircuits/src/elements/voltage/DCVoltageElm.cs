using System;
using System.Collections;
using System.Collections.Generic;

namespace SharpCircuit {
	
	public class DCVoltageElm : VoltageElm {

		public Circuit.Lead leadIn { get { return lead0; } }
		public Circuit.Lead leadOut { get { return lead1; } }

		public DCVoltageElm() : base(WaveType.DC) {

		}

	}
}