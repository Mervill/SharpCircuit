using System;
using System.Collections;
using System.Collections.Generic;

namespace Circuits {

	// Initializers	[X]
	// Properties	[X]
	// Leads		[X]
	// Test Basic	[_]
	// Test Prop	[_]
	public class CurrentElm : CircuitElement {

		/// <summary>
		/// Current (A)
		/// </summary>
		public double currentValue{ get; set; }

		public CurrentElm(CirSim s) : base(s) {
			currentValue = 0.01;
		}

		public override void stamp() {
			current = currentValue;
			sim.stampCurrentSource(nodes[0], nodes[1], current);
		}

		public override void getInfo(String[] arr) {
			arr[0] = "current source";
			getBasicInfo(arr);
		}

		public override double getVoltageDiff() {
			return volts[1] - volts[0];
		}
	}
}