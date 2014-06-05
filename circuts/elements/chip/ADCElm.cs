using System;
using System.Collections;
using System.Collections.Generic;

namespace Circuts {

	public class ADCElm : ChipElm {
		
		public ADCElm(int xx, int yy, CirSim s) : base(xx, yy, s) {
			
		}

		public override String getChipName() {
			return "ADC";
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
				pins[i] = new Pin(bits - 1 - i, SIDE_E, "D" + i, this);
				pins[i].output = true;
			}
			pins[bits] = new Pin(0, SIDE_W, "In", this);
			pins[bits + 1] = new Pin(sizeY - 1, SIDE_W, "V+", this);
			allocNodes();
		}

		public override void execute() {
			int imax = (1 << bits) - 1;
			// if we round, the half-flash doesn't work
			double val = imax * volts[bits] / volts[bits + 1]; // + .5;
			int ival = (int) val;
			ival = min(imax, max(0, ival));
			int i;
			for (i = 0; i != bits; i++) {
				pins[i].value = ((ival & (1 << i)) != 0);
			}
		}

		public override int getVoltageSourceCount() {
			return bits;
		}

		public override int getPostCount() {
			return bits + 2;
		}

	}
}