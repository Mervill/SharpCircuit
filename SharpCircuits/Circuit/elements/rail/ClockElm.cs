using System;
using System.Collections;
using System.Collections.Generic;

namespace SharpCircuit {

	public class ClockElm : RailElm {
		
		public ClockElm() : base(WaveType.SQUARE) {
			maxVoltage = 2.5;
			bias = 2.5;
			frequency = 100;
		}
		
	}
}