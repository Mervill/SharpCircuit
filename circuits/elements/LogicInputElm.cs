using System;
using System.Collections;
using System.Collections.Generic;

namespace Circuits {
	
	public class LogicInputElm : SwitchElm {

		public double highVoltage 	{ get; set; } 
		public double lowVoltage 	{ get; set; }
		public bool isTernary		{ get; set; }
		public bool isNumeric 		{ get; set; }

		public LogicInputElm(CirSim s) : base(s) {
			highVoltage = 5;
			lowVoltage = 0;
		}

		public override int getLeadCount() {
			return 1;
		}
		
		public override void setCurrent(int vs, double c) {
			current = -c;
		}

		public override void stamp() {
			double v = (position == 0) ? lowVoltage : highVoltage;
			if (isTernary)
				v = position * 2.5;
			sim.stampVoltageSource(0, nodes[0], voltSource, v);
		}

		public override int getVoltageSourceCount() {
			return 1;
		}

		public override double getVoltageDiff() {
			return volts[0];
		}

		public override void getInfo(String[] arr) {
			arr[0] = "logic input";
			arr[1] = (position == 0) ? "low" : "high";
			if (isNumeric)
				arr[1] = "" + position;
			arr[1] += " (" + getVoltageText(volts[0]) + ")";
			arr[2] = "I = " + getCurrentText(getCurrent());
		}

		public override bool hasGroundConnection(int n1) {
			return true;
		}
		
		/*public EditInfo getEditInfo(int n) {
			if (n == 0) {
				EditInfo ei = new EditInfo("", 0, 0, 0);
				ei.checkbox = new Checkbox("Momentary Switch", momentary);
				return ei;
			}
			if (n == 1)
				return new EditInfo("High Voltage", hiV, 10, -10);
			if (n == 2)
				return new EditInfo("Low Voltage", loV, 10, -10);
			return null;
		}


		public void setEditValue(int n, EditInfo ei) {
			if (n == 0)
				momentary = ei.checkbox.getState();
			if (n == 1)
				hiV = ei.value;
			if (n == 2)
				loV = ei.value;
		}*/

	}
}