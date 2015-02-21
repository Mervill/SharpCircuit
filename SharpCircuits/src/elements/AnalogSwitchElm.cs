using System;
using System.Collections;
using System.Collections.Generic;

namespace SharpCircuit {

	public class AnalogSwitchElm : CircuitElement {

		public Circuit.Lead leadIn { get { return lead0; } }
		public Circuit.Lead leadOut { get { return lead1; } }
		public Circuit.Lead leadSwitch { get { return new Circuit.Lead(this, 2); } }

		/// <summary>
		/// Normally closed
		/// </summary>
		public bool invert { get; set; }

		/// <summary>
		/// On Resistance (ohms)
		/// </summary>
		public double r_on { get; set; }

		/// <summary>
		/// Off Resistance (ohms)
		/// </summary>
		public double r_off { get; set; }

		public bool open { get; protected set; }

		private double resistance;

		public AnalogSwitchElm() : base() {
			r_on = 20;
			r_off = 1E10;
		}

		public override void calculateCurrent() {
			current = (lead_volt[0] - lead_volt[1]) / resistance;
		}

		// we need this to be able to change the matrix for each step
		public override bool nonLinear() { return true; }

		public override void stamp(Circuit sim) {
			sim.stampNonLinear(lead_node[0]);
			sim.stampNonLinear(lead_node[1]);
		}

		public override void step(Circuit sim) {
			open = (lead_volt[2] < 2.5);
			if(invert) open = !open;
			resistance = (open) ? r_off : r_on;
			sim.stampResistor(lead_node[0], lead_node[1], resistance);
		}

		public override int getLeadCount() {
			return 3;
		}

		/*public override void getInfo(String[] arr) {
			arr[0] = "analog switch";
			arr[1] = open ? "open" : "closed";
			arr[2] = "Vd = " + getVoltageDText(getVoltageDiff());
			arr[3] = "I = " + getCurrentDText(current);
			arr[4] = "Vc = " + getVoltageText(lead_volt[2]);
		}*/

		// we have to just assume current will flow either way, even though that
		// might cause singular matrix errors
		public override bool leadsAreConnected(int n1, int n2) {
			return !(n1 == 2 || n2 == 2);
		}

	}
}