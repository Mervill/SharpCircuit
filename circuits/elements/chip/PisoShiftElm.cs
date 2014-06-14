using System;
using System.Collections;
using System.Collections.Generic;

namespace Circuits {

	// Contributed by Edward Calver

	public class PisoShiftElm : ChipElm {

		private short data = 0;// Lack of unsigned types sucks
		private bool clockstate = false;
		private bool modestate = false;

		public PisoShiftElm(CirSim s) : base(s) {

		}
		
		bool hasReset() {
			return false;
		}

		public override String getChipName() {
			return "PISO shift register";
		}

		public override void setupPins() {
			pins = new Pin[getLeadCount()];

			pins[0] = new Pin("L");
			pins[1] = new Pin("");
			pins[1].clock = true;

			pins[2] = new Pin("I7");
			pins[3] = new Pin("I6");
			pins[4] = new Pin("I5");
			pins[5] = new Pin("I4");
			pins[6] = new Pin("I3");
			pins[7] = new Pin("I2");
			pins[8] = new Pin("I1");
			pins[9] = new Pin("I0");

			pins[10] = new Pin("Q");
			pins[10].output = true;

		}

		public override int getLeadCount() {
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

	}
}