using System;
using System.Collections;
using System.Collections.Generic;

namespace SharpCircuit {

	public class TFlipFlopElm : ChipElm {

		public bool hasResetPin {
			get {
				return _hasReset;
			}
			set {
				_hasReset = value;
				setupPins();
				allocLeads();
			}
		}

		public bool hasSetPin {
			get {
				return _hasSet;
			}
			set {
				_hasSet = value;
				setupPins();
				allocLeads();
			}
		}

		private bool _hasReset;
		private bool _hasSet;

		private bool last_val;

		public TFlipFlopElm() : base() {

		}

		public override String getChipName() {
			return "T flip-flop";
		}

		public override void setupPins() {
			pins = new Pin[getLeadCount()];
			pins[0] = new Pin("T");
			pins[1] = new Pin("Q");
			pins[1].output = true;
			pins[2] = new Pin("Q");
			pins[2].output = true;
			pins[2].lineOver = true;
			pins[3] = new Pin("");
			pins[3].clock = true;
			if(!hasSetPin) {
				if(hasResetPin)
					pins[4] = new Pin("R");
			} else {
				pins[5] = new Pin("S");
				pins[4] = new Pin("R");
			}
		}

		public override int getLeadCount() {
			return 4 + (hasResetPin ? 1 : 0) + (hasSetPin ? 1 : 0);
		}

		public override int getVoltageSourceCount() {
			return 2;
		}

		public override void reset() {
			base.reset();
			lead_volt[2] = 5;
			pins[2].value = true;
		}

		public override void execute(Circuit sim) {
			if(pins[3].value && !lastClock) {
				if(pins[0].value) { // if T = 1 {
					pins[1].value = !last_val;
					pins[2].value = !pins[1].value;
					last_val = !last_val;
				}
				// else no change

			}
			if(hasSetPin && pins[5].value) {
				pins[1].value = true;
				pins[2].value = false;
			}
			if(hasResetPin && pins[4].value) {
				pins[1].value = false;
				pins[2].value = true;
			}
			lastClock = pins[3].value;
		}

	}
}