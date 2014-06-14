using System;
using System.Collections;
using System.Collections.Generic;

namespace Circuits {

	// Initializers	[X]
	// Properties	[X]
	// Leads		[_]
	// Test Basic	[_]
	// Test Prop	[_]
	public class AnalogSwitchElm : CircuitElement {

		/// <summary>
		/// Normally closed
		/// </summary>
		public static readonly int FLAG_INVERT = 1;

		/// <summary>
		/// On Resistance (ohms)
		/// </summary>
		public double r_on{ get; set; }

		/// <summary>
		/// Off Resistance (ohms)
		/// </summary>
		public double r_off{ get; set; }

		public bool open{ get; protected set; }

		public ElementLead lead3;

		private double resistance;

		public AnalogSwitchElm(CirSim s) : base(s) {
			r_on = 20;
			r_off = 1E10;
		}

		public override void calculateCurrent() {
			current = (volts[0] - volts[1]) / resistance;
		}

		// we need this to be able to change the matrix for each step
		public override bool nonLinear() {
			return true;
		}

		public override void stamp() {
			sim.stampNonLinear(nodes[0]);
			sim.stampNonLinear(nodes[1]);
		}

		public override void doStep() {
			open = (volts[2] < 2.5);
			if ((flags & FLAG_INVERT) != 0) {
				open = !open;
			}
			resistance = (open) ? r_off : r_on;
			sim.stampResistor(nodes[0], nodes[1], resistance);
		}

		public override int getLeadCount() {
			return 3;
		}

		public override ElementLead getLead(int n) {
			return (n == 0) ? lead0 : (n == 1) ? lead1 : lead3;
		}

		public override void getInfo(String[] arr) {
			arr[0] = "analog switch";
			arr[1] = open ? "open" : "closed";
			arr[2] = "Vd = " + getVoltageDText(getVoltageDiff());
			arr[3] = "I = " + getCurrentDText(getCurrent());
			arr[4] = "Vc = " + getVoltageText(volts[2]);
		}

		// we have to just assume current will flow either way, even though that
		// might cause singular matrix errors
		public override bool getConnection(int n1, int n2) {
			if (n1 == 2 || n2 == 2) {
				return false;
			}
			return true;
		}

	}
}