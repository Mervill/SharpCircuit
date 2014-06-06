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
			sizeX = 2;
			sizeY = 3;
			pins = new Pin[getPostCount()];

			pins[0] = new Pin(2, SIDE_E, "S", this);
			pins[0].output = true;
			pins[1] = new Pin(0, SIDE_E, "C", this);
			pins[1].output = true;
			pins[2] = new Pin(0, SIDE_W, "A", this);
			pins[3] = new Pin(1, SIDE_W, "B", this);
			pins[4] = new Pin(2, SIDE_W, "Cin", this);

		}

		public override int getPostCount() {
			return 5;
		}

		public override int getVoltageSourceCount() {
			return 2;
		}

		public override void execute() {
			pins[0].value = (pins[2].value ^ pins[3].value) ^ pins[4].value;
			pins[1].value = (pins[2].value && pins[3].value)
					|| (pins[2].value && pins[4].value)
					|| (pins[3].value && pins[4].value);
		}

	}
}