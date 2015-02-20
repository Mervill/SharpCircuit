using System;
using System.Collections;
using System.Collections.Generic;

namespace SharpCircuit {

	public class LogicOutputElm : CircuitElement {

		public Circuit.Lead leadIn { get { return lead0; } }

		/// <summary>
		/// The Threshold Voltage.
		/// </summary>
		public double threshold { get; set; }

		public bool needsPullDown { get; set; }

		public LogicOutputElm() : base() {
			threshold = 2.5;
		}

		public override int getLeadCount() {
			return 1;
		}

		public override void stamp(Circuit sim) {
			if(needsPullDown)
				sim.stampResistor(lead_node[0], 0, 1E6);
		}

		public override double getVoltageDelta() {
			return lead_volt[0];
		}

		public bool isHigh() {
			return (lead_volt[0] < threshold) ? false : true;
		}

		/*public override void getInfo(String[] arr) {
			arr[0] = "logic output";
			arr[1] = (lead_volt[0] < threshold) ? "low" : "high";
			arr[2] = "V = " + getVoltageText(lead_volt[0]);
		}*/

	}
}