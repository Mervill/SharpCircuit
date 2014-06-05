using System;
using System.Collections;
using System.Collections.Generic;

namespace Circuts {

	// contributed by Edward Calver

	public class SipoShiftElm : ChipElm {
		
		public SipoShiftElm(int xx, int yy, CirSim s) : base(xx, yy, s) {
			
		}

		public short data = 0;	// This has to be a short because there's no unsigned byte
								// and it's screwing with my code
		public bool clockstate = false;
		
		bool hasReset() {
			return false;
		}
		
		public override String getChipName() {
			return "SIPO shift register";
		}

		public override void setupPins() {
			sizeX = 9;
			sizeY = 3;
			pins = new Pin[getPostCount()];

			pins[0] = new Pin(1, SIDE_W, "D", this);
			pins[1] = new Pin(2, SIDE_W, "", this);
			pins[1].clock = true;

			pins[2] = new Pin(1, SIDE_N, "I7", this);
			pins[2].output = true;
			pins[3] = new Pin(2, SIDE_N, "I6", this);
			pins[3].output = true;
			pins[4] = new Pin(3, SIDE_N, "I5", this);
			pins[4].output = true;
			pins[5] = new Pin(4, SIDE_N, "I4", this);
			pins[5].output = true;
			pins[6] = new Pin(5, SIDE_N, "I3", this);
			pins[6].output = true;
			pins[7] = new Pin(6, SIDE_N, "I2", this);
			pins[7].output = true;
			pins[8] = new Pin(7, SIDE_N, "I1", this);
			pins[8].output = true;
			pins[9] = new Pin(8, SIDE_N, "I0", this);
			pins[9].output = true;

		}

		public override int getPostCount() {
			return 10;
		}

		public override int getVoltageSourceCount() {
			return 8;
		}

		public override void execute() {

			if (pins[1].value && !clockstate) {
				clockstate = true;
				data = (short) (data >> 1);
				if (pins[0].value) {
					data += 128;
				}

				if ((data & 128) > 0) {
					pins[2].value = true;
				} else {
					pins[2].value = false;
				}
				if ((data & 64) > 0) {
					pins[3].value = true;
				} else {
					pins[3].value = false;
				}
				if ((data & 32) > 0) {
					pins[4].value = true;
				} else {
					pins[4].value = false;
				}
				if ((data & 16) > 0) {
					pins[5].value = true;
				} else {
					pins[5].value = false;
				}
				if ((data & 8) > 0) {
					pins[6].value = true;
				} else {
					pins[6].value = false;
				}
				if ((data & 4) > 0) {
					pins[7].value = true;
				} else {
					pins[7].value = false;
				}
				if ((data & 2) > 0) {
					pins[8].value = true;
				} else {
					pins[8].value = false;
				}
				if ((data & 1) > 0) {
					pins[9].value = true;
				} else {
					pins[9].value = false;
				}
			}
			if (!pins[1].value) {
				clockstate = false;
			}
		}

		public override int getDumpType() {
			return 189;
		}

	}
}