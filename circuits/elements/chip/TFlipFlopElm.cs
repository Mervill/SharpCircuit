using System;
using System.Collections;
using System.Collections.Generic;

namespace Circuits {

	public class TFlipFlopElm : ChipElm {

		public int FLAG_RESET = 2;
		public int FLAG_SET = 4;
		public bool last_val;
		
		public TFlipFlopElm(CirSim s) : base(s) {

		}
		
		public bool hasReset() {
			return (flags & FLAG_RESET) != 0 || hasSet();
		}

		public bool hasSet() {
			return (flags & FLAG_SET) != 0;
		}
		
		public override String getChipName() {
			return "T flip-flop";
		}

		public override void setupPins() {
			pins = new Pin[getLeadCount()];
			pins[0] = new Pin("T");
			pins[1] = new Pin("Q");
			pins[1].output = true;
			pins[2] = new Pin("Q");
			pins[2].output = true;
			pins[2].lineOver = true;
			pins[3] = new Pin("");
			pins[3].clock = true;
			if (!hasSet()) {
				if (hasReset()) {
					pins[4] = new Pin("R");
				}
			} else {
				pins[5] = new Pin("S");
				pins[4] = new Pin("R");
			}
		}

		public override int getLeadCount() {
			return 4 + (hasReset() ? 1 : 0) + (hasSet() ? 1 : 0);
		}

		public override int getVoltageSourceCount() {
			return 2;
		}

		public override void reset() {
			base.reset();
			volts[2] = 5;
			pins[2].value = true;
		}

		public override void execute() {
			if (pins[3].value && !lastClock) {
				if (pins[0].value) // if T = 1
				{
					pins[1].value = !last_val;
					pins[2].value = !pins[1].value;
					last_val = !last_val;
				}
				// else no change

			}
			if (hasSet() && pins[5].value) {
				pins[1].value = true;
				pins[2].value = false;
			}
			if (hasReset() && pins[4].value) {
				pins[1].value = false;
				pins[2].value = true;
			}
			lastClock = pins[3].value;
		}

		/*public EditInfo getEditInfo(int n) {
			if (n == 2) {
				EditInfo ei = new EditInfo("", 0, -1, -1);
				ei.checkbox = new Checkbox("Reset Pin", hasReset());
				return ei;
			}
			if (n == 3) {
				EditInfo ei = new EditInfo("", 0, -1, -1);
				ei.checkbox = new Checkbox("Set Pin", hasSet());
				return ei;
			}
			return super.getEditInfo(n);
		}

		public void setEditValue(int n, EditInfo ei) {
			if (n == 2) {
				if (ei.checkbox.getState()) {
					flags |= FLAG_RESET;
				} else {
					flags &= ~FLAG_RESET | FLAG_SET;
				}
				setupPins();
				allocNodes();
				setPoints();
			}
			if (n == 3) {
				if (ei.checkbox.getState()) {
					flags |= FLAG_SET;
				} else {
					flags &= ~FLAG_SET;
				}
				setupPins();
				allocNodes();
				setPoints();
			}
			super.setEditValue(n, ei);
		}*/
	}
}