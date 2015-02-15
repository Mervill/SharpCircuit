using System;
using System.Collections;
using System.Collections.Generic;

namespace SharpCircuit {

	public class DFlipFlopElm : ChipElm {

		public bool hasResetPin {
			get {
				return _hasReset;
			}
			set {
				_hasReset = value;
				setupPins();
			}
		}

		public bool hasSetPin {
			get {
				return _hasSet;
			}
			set {
				_hasSet = value;
				setupPins();
			}
		}

		private bool _hasReset;
		private bool _hasSet;

		public DFlipFlopElm() : base() {

		}

		public override String getChipName() {
			return "D flip-flop";
		}

		public override void setupPins() {
			pins = new Pin[getLeadCount()];
			pins[0] = new Pin("D");
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
				pins[1].value = pins[0].value;
				pins[2].value = !pins[0].value;
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