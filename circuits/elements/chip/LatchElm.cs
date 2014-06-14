using System;
using System.Collections;
using System.Collections.Generic;

namespace Circuits {

	public class LatchElm : ChipElm {

		public int loadPin;
		public bool lastLoad = false;

		public LatchElm(CirSim s) : base(s) {
			
		}

		public override String getChipName() {
			return "Latch";
		}

		public override bool needsBits() {
			return true;
		}

		public override void setupPins() {
			pins = new Pin[getLeadCount()];
			int i;
			for (i = 0; i != bits; i++) {
				pins[i] = new Pin("I"+i);
			}
			for (i = 0; i != bits; i++) {
				pins[i + bits] = new Pin("O"+i);
				pins[i + bits].output = true;
			}
			pins[loadPin = bits * 2] = new Pin("Ld");
			allocNodes();
		}

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

		public override int getLeadCount() {
			return bits * 2 + 1;
		}

	}
}