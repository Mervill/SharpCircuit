using System;
using System.Collections;
using System.Collections.Generic;

namespace Circuits {

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
			pins = new Pin[getPostCount()];
			int i;
			for (i = 0; i != bits; i++) {
				pins[i] = new Pin("D" + i);
			}
			pins[bits] = new Pin("O");
			pins[bits].output = true;
			pins[bits + 1] = new Pin("V+");
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
		
	}
}