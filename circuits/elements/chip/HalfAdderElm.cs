using System;
using System.Collections;
using System.Collections.Generic;

namespace Circuits {

	public class HalfAdderElm : ChipElm {
		
		public HalfAdderElm(CirSim s) : base(s) {
			
		}
		
		public bool hasReset() {
			return false;
		}
		
		public override String getChipName() {
			return "Half Adder";
		}

		public override void setupPins() {
			pins = new Pin[getLeadCount()];

			pins[0] = new Pin("S");
			pins[0].output = true;
			pins[1] = new Pin("C");
			pins[1].output = true;
			pins[2] = new Pin("A");
			pins[3] = new Pin("B");
		}

		public override int getLeadCount() {
			return 4;
		}

		public override int getVoltageSourceCount() {
			return 2;
		}

		public override void execute() {
			pins[0].value = pins[2].value ^ pins[3].value;
			pins[1].value = pins[2].value && pins[3].value;
		}

	}
}