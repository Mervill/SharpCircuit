using System;
using System.Collections;
using System.Collections.Generic;

namespace SharpCircuit {

	public class VarRailElm : RailElm {

		public double output { get; set; }

		public VarRailElm() : base(WaveType.VAR) {

		}

		protected override void onGetSim() {
			output = 1;
			frequency = maxVoltage;
			waveform = WaveType.VAR;
		}

		public override double getVoltage() {
			frequency = output * (maxVoltage - bias) + bias;
			return frequency;
		}

	}
}