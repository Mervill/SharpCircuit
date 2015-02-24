using System;
using System.Collections;
using System.Collections.Generic;

namespace SharpCircuit {

	public class VarRailElm : VoltageInput {

		public double output { get; set; }

		public VarRailElm() : base(WaveType.VAR) {
			output = 1;
			frequency = maxVoltage;
			waveform = WaveType.VAR;
		}

		protected override double getVoltage(Circuit sim) {
			frequency = output * (maxVoltage - bias) + bias;
			return base.getVoltage(sim);
		}

	}
}