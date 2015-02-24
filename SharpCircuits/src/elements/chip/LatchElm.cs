using System;
using System.Collections;
using System.Collections.Generic;

namespace SharpCircuit {

	public class LatchElm : Chip {

		private int loadPin;
		private bool lastLoad = false;

		public LatchElm() : base() {

		}

		public override String getChipName() {
			return "Latch";
		}

		public override bool needsBits() {
			return true;
		}

		public override void setupPins() {
			pins = new Pin[getLeadCount()];
			for(int i = 0; i != bits; i++)
				pins[i] = new Pin("I" + i);
			for(int i = 0; i != bits; i++) {
				pins[i + bits] = new Pin("O" + i);
				pins[i + bits].output = true;
			}
			pins[loadPin = bits * 2] = new Pin("Ld");
			allocLeads();
		}

		public override void execute(Circuit sim) {
			if(pins[loadPin].value && !lastLoad)
				for(int i = 0; i != bits; i++)
					pins[i + bits].value = pins[i].value;
			lastLoad = pins[loadPin].value;
		}

		public override int getVoltageSourceCount() {
			return bits;
		}

		public override int getLeadCount() {
			return bits * 2 + 1;
		}

	}
}