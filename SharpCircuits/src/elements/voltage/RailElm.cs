using System;
using System.Collections;
using System.Collections.Generic;

namespace SharpCircuit {
	
	public class RailElm : VoltageElm {

		//public ElementLead leadOut 	{ get { return lead0; }}

		public RailElm() : base(WaveType.DC) {

		}

		public RailElm(WaveType wf) : base(wf) {

		}

		public override int getLeadCount() {
			return 1;
		}

		public override double getVoltageDiff() {
			return volts[0];
		}

		public override void stamp(CirSim sim) {
			if (waveform == WaveType.DC) {
				sim.stampVoltageSource(0, nodes[0], voltSource, getVoltage(sim));
			} else {
				sim.stampVoltageSource(0, nodes[0], voltSource);
			}
		}

		public override void doStep(CirSim sim) {
			if (waveform != WaveType.DC)
				sim.updateVoltageSource(0, nodes[0], voltSource, getVoltage(sim));
		}

		public override bool hasGroundConnection(int n1) {
			return true;
		}

	}
}