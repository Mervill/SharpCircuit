using System;
using System.Collections;
using System.Collections.Generic;

namespace Circuits {

	// Contributed by Edward Calver

	public class SRAMElm : ChipElm {
		
		public SRAMElm(CirSim s) : base(s) {
			short i;
			for (i = 0; i < 256; i++)
				data[i] = 0;
		}

		public override String getChipName() {
			return "SRAM";
		}

		// Fuck this lack of unsigned types. That's
		// twice as much data as I'd need in C
		public short[] data = new short[256];

		public override void setupPins() {
			pins = new Pin[19];
			pins[0] = new Pin("A7");
			pins[1] = new Pin("A6");
			pins[2] = new Pin("A5");
			pins[3] = new Pin("A4");
			pins[4] = new Pin("A3");
			pins[5] = new Pin("A2");
			pins[6] = new Pin("A1");
			pins[7] = new Pin("A0");
			pins[8] = new Pin("R");
			pins[9] = new Pin("W");

			pins[10] = new Pin("D7");
			pins[11] = new Pin("D6");
			pins[12] = new Pin("D5");
			pins[13] = new Pin("D4");
			pins[14] = new Pin("D3");
			pins[15] = new Pin("D2");
			pins[16] = new Pin("D1");
			pins[17] = new Pin("D0");
			pins[10].output = true;
			pins[11].output = true;
			pins[12].output = true;
			pins[13].output = true;
			pins[14].output = true;
			pins[15].output = true;
			pins[16].output = true;
			pins[17].output = true;
		}

		public override int getLeadCount() {
			return 18;
		}

		public override int getVoltageSourceCount() {
			return 8;
		}

		public override void execute() {
			short index = 0;
			if (pins[8].value || pins[9].value) {
				if (pins[0].value) {
					index += 128;
				}
				if (pins[1].value) {
					index += 64;
				}
				if (pins[2].value) {
					index += 32;
				}
				if (pins[3].value) {
					index += 16;
				}
				if (pins[4].value) {
					index += 8;
				}
				if (pins[5].value) {
					index += 4;
				}
				if (pins[6].value) {
					index += 2;
				}
				if (pins[7].value) {
					index += 1;
				}
				if (pins[8].value) {
					if ((data[index] & 128) > 0) {
						pins[10].value = true;
					} else {
						pins[10].value = false;
					}
					if ((data[index] & 64) > 0) {
						pins[11].value = true;
					} else {
						pins[11].value = false;
					}
					if ((data[index] & 32) > 0) {
						pins[12].value = true;
					} else {
						pins[12].value = false;
					}
					if ((data[index] & 16) > 0) {
						pins[13].value = true;
					} else {
						pins[13].value = false;
					}
					if ((data[index] & 8) > 0) {
						pins[14].value = true;
					} else {
						pins[14].value = false;
					}
					if ((data[index] & 4) > 0) {
						pins[15].value = true;
					} else {
						pins[15].value = false;
					}
					if ((data[index] & 2) > 0) {
						pins[16].value = true;
					} else {
						pins[16].value = false;
					}
					if ((data[index] & 1) > 0) {
						pins[17].value = true;
					} else {
						pins[17].value = false;
					}
				} else {
					data[index] = 0;
					if (pins[10].value) {
						data[index] += 128;
					}
					if (pins[11].value) {
						data[index] += 64;
					}
					if (pins[12].value) {
						data[index] += 32;
					}
					if (pins[13].value) {
						data[index] += 16;
					}
					if (pins[14].value) {
						data[index] += 8;
					}
					if (pins[15].value) {
						data[index] += 4;
					}
					if (pins[16].value) {
						data[index] += 2;
					}
					if (pins[17].value) {
						data[index] += 1;
					}
				}
			}
		}

		public override void doStep() {
			int i;
			for (i = 0; i != getLeadCount(); i++) {
				Pin p = pins[i];
				if (p.output && pins[9].value) {
					p.value = volts[i] > 2.5;
				}
				if (!p.output) {
					p.value = volts[i] > 2.5;
				}
			}
			execute();
			for (i = 0; i != getLeadCount(); i++) {
				Pin p = pins[i];
				if (p.output && !pins[9].value) {
					sim.updateVoltageSource(0, nodes[i], p.voltSource, p.value ? 5
							: 0);
				}
			}
		}

	}
}