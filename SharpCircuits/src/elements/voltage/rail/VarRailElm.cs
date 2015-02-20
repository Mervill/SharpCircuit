using System;
using System.Collections;
using System.Collections.Generic;

namespace SharpCircuit {

	public class VarRailElm : VoltageInputElm {

		public double output { get; set; }

		public VarRailElm() : base(WaveType.VAR) {
			output = 1;
			frequency = maxVoltage;
			waveform = WaveType.VAR;
		}

		public override double getVoltage(Circuit sim) {
			frequency = output * (maxVoltage - bias) + bias;
			return base.getVoltage(sim);
		}

	}
}