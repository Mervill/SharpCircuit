using System;
using System.Collections;
using System.Collections.Generic;

namespace Circuits {

	public class ClockElm : RailElm {
		
		public ClockElm(int xx, int yy, CirSim s) : base(xx, yy, WF_SQUARE, s) {
			maxVoltage = 2.5;
			bias = 2.5;
			frequency = 100;
			flags |= FLAG_CLOCK;
		}
		
	}
}