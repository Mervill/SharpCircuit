using System;
using System.Collections;
using System.Collections.Generic;

namespace Circuts {

	public class DACElm : ChipElm {
		
		public DACElm(int xx, int yy, CirSim s) : base(xx, yy, s) {
			
		}

		public override String getChipName() {
			return "DAC";
		}

		public override bool needsBits() {
			return true;
		}

		public override void setupPins() {
			sizeX = 2;
			sizeY = bits > 2 ? bits : 2;
			pins = new Pin[getPostCount()];
			int i;
			for (i = 0; i != bits; i++) {
				pins[i] = new Pin(bits - 1 - i, SIDE_W, "D" + i, this);
			}
			pins[bits] = new Pin(0, SIDE_E, "O", this);
			pins[bits].output = true;
			pins[bits + 1] = new Pin(sizeY - 1, SIDE_E, "V+", this);
			allocNodes();
		}

		public override void doStep() {
			int ival = 0;
			int i;
			for (i = 0; i != bits; i++) {
				if (volts[i] > 2.5) {
					ival |= 1 << i;
				}
			}
			int ivalmax = (1 << bits) - 1;
			double v = ival * volts[bits + 1] / ivalmax;
			sim.updateVoltageSource(0, nodes[bits], pins[bits].voltSource, v);
		}

		public override int getVoltageSourceCount() {
			return 1;
		}

		public override int getPostCount() {
			return bits + 2;
		}

		public override int getDumpType() {
			return 166;
		}
	}
}