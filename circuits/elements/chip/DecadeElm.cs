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
			sizeX = bits > 2 ? bits : 2;
			sizeY = 2;
			pins = new Pin[getPostCount()];
			pins[0] = new Pin(1, SIDE_W, "", this);
			pins[0].clock = true;
			pins[1] = new Pin(sizeX - 1, SIDE_S, "R", this);
			pins[1].bubble = true;
			int i;
			for (i = 0; i != bits; i++) {
				int ii = i + 2;
				pins[ii] = new Pin(i, SIDE_N, "Q" + i, this);
				pins[ii].output = pins[ii].state = true;
			}
			allocNodes();
		}

		public override int getPostCount() {
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