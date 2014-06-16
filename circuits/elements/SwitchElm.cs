using System;
using System.Collections;
using System.Collections.Generic;

namespace Circuits {

	// Initializers	[X]
	// Properties	[X]
	// Leads		[X]
	// Test Basic	[_]
	// Test Prop	[_]
	public class SwitchElm : CircuitElement {

		public ElementLead leadIn 	{ get { return leads[0]; }}
		public ElementLead leadOut 	{ get { return leads[1]; }}

		/// <summary>
		/// Momentary Switch TODO: Fixme
		/// </summary>
		public bool momentary{ get; private set; }

		// position 0 == closed, position 1 == open
		protected int position, posCount;

		public SwitchElm(CirSim s) : base(s) {
			momentary = false;
			position = 0;
			posCount = 2;
		}

		public SwitchElm(CirSim s,bool mm) : base(s) {
			position = (mm) ? 1 : 0;
			momentary = mm;
			posCount = 2;
		}

		public override void calculateCurrent() {
			if (position == 1) {
				current = 0;
			}
		}

		public override void stamp() {
			if (position == 0) {
				sim.stampVoltageSource(nodes[0], nodes[1], voltSource, 0);
			}
		}

		public override int getVoltageSourceCount() {
			return (position == 1) ? 0 : 1;
		}

		public virtual void toggle() {
			position++;
			if (position >= posCount) {
				position = 0;
			}
		}

		public override void getInfo(String[] arr) {
			arr[0] = (momentary) ? "push switch (SPST)" : "switch (SPST)";
			if (position == 1) {
				arr[1] = "open";
				arr[2] = "Vd = " + getVoltageDText(getVoltageDiff());
			} else {
				arr[1] = "closed";
				arr[2] = "V = " + getVoltageText(volts[0]);
				arr[3] = "I = " + getCurrentDText(getCurrent());
			}
		}

		public override bool getConnection(int n1, int n2) {
			return position == 0;
		}

		public override bool isWire() {
			return true;
		}

	}
}