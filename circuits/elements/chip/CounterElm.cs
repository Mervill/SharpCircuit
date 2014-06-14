using System;
using System.Collections;
using System.Collections.Generic;

namespace Circuits {

	public class CounterElm : ChipElm {

		public int FLAG_ENABLE = 2;
		public bool invertreset = false;

		public CounterElm(CirSim s) : base(s) {
			
		}

		public override bool needsBits() {
			return true;
		}

		public override String getChipName() {
			return "Counter";
		}

		public override void setupPins() {
			pins = new Pin[getLeadCount()];
			pins[0] = new Pin("");
			pins[0].clock = true;
			pins[1] = new Pin("R");
			int i;
			for (i = 0; i != bits; i++) {
				int ii = i + 2;
				pins[ii] = new Pin("Q" + (bits - i - 1));
				pins[ii].output = true;
			}
			if (hasEnable()) {
				pins[bits + 2] = new Pin("En");
			}
			allocNodes();
		}

		public override int getLeadCount() {
			if (hasEnable()) {
				return bits + 3;
			}
			return bits + 2;
		}

		/*public EditInfo getEditInfo(int n) {
			if (n == 0) {
				EditInfo ei = new EditInfo("", 0, -1, -1);
				ei.checkbox = new Checkbox("Flip X", (flags & FLAG_FLIP_X) != 0);
				return ei;
			}
			if (n == 1) {
				EditInfo ei = new EditInfo("", 0, -1, -1);
				ei.checkbox = new Checkbox("Flip Y", (flags & FLAG_FLIP_Y) != 0);
				return ei;
			}
			if (n == 2) {
				EditInfo ei = new EditInfo("", 0, -1, -1);
				ei.checkbox = new Checkbox("Invert reset pin", invertreset);
				return ei;
			}
			return null;
		}

		public void setEditValue(int n, EditInfo ei) {
			if (n == 0) {
				if (ei.checkbox.getState()) {
					flags |= FLAG_FLIP_X;
				} else {
					flags &= ~FLAG_FLIP_X;
				}
				setPoints();
			}
			if (n == 1) {
				if (ei.checkbox.getState()) {
					flags |= FLAG_FLIP_Y;
				} else {
					flags &= ~FLAG_FLIP_Y;
				}
				setPoints();
			}
			if (n == 2) {
				if (ei.checkbox.getState()) {
					invertreset = true;
					pins[1].bubble = true;
				} else {
					invertreset = false;
					pins[1].bubble = false;
				}
				setPoints();
			}
		}*/

		public bool hasEnable() {
			return (flags & FLAG_ENABLE) != 0;
		}

		public override int getVoltageSourceCount() {
			return bits;
		}

		public override void execute() {
			bool en = true;
			if (hasEnable()) {
				en = pins[bits + 2].value;
			}
			if (pins[0].value && !lastClock && en) {
				int i;
				for (i = bits - 1; i >= 0; i--) {
					int ii = i + 2;
					if (!pins[ii].value) {
						pins[ii].value = true;
						break;
					}
					pins[ii].value = false;
				}
			}
			if (!pins[1].value == invertreset) {
				int i;
				for (i = 0; i != bits; i++) {
					pins[i + 2].value = false;
				}
			}
			lastClock = pins[0].value;
		}
		
	}
}