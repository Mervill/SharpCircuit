using System;
using System.Collections;
using System.Collections.Generic;

namespace Circuits {
	// contributed by Edward Calver

	public class DeMultiplexerElm : ChipElm {
		
		public DeMultiplexerElm(int xx, int yy, CirSim s) : base(xx, yy, s) {

		}
		
		bool hasReset() {
			return false;
		}
		
		public override String getChipName() {
			return "Multiplexer";
		}

		public override void setupPins() {
			pins = new Pin[getLeadCount()];

			pins[0] = new Pin("Q0");
			pins[0].output = true;
			pins[1] = new Pin("Q1");
			pins[1].output = true;
			pins[2] = new Pin("Q2");
			pins[2].output = true;
			pins[3] = new Pin("Q3");
			pins[3].output = true;

			pins[4] = new Pin("S0");
			pins[5] = new Pin("S1");

			pins[6] = new Pin("Q");

		}

		public override int getLeadCount() {
			return 7;
		}

		public override int getVoltageSourceCount() {
			return 4;
		}

		public override void execute() {
			int selectedvalue = 0;
			if (pins[4].value) {
				selectedvalue++;
			}
			if (pins[5].value) {
				selectedvalue += 2;
			}
			for (int i = 0; i < 4; i++) {
				pins[i].value = false;
			}
			pins[selectedvalue].value = pins[6].value;

		}

	}
}