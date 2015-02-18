using System;
using System.Collections;
using System.Collections.Generic;

namespace SharpCircuit {

	public class HalfAdderElm : ChipElm {

		public Circuit.Lead leadOut1 { get { return lead1; } } // Grrr...
		public Circuit.Lead leadOut2 { get { return lead0; } }
		public Circuit.Lead leadIn0 { get { return new Circuit.Lead(this, 2); } }
		public Circuit.Lead leadIn1 { get { return new Circuit.Lead(this, 3); } }

		public override String getChipName() {
			return "Half Adder";
		}

		public bool hasReset() {
			return false;
		}

		public override void setupPins() {
			pins = new Pin[getLeadCount()];
			pins[0] = new Pin("Out1"); // S
			pins[0].output = true;
			pins[1] = new Pin("Out2"); // C
			pins[1].output = true;
			pins[2] = new Pin("In0");
			pins[3] = new Pin("In1");
		}

		public override int getLeadCount() {
			return 4;
		}

		public override int getVoltageSourceCount() {
			return 2;
		}

		public override void execute(Circuit sim) {
			pins[0].value = pins[2].value ^ pins[3].value;
			pins[1].value = pins[2].value && pins[3].value;
		}

	}
}