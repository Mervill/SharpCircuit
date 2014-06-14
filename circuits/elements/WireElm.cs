using System;
using System.Collections;
using System.Collections.Generic;

namespace Circuits {

	// Initializers	[X]
	// Properties	[X]
	// Leads		[X]
	// Test Basic	[X]
	// Test Prop	[X]
	//
	// Format		[_]
	// Flags		[_]
	// Prop:Contd	[_]
	public class WireElm : CircuitElement {

		public WireElm(CirSim s) : base(s) {
			
		}

		public override void stamp() {
			sim.stampVoltageSource(nodes[0], nodes[1], voltSource, 0);
		}

		public override int getVoltageSourceCount() {
			return 1;
		}

		public override void getInfo(String[] arr) {
			arr[0] = "wire";
			arr[1] = "I = " + getCurrentDText(getCurrent());
			arr[2] = "V = " + getVoltageText(volts[0]);
		}

		public override double getPower() {
			return 0;
		}

		public override double getVoltageDiff() {
			return volts[0];
		}

		public override bool isWire() {
			return true;
		}

	}
}