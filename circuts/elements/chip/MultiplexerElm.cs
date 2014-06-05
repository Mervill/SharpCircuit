using System;
using System.Collections;
using System.Collections.Generic;

namespace Circuts {

	// contributed by Edward Calver

	public class MultiplexerElm : ChipElm {
			
		public MultiplexerElm(int xx, int yy, CirSim s) : base(xx, yy, s) {

		}

		bool hasReset() {
			return false;
		}
		
		public override String getChipName() {
			return "Multiplexer";
		}

		public override void setupPins() {
			sizeX = 3;
			sizeY = 5;
			pins = new Pin[getPostCount()];

			pins[0] = new Pin(0, SIDE_W, "I0", this);
			pins[1] = new Pin(1, SIDE_W, "I1", this);
			pins[2] = new Pin(2, SIDE_W, "I2", this);
			pins[3] = new Pin(3, SIDE_W, "I3", this);

			pins[4] = new Pin(1, SIDE_S, "S0", this);
			pins[5] = new Pin(2, SIDE_S, "S1", this);

			pins[6] = new Pin(0, SIDE_E, "Q", this);
			pins[6].output = true;

		}

		public override int getPostCount() {
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