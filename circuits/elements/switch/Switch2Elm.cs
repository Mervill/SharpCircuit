using System;
using System.Collections;
using System.Collections.Generic;

namespace Circuits {

	public class Switch2Elm : SwitchElm {

		public static int FLAG_CENTER_OFF = 1;

		public int link{ get; set; }

		public ElementLead[] swposts;

		public Switch2Elm(CirSim s) : base(s) {
			swposts = newLeadArray(2);
			posCount = hasCenterOff() ? 3 : 2;
		}

		public Switch2Elm(CirSim s,bool mm) : base(s,mm) {
			swposts = newLeadArray(2);
			posCount = hasCenterOff() ? 3 : 2;
		}

		public override ElementLead getLead(int n) {
			return (n == 0) ? lead0 : swposts[n - 1];
		}

		public override int getLeadCount() {
			return 3;
		}

		public override void calculateCurrent() {
			if (position == 2) {
				current = 0;
			}
		}

		public override void stamp() {
			if (position == 2) {
				return;
			}
			sim.stampVoltageSource(nodes[0], nodes[position + 1], voltSource, 0);
		}

		public override int getVoltageSourceCount() {
			return (position == 2) ? 0 : 1;
		}

		public override void toggle() {
			base.toggle();
			if (link != 0) {
				int i;
				for (i = 0; i != sim.elements.Count; i++) {
					Object o = sim.elements[i];
					if (o is Switch2Elm) {
						Switch2Elm s2 = (Switch2Elm) o;
						if (s2.link == link) {
							s2.position = position;
						}
					}
				}
			}
		}

		public override bool getConnection(int n1, int n2) {
			if (position == 2) {
				return false;
			}
			return comparePair(n1, n2, 0, 1 + position);
		}

		public override void getInfo(String[] arr) {
			arr[0] = (link == 0) ? "switch (SPDT)" : "switch (DPDT)";
			arr[1] = "I = " + getCurrentDText(getCurrent());
		}

		/*public void setEditValue(int n, EditInfo ei) {
			if (n == 1) {
				flags &= ~FLAG_CENTER_OFF;
				if (ei.checkbox.getState()) {
					flags |= FLAG_CENTER_OFF;
				}
				if (hasCenterOff()) {
					momentary = false;
				}
				setPoints();
			} else {
				super.setEditValue(n, ei);
			}
		}*/

		bool hasCenterOff() {
			return (flags & FLAG_CENTER_OFF) != 0;
		}
		
	}
}