using System;
using System.Collections;
using System.Collections.Generic;

namespace Circuits {

	// Initializers	[X]
	// Properties	[X]
	// Leads		[_]
	// Test Basic	[_]
	// Test Prop	[_]
	public class RailElm : VoltageElm {

		public static readonly int FLAG_CLOCK = 1;

		public RailElm(CirSim s,WaveType wf) : base(s,wf) {

		}

		public override int getLeadCount() {
			return 1;
		}

		public override double getVoltageDiff() {
			return volts[0];
		}

		public override void stamp() {
			if (waveform == WaveType.DC) {
				sim.stampVoltageSource(0, nodes[0], voltSource, getVoltage());
			} else {
				sim.stampVoltageSource(0, nodes[0], voltSource);
			}
		}

		public override void doStep() {
			if (waveform != WaveType.DC) {
				sim.updateVoltageSource(0, nodes[0], voltSource, getVoltage());
			}
		}

		public override bool hasGroundConnection(int n1) {
			return true;
		}

	}
}