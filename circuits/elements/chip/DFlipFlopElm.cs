using System;
using System.Collections;
using System.Collections.Generic;

namespace Circuits {

	public class DFlipFlopElm : ChipElm {
		public int FLAG_RESET = 2;
		public int FLAG_SET = 4;

		public DFlipFlopElm(int xx, int yy, CirSim s) : base(xx, yy, s) {
			
		}

		public bool hasReset() {
			return (flags & FLAG_RESET) != 0 || hasSet();
		}

		public bool hasSet() {
			return (flags & FLAG_SET) != 0;
		}
		
		public override String getChipName() {
			return "D flip-flop";
		}

		public override void setupPins() {
			sizeX = 2;
			sizeY = 3;
			pins = new Pin[getPostCount()];
			pins[0] = new Pin(0, SIDE_W, "D", this);
			pins[1] = new Pin(0, SIDE_E, "Q", this);
			pins[1].output = pins[1].state = true;
			pins[2] = new Pin(hasSet() ? 1 : 2, SIDE_E, "Q", this);
			pins[2].output = true;
			pins[2].lineOver = true;
			pins[3] = new Pin(1, SIDE_W, "", this);
			pins[3].clock = true;
			if (!hasSet()) {
				if (hasReset()) {
					pins[4] = new Pin(2, SIDE_W, "R", this);
				}
			} else {
				pins[5] = new Pin(2, SIDE_W, "S", this);
				pins[4] = new Pin(2, SIDE_E, "R", this);
			}
		}

		public override int getPostCount() {
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
				pins[1].value = pins[0].value;
				pins[2].value = !pins[0].value;
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