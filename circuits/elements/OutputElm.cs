using System;
using System.Collections;
using System.Collections.Generic;

namespace Circuits {

	public class OutputElm : CircuitElement {

		public OutputElm( CirSim s) : base(s) {

		}

		public override int getLeadCount() {
			return 1;
		}

		public override double getVoltageDiff() {
			return volts[0];
		}

		public override void getInfo(String[] arr) {
			arr[0] = "output";
			arr[1] = "V = " + getVoltageText(volts[0]);
		}

		/*public EditInfo getEditInfo(int n) {
			if (n == 0) {
				EditInfo ei = new EditInfo("", 0, -1, -1);
				ei.checkbox = new Checkbox("Show Voltage",
						(flags & FLAG_VALUE) != 0);
				return ei;
			}
			return null;
		}

		public void setEditValue(int n, EditInfo ei) {
			if (n == 0) {
				flags = (ei.checkbox.getState()) ? (flags | FLAG_VALUE)
						: (flags & ~FLAG_VALUE);
			}
		}*/
	}
}