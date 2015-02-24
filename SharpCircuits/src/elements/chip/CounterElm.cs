using System;
using System.Collections;
using System.Collections.Generic;

namespace SharpCircuit {

	public class CounterElm : Chip {

		public bool hasEnable {
			get {
				return _hasEnable;
			}
			set {
				_hasEnable = value;
				setupPins();
			}
		}

		public bool invertReset { get; set; }

		private bool _hasEnable;

		public override bool needsBits() {
			return true;
		}

		public override String getChipName() {
			return "Counter";
		}

		public override void setupPins() {
			pins = new Pin[getLeadCount()];
			pins[0] = new Pin("");
			pins[0].clock = true;
			pins[1] = new Pin("R");
			for(int i = 0; i != bits; i++) {
				int ii = i + 2;
				pins[ii] = new Pin("Q" + (bits - i - 1));
				pins[ii].output = true;
			}
			if(hasEnable)
				pins[bits + 2] = new Pin("En");
			allocLeads();
		}

		public override int getLeadCount() {
			if(hasEnable)
				return bits + 3;
			return bits + 2;
		}

		public override int getVoltageSourceCount() {
			return bits;
		}

		public override void execute(Circuit sim) {
			bool en = true;
			if(hasEnable)
				en = pins[bits + 2].value;
			if(pins[0].value && !lastClock && en) {
				for(int i = bits - 1; i >= 0; i--) {
					int ii = i + 2;
					if(!pins[ii].value) {
						pins[ii].value = true;
						break;
					}
					pins[ii].value = false;
				}
			}
			if(!pins[1].value == invertReset) {
				for(int i = 0; i != bits; i++)
					pins[i + 2].value = false;
			}
			lastClock = pins[0].value;
		}

	}
}