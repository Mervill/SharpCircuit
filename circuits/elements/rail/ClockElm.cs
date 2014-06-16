using System;
using System.Collections;
using System.Collections.Generic;

namespace Circuits {

	public class ClockElm : RailElm {
		
		public ClockElm(CirSim s) : base(s,WaveType.SQUARE) {
			maxVoltage = 2.5;
			bias = 2.5;
			frequency = 100;
		}
		
	}
}