using System;
using System.Collections;
using System.Collections.Generic;

namespace SharpCircuit {

	public class ClockInput : VoltageInput {
		
		public ClockInput() : base(WaveType.SQUARE) {
			maxVoltage = 2.5;
			bias = 2.5;
			frequency = 100;
		}
		
	}
}