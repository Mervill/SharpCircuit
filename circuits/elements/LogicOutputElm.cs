using System;
using System.Collections;
using System.Collections.Generic;

namespace Circuits {

	// Initializers	[X]
	// Properties	[X]
	// Leads		[_]
	// Test Basic	[_]
	// Test Prop	[_]
	public class LogicOutputElm : CircuitElement {

		/// <summary>
		/// The Threshold Voltage.
		/// </summary>
		public double threshold{ get; set; }

		public bool needsPullDown { get; set; }

		public LogicOutputElm( CirSim s) : base(s) {
			threshold = 2.5;
		}

		public override int getLeadCount() {
			return 1;
		}
		
		public override void stamp() {
			if(needsPullDown)
				sim.stampResistor(nodes[0], 0, 1E6);
		}

		public override double getVoltageDiff() {
			return volts[0];
		}

		public override void getInfo(String[] arr) {
			arr[0] = "logic output";
			arr[1] = (volts[0] < threshold) ? "low" : "high";
			arr[2] = "V = " + getVoltageText(volts[0]);
		}

	}
}