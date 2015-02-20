using System;
using System.Collections;
using System.Collections.Generic;

namespace SharpCircuit {

	public class PotentElm : CircuitElement {

		public Circuit.Lead leadOut { get { return lead0; } }
		public Circuit.Lead leadIn { get { return lead0; } }
		public Circuit.Lead leadVoltage { get { return new Circuit.Lead(this, 2); } }

		public double position { get; set; }

		/// <summary>
		/// Resistance (ohms)
		/// </summary>
		public double maxResistance { get; set; }

		private double resistance1;
		private double resistance2;
		private double current1;
		private double current2;
		private double current3;

		public PotentElm() : base() {
			maxResistance = 1000;
			position = 0.5;
		}

		public override int getLeadCount() {
			return 3;
		}

		public override void calculateCurrent() {
			current1 = (lead_volt[0] - lead_volt[2]) / resistance1;
			current2 = (lead_volt[1] - lead_volt[2]) / resistance2;
			current3 = -current1 - current2;
		}

		public override void stamp(Circuit sim) {
			resistance1 = maxResistance * position;
			resistance2 = maxResistance * (1 - position);
			sim.stampResistor(lead_node[0], lead_node[2], resistance1);
			sim.stampResistor(lead_node[2], lead_node[1], resistance2);
		}

		/*public override void getInfo(String[] arr) {
			arr[0] = "potentiometer";
			//arr[1] = "Vd = " + getVoltageDText(getVoltageDiff());
			arr[2] = "R1 = " + getUnitText(resistance1, Circuit.ohmString);
			arr[3] = "R2 = " + getUnitText(resistance2, Circuit.ohmString);
			arr[4] = "I1 = " + getCurrentDText(current1);
			arr[5] = "I2 = " + getCurrentDText(current2);
		}*/

	}
}