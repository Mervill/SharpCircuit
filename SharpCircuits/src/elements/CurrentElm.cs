using System;
using System.Collections;
using System.Collections.Generic;

namespace SharpCircuit {
	
	public class CurrentElm : CircuitElement {

		//public ElementLead leadIn 	{ get { return lead0; }}
		//public ElementLead leadOut 	{ get { return lead1; }}

		/// <summary>
		/// Current (A)
		/// </summary>
		public double currentVoltage { get; set; }

		public CurrentElm() {
			currentVoltage = 0.01;
		}

		public override void stamp() {
			current = currentVoltage;
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