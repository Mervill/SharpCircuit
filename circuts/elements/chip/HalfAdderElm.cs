using System;
using System.Collections;
using System.Collections.Generic;

namespace Circuts {

	public class HalfAdderElm : ChipElm {
		
		public HalfAdderElm(int xx, int yy, CirSim s) : base(xx, yy, s) {
			
		}
		
		public bool hasReset() {
			return false;
		}
		
		public override String getChipName() {
			return "Half Adder";
		}

		public override void setupPins() {
			sizeX = 2;
			sizeY = 2;
			pins = new Pin[getPostCount()];

			pins[0] = new Pin(0, SIDE_E, "S", this);
			pins[0].output = true;
			pins[1] = new Pin(1, SIDE_E, "C", this);
			pins[1].output = true;
			pins[2] = new Pin(0, SIDE_W, "A", this);
			pins[3] = new Pin(1, SIDE_W, "B", this);

		}

		public override int getPostCount() {
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