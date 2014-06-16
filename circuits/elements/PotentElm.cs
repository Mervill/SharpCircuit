using System;
using System.Collections;
using System.Collections.Generic;

namespace Circuits {

	// Initializers	[X]
	// Properties	[X]
	// Leads		[X]
	// Test Basic	[_]
	// Test Prop	[_]
	public class PotentElm : CircuitElement {

		public ElementLead leadOut 		{ get { return leads[0]; }}
		public ElementLead leadIn1 		{ get { return leads[1]; }}
		public ElementLead leadVoltage 	{ get { return leads[2]; }}

		public double position{ get; set; }

		/// <summary>
		/// Resistance (ohms)
		/// </summary>
		public double maxResistance{ get; set; }

		private double resistance1;
		private double resistance2;
		private double current1; 
		private double current2;
		private double current3;

		public PotentElm(CirSim s) : base(s) {
			maxResistance = 1000;
			position = 0.5;
		}

		public override int getLeadCount() {
			return 3;
		}

		public override void calculateCurrent() {
			current1 = (volts[0] - volts[2]) / resistance1;
			current2 = (volts[1] - volts[2]) / resistance2;
			current3 = -current1 - current2;
		}

		public override void stamp() {
			resistance1 = maxResistance * position;
			resistance2 = maxResistance * (1 - position);
			sim.stampResistor(nodes[0], nodes[2], resistance1);
			sim.stampResistor(nodes[2], nodes[1], resistance2);
		}

		public override void getInfo(String[] arr) {
			arr[0] = "potentiometer";
			arr[1] = "Vd = " + getVoltageDText(getVoltageDiff());
			arr[2] = "R1 = " + getUnitText(resistance1, CirSim.ohmString);
			arr[3] = "R2 = " + getUnitText(resistance2, CirSim.ohmString);
			arr[4] = "I1 = " + getCurrentDText(current1);
			arr[5] = "I2 = " + getCurrentDText(current2);
		}

	}
}