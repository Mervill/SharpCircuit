using System;
using System.Collections;
using System.Collections.Generic;

namespace Circuts {

	// contributed by Edward Calver

	public class PisoShiftElm : ChipElm {
		
		public PisoShiftElm(int xx, int yy, CirSim s) : base(xx, yy, s) {

		}
		
		bool hasReset() {
			return false;
		}
		
		public short data = 0;// Lack of unsigned types sucks
		public bool clockstate = false;
		public bool modestate = false;

		public override String getChipName() {
			return "PISO shift register";
		}

		public override void setupPins() {
			sizeX = 10;
			sizeY = 3;
			pins = new Pin[getPostCount()];

			pins[0] = new Pin(1, SIDE_W, "L", this);
			pins[1] = new Pin(2, SIDE_W, "", this);
			pins[1].clock = true;

			pins[2] = new Pin(1, SIDE_N, "I7", this);
			pins[3] = new Pin(2, SIDE_N, "I6", this);
			pins[4] = new Pin(3, SIDE_N, "I5", this);
			pins[5] = new Pin(4, SIDE_N, "I4", this);
			pins[6] = new Pin(5, SIDE_N, "I3", this);
			pins[7] = new Pin(6, SIDE_N, "I2", this);
			pins[8] = new Pin(7, SIDE_N, "I1", this);
			pins[9] = new Pin(8, SIDE_N, "I0", this);

			pins[10] = new Pin(1, SIDE_E, "Q", this);
			pins[10].output = true;

		}

		public override int getPostCount() {
			return 11;
		}

		public override int getVoltageSourceCount() {
			return 1;
		}

		public override void execute() {
			if (pins[0].value && !modestate) {
				modestate = true;
				data = 0;
				if (pins[2].value) {
					data += 128;
				}
				if (pins[3].value) {
					data += 64;
				}
				if (pins[4].value) {
					data += 32;
				}
				if (pins[5].value) {
					data += 16;
				}
				if (pins[6].value) {
					data += 8;
				}
				if (pins[7].value) {
					data += 4;
				}
				if (pins[8].value) {
					data += 2;
				}
				if (pins[9].value) {
					data += 1;
				}
			} else if (pins[1].value && !clockstate) {
				clockstate = true;
				if ((data & 1) == 0) {
					pins[10].value = false;
				} else {
					pins[10].value = true;
				}
				data = (byte) (data >> 1);
			}
			if (!pins[0].value) {
				modestate = false;
			}
			if (!pins[1].value) {
				clockstate = false;
			}
		}

		public override int getDumpType() {
			return 186;
		}

	}
}