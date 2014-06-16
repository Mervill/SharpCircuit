using System;
using System.Collections;
using System.Collections.Generic;

namespace Circuits {

	// Contributed by Edward Calver

	public class MultiplexerElm : ChipElm {
			
		public MultiplexerElm(CirSim s) : base(s) {

		}

		bool hasReset() {
			return false;
		}
		
		public override String getChipName() {
			return "Multiplexer";
		}

		public override void setupPins() {
			pins = new Pin[getLeadCount()];

			pins[0] = new Pin("I0");
			pins[1] = new Pin("I1");
			pins[2] = new Pin("I2");
			pins[3] = new Pin("I3");

			pins[4] = new Pin("S0");
			pins[5] = new Pin("S1");

			pins[6] = new Pin("Q");
			pins[6].output = true;

		}

		public override int getLeadCount() {
			return 7;
		}

		public override int getVoltageSourceCount() {
			return 1;
		}

		public override void execute() {
			int selectedvalue = 0;
			if (pins[4].value) {
				selectedvalue++;
			}
			if (pins[5].value) {
				selectedvalue += 2;
			}
			pins[6].value = pins[selectedvalue].value;

		}

	}
}