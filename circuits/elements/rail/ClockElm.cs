using System;
using System.Collections;
using System.Collections.Generic;

namespace Circuits {

	public class ClockElm : RailElm {
		
		public ClockElm(CirSim s) : base(WaveformType.SQUARE, s) {
			MaxVoltage = 2.5;
			Bias = 2.5;
			frequency = 100;
			flags |= FLAG_CLOCK;
		}
		
	}
}