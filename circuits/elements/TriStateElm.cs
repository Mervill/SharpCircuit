using System;
using System.Collections;
using System.Collections.Generic;

namespace Circuits {

	// contributed by Edward Calver

	public class TriStateElm : CircuitElement {

		/// <summary>
		/// Resistance
		/// </summary>
		public double Resistance{ get; private set; }

		/// <summary>
		/// On Resistance (ohms)
		/// </summary>
		public double ROn{ get; set; }

		/// <summary>
		/// Off Resistance (ohms)
		/// </summary>
		public double ROff{ get; set; }

		/// <summary>
		/// <c>true</c> if buffer open; otherwise, <c>false</c>
		/// </summary>
		public bool Open{ get; private set; }

		public ElementLead lead2;
		public ElementLead lead3;

		public TriStateElm( CirSim s) : base(s) {
			lead2 = new ElementLead(this,2);
			lead3 = new ElementLead(this,3);
			ROn = 0.1;
			ROff = 1e10;
		}

		public override void calculateCurrent() {
			current = (volts[0] - volts[1]) / Resistance;
		}

		// we need this to be able to change the matrix for each step
		public override bool nonLinear() {
			return true;
		}

		public override void stamp() {
			sim.stampVoltageSource(0, nodes[3], voltSource);
			sim.stampNonLinear(nodes[3]);
			sim.stampNonLinear(nodes[1]);
		}

		public override void doStep() {
			Open = (volts[2] < 2.5);
			Resistance = (Open) ? ROff : ROn;
			sim.stampResistor(nodes[3], nodes[1], Resistance);
			sim.updateVoltageSource(0, nodes[3], voltSource, volts[0] > 2.5 ? 5 : 0);
		}

		public override int getLeadCount() {
			return 4;
		}

		public override int getVoltageSourceCount() {
			return 1;
		}

		public override ElementLead getLead(int n) {
			return (n == 0) ? lead0 : (n == 1) ? lead1 : (n == 2) ? lead2 : lead3;
		}

		public override void getInfo(String[] arr) {
			arr[0] = "tri-state buffer";
			arr[1] = Open ? "open" : "closed";
			arr[2] = "Vd = " + getVoltageDText(getVoltageDiff());
			arr[3] = "I = " + getCurrentDText(getCurrent());
			arr[4] = "Vc = " + getVoltageText(volts[2]);
		}

		// we have to just assume current will flow either way, even though that
		// might cause singular matrix errors

		// 0---3----------1
		// /
		// 2

		public override bool getConnection(int n1, int n2) {
			if ((n1 == 1 && n2 == 3) || (n1 == 3 && n2 == 1)) {
				return true;
			}
			return false;
		}
	}
}