using System;
using System.Collections;
using System.Collections.Generic;

namespace Circuits {

	public class FullAdderElm : ChipElm {
		
		public FullAdderElm(int xx, int yy, CirSim s) : base(xx, yy, s) {

		}

		public override String getChipName() {
			return "Full Adder";
		}

		bool hasReset() {
			return false;
		}
		
		public override void setupPins() {
			pins = new Pin[getLeadCount()];

			pins[0] = new Pin("S");
			pins[0].output = true;
			pins[1] = new Pin("C");
			pins[1].output = true;
			pins[2] = new Pin("A");
			pins[3] = new Pin("B");
			pins[4] = new Pin("Cin");

		}

		public override int getLeadCount() {
			return 5;
		}

		public override int getVoltageSourceCount() {
			return 2;
		}

		public override void execute() {
			pins[0].value = (pins[2].value ^ pins[3].value) ^ pins[4].value;
			pins[1].value = (pins[2].value && pins[3].value) || (pins[2].value && pins[4].value) || (pins[3].value && pins[4].value);
		}

	}
}