using System;
using System.Collections;
using System.Collections.Generic;

namespace Circuits {

	// Initializers	[X]
	// Properties	[X]
	// Leads		[X]
	// Test Basic	[X]
	// Test Prop	[X]
	public class ResistorElm : CircuitElement {

		public ElementLead leadIn 	{ get { return leads[0]; }}
		public ElementLead leadOut 	{ get { return leads[1]; }}

		/// <summary>
		/// Resistance (ohms)
		/// </summary>
		public double resistance{ get; set; }

		public ResistorElm(CirSim s) : base(s) {
			resistance = 100;
		}

		public ResistorElm(CirSim s,double r) : base(s) {
			resistance = r;
		}

		public override void calculateCurrent() {
			current = (volts[0] - volts[1]) / resistance;
			// System.out.print(this + " res current set to " + current + "\n");
		}

		public override void stamp() {
			sim.stampResistor(nodes[0], nodes[1], resistance);
		}

		public override void getInfo(String[] arr) {
			arr[0] = "resistor";
			getBasicInfo(arr);
			arr[3] = "R = " + getUnitText(resistance, CirSim.ohmString);
			arr[4] = "P = " + getUnitText(getPower(), "W");
		}
		
	}
}