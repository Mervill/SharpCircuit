using System;
using System.Collections;
using System.Collections.Generic;

namespace Circuits {

	public class ADCElm : ChipElm {
		
		public ADCElm(CirSim s) : base(s) {
			
		}

		public override String getChipName() {
			return "ADC";
		}

		public override bool needsBits() {
			return true;
		}

		public override void setupPins() {
			pins = new Pin[getLeadCount()];
			int i;
			for (i = 0; i != bits; i++) {
				pins[i] = new Pin("D"+i);
				pins[i].output = true;
			}
			pins[bits] = new Pin("In");
			pins[bits + 1] = new Pin("V+");
			allocNodes();
		}

		public override void execute() {
			int imax = (1 << bits) - 1;
			// if we round, the half-flash doesn't work
			double val = imax * volts[bits] / volts[bits + 1]; // + .5;
			int ival = (int) val;
			ival = Math.Min(imax, Math.Max(0, ival));
			int i;
			for (i = 0; i != bits; i++) {
				pins[i].value = ((ival & (1 << i)) != 0);
			}
		}

		public override int getVoltageSourceCount() {
			return bits;
		}

		public override int getLeadCount() {
			return bits + 2;
		}

	}
}