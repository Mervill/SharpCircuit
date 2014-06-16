using System;
using System.Collections;
using System.Collections.Generic;

namespace Circuits {

	// Initializers	[X]
	// Properties	[X]
	// Leads		[X]
	// Test Basic	[_]
	// Test Prop	[_]
	public class OutputElm : CircuitElement {

		public ElementLead leadOut {
			get {
				return leads[0];
			}
		}

		public OutputElm(CirSim s) : base(s) {

		}

		public override int getLeadCount() {
			return 1;
		}

		public override double getVoltageDiff() {
			return volts[0];
		}

		public override void getInfo(String[] arr) {
			arr[0] = "output";
			arr[1] = "V = " + getVoltageText(volts[0]);
		}
		
	}
}