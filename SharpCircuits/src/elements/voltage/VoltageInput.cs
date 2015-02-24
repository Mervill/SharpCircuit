using System;
using System.Collections;
using System.Collections.Generic;

namespace SharpCircuit {
	
	public class VoltageInput : Voltage {

		public Circuit.Lead leadPos { get { return lead0; } }

		public VoltageInput() : base(WaveType.DC) {

		}

		public VoltageInput(WaveType wf) : base(wf) {

		}

		public override int getLeadCount() {
			return 1;
		}

		public override double getVoltageDelta() {
			return lead_volt[0];
		}

		public override void stamp(Circuit sim) {
			if (waveform == WaveType.DC) {
				sim.stampVoltageSource(0, lead_node[0], voltSource, getVoltage(sim));
			} else {
				sim.stampVoltageSource(0, lead_node[0], voltSource);
			}
		}

		public override void step(Circuit sim) {
			if (waveform != WaveType.DC)
				sim.updateVoltageSource(0, lead_node[0], voltSource, getVoltage(sim));
		}

		public override bool leadIsGround(int n1) {
			return true;
		}

	}
}