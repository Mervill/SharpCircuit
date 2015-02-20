using System;
using System.Collections;
using System.Collections.Generic;

namespace SharpCircuit {

	public class CurrentSourceElm : CircuitElement {

		public Circuit.Lead leadIn { get { return lead0; } }
		public Circuit.Lead leadOut { get { return lead1; } }

		/// <summary>
		/// Current (A)
		/// </summary>
		public double sourceCurrent { get; set; } // Also voltage?!

		public CurrentSourceElm() : base() {
			sourceCurrent = 0.01;
		}

		public override void stamp(Circuit sim) {
			sim.stampCurrentSource(lead_node[0], lead_node[1], sourceCurrent);
		}

		/*public override void getInfo(String[] arr) {
			arr[0] = "current source";
			getBasicInfo(arr);
		}*/

		public override double getVoltageDelta() {
			return lead_volt[1] - lead_volt[0];
		}
	}
}