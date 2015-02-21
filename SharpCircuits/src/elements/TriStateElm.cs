using System;
using System.Collections;
using System.Collections.Generic;

namespace SharpCircuit {

	// Contributed by Edward Calver.

	public class TriStateElm : CircuitElement {

		/// <summary>
		/// Resistance
		/// </summary>
		public double resistance { get; private set; }

		/// <summary>
		/// On Resistance (ohms)
		/// </summary>
		public double r_on { get; set; }

		/// <summary>
		/// Off Resistance (ohms)
		/// </summary>
		public double r_off { get; set; }

		/// <summary>
		/// <c>true</c> if buffer open; otherwise, <c>false</c>
		/// </summary>
		public bool open { get; private set; }

		//public ElementLead lead2;
		//public ElementLead lead3;

		public TriStateElm() : base() {
			//lead2 = new ElementLead(this,2);
			//lead3 = new ElementLead(this,3);
			r_on = 0.1;
			r_off = 1e10;
		}

		public override void calculateCurrent() {
			current = (lead_volt[0] - lead_volt[1]) / resistance;
		}

		// we need this to be able to change the matrix for each step
		public override bool nonLinear() { return true; }

		public override void stamp(Circuit sim) {
			sim.stampVoltageSource(0, lead_node[3], voltSource);
			sim.stampNonLinear(lead_node[3]);
			sim.stampNonLinear(lead_node[1]);
		}

		public override void step(Circuit sim) {
			open = (lead_volt[2] < 2.5);
			resistance = (open) ? r_off : r_on;
			sim.stampResistor(lead_node[3], lead_node[1], resistance);
			sim.updateVoltageSource(0, lead_node[3], voltSource, lead_volt[0] > 2.5 ? 5 : 0);
		}

		public override int getLeadCount() {
			return 4;
		}

		public override int getVoltageSourceCount() {
			return 1;
		}

		/*public override void getInfo(String[] arr) {
			arr[0] = "tri-state buffer";
			arr[1] = open ? "open" : "closed";
			arr[2] = "Vd = " + getVoltageDText(getVoltageDiff());
			arr[3] = "I = " + getCurrentDText(current);
			arr[4] = "Vc = " + getVoltageText(lead_volt[2]);
		}*/

		// we have to just assume current will flow either way, even though that
		// might cause singular matrix errors

		// 0---3----------1
		// /
		// 2

		public override bool leadsAreConnected(int n1, int n2) {
			return (n1 == 1 && n2 == 3) || (n1 == 3 && n2 == 1);
		}
	}
}