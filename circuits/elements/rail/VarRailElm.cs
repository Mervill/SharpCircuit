using System;
using System.Collections;
using System.Collections.Generic;

namespace Circuits {

	public class VarRailElm : RailElm {

		public double output{ get; set; }

		public VarRailElm(CirSim s) : base(s,WaveformType.VAR) {
			output = 1;
			frequency = MaxVoltage;
			waveform = WaveformType.VAR;
		}

		public override double getVoltage() {
			//frequency = slider * (MaxVoltage - Bias) / 100.0 + Bias;
			frequency = output * (MaxVoltage - Bias) + Bias;
			return frequency;
		}

	}
}