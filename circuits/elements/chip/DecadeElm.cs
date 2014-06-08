using System;
using System.Collections;
using System.Collections.Generic;

namespace Circuits {

	public class DecadeElm : ChipElm {
		
		public DecadeElm(int xx, int yy, CirSim s) : base(xx, yy, s) {

		}

		public override String getChipName() {
			return "decade counter";
		}

		public override bool needsBits() {
			return true;
		}

		public override void setupPins() {
			pins = new Pin[getLeadCount()];
			pins[0] = new Pin("");
			pins[0].clock = true;
			pins[1] = new Pin("R");
			int i;
			for (i = 0; i != bits; i++) {
				int ii = i + 2;
				pins[ii] = new Pin("Q" + i);
				pins[ii].output = true;
			}
			allocNodes();
		}

		public override int getLeadCount() {
			return bits + 2;
		}

		public override int getVoltageSourceCount() {
			return bits;
		}

		public override void execute() {
			int i;
			if (pins[0].value && !lastClock) {
				for (i = 0; i != bits; i++) {
					if (pins[i + 2].value) {
						break;
					}
				}
				if (i < bits) {
					pins[i++ + 2].value = false;
				}
				i %= bits;
				pins[i + 2].value = true;
			}
			if (!pins[1].value) {
				for (i = 1; i != bits; i++) {
					pins[i + 2].value = false;
				}
				pins[2].value = true;
			}
			lastClock = pins[0].value;
		}
		
	}
}