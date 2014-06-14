using System;
using System.Collections;
using System.Collections.Generic;

namespace Circuits {

	public class JKFlipFlopElm : ChipElm {

		public bool hasResetPin { 
			get {
				return _hasReset;
			}
			set {
				_hasReset = value;
				setupPins();
				allocNodes();
			}
		}

		private bool _hasReset;

		public JKFlipFlopElm(CirSim s) : base(s) {
			
		}
		
		public override String getChipName() {
			return "JK flip-flop";
		}

		public override void setupPins() {
			pins = new Pin[getLeadCount()];
			pins[0] = new Pin("J");
			pins[1] = new Pin("");
			pins[1].clock = true;
			pins[2] = new Pin("K");
			pins[3] = new Pin("Q");
			pins[3].output = true;
			pins[4] = new Pin("Q");
			pins[4].output = true;
			pins[4].lineOver = true;

			if (hasResetPin) {
				pins[5] = new Pin("R");
			}
		}

		public override int getLeadCount() {
			return 5 + (hasResetPin ? 1 : 0);
		}

		public override int getVoltageSourceCount() {
			return 2;
		}

		public override void execute() {
			if (!pins[1].value && lastClock) {
				bool q = pins[3].value;
				if (pins[0].value) {
					if (pins[2].value) {
						q = !q;
					} else {
						q = true;
					}
				} else if (pins[2].value) {
					q = false;
				}
				pins[3].value = q;
				pins[4].value = !q;
			}
			lastClock = pins[1].value;

			if (hasResetPin) {
				if (pins[5].value) {
					pins[3].value = false;
					pins[4].value = true;
				}
			}
		}

	}
}