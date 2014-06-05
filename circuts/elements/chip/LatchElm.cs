using System;
using System.Collections;
using System.Collections.Generic;

namespace Circuts {

	public class LatchElm : ChipElm {
	
		public LatchElm(int xx, int yy, CirSim s) : base(xx, yy, s) {
			
		}

		public override String getChipName() {
			return "Latch";
		}

		public override bool needsBits() {
			return true;
		}

		public int loadPin;

		public override void setupPins() {
			sizeX = 2;
			sizeY = bits + 1;
			pins = new Pin[getPostCount()];
			int i;
			for (i = 0; i != bits; i++) {
				pins[i] = new Pin(bits - 1 - i, SIDE_W, "I" + i, this);
			}
			for (i = 0; i != bits; i++) {
				pins[i + bits] = new Pin(bits - 1 - i, SIDE_E, "O", this);
				pins[i + bits].output = true;
			}
			pins[loadPin = bits * 2] = new Pin(bits, SIDE_W, "Ld", this);
			allocNodes();
		}

		public bool lastLoad = false;

		public override void execute() {
			int i;
			if (pins[loadPin].value && !lastLoad) {
				for (i = 0; i != bits; i++) {
					pins[i + bits].value = pins[i].value;
				}
			}
			lastLoad = pins[loadPin].value;
		}

		public override int getVoltageSourceCount() {
			return bits;
		}

		public override int getPostCount() {
			return bits * 2 + 1;
		}

	}
}