using System;
using System.Collections;
using System.Collections.Generic;

namespace Circuits {

	public class VarRailElm : RailElm {

		public double output{ get; set; }

		public VarRailElm(CirSim s) : base(s,WaveType.VAR) {
			output = 1;
			frequency = maxVoltage;
			waveform = WaveType.VAR;
		}

		public override double getVoltage() {
			//frequency = slider * (MaxVoltage - Bias) / 100.0 + Bias;
			frequency = output * (maxVoltage - bias) + bias;
			return frequency;
		}

	}
}