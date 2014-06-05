using System;
using System.Collections;
using System.Collections.Generic;

namespace Circuts {

	public class SwitchElm : CircuitElm {
		public bool momentary;
		// position 0 == closed, position 1 == open
		public int position, posCount;

		public SwitchElm(int xx, int yy, CirSim s) : base(xx, yy, s) {
			momentary = false;
			position = 0;
			posCount = 2;
		}

		public SwitchElm(int xx, int yy, bool mm, CirSim s) : base(xx, yy,s) {
			position = (mm) ? 1 : 0;
			momentary = mm;
			posCount = 2;
		}

		public override int getDumpType() {
			return 's';
		}

		public override String dump() {
			return base.dump() + " " + position + " " + momentary;
		}

		public Point ps;

		public override void setPoints() {
			base.setPoints();
			calcLeads(32);
			ps = new Point();
			ps2 = new Point();
		}

		/*public override void draw(Graphics g) {
			int openhs = 16;
			int hs1 = (position == 1) ? 0 : 2;
			int hs2 = (position == 1) ? openhs : 2;
			setBbox(point1, point2, openhs);

			draw2Leads(g);

			if (position == 0) {
				doDots(g);
			}

			if (!needsHighlight()) {
				g.setColor(whiteColor);
			}
			interpPoint(lead1, lead2, ps, 0, hs1);
			interpPoint(lead1, lead2, ps2, 1, hs2);

			drawThickLine(g, ps, ps2);
			drawPosts(g);
		}*/

		public override void calculateCurrent() {
			if (position == 1) {
				current = 0;
			}
		}

		public override void stamp() {
			if (position == 0) {
				sim.stampVoltageSource(nodes[0], nodes[1], voltSource, 0);
			}
		}

		public override int getVoltageSourceCount() {
			return (position == 1) ? 0 : 1;
		}

		public virtual void mouseUp() {
			if (momentary) {
				toggle();
			}
		}

		public virtual void toggle() {
			position++;
			if (position >= posCount) {
				position = 0;
			}
		}

		public override void getInfo(String[] arr) {
			arr[0] = (momentary) ? "push switch (SPST)" : "switch (SPST)";
			if (position == 1) {
				arr[1] = "open";
				arr[2] = "Vd = " + getVoltageDText(getVoltageDiff());
			} else {
				arr[1] = "closed";
				arr[2] = "V = " + getVoltageText(volts[0]);
				arr[3] = "I = " + getCurrentDText(getCurrent());
			}
		}

		public override bool getConnection(int n1, int n2) {
			return position == 0;
		}

		public override bool isWire() {
			return true;
		}

		/*public EditInfo getEditInfo(int n) {
			if (n == 0) {
				EditInfo ei = new EditInfo("", 0, -1, -1);
				ei.checkbox = new Checkbox("Momentary Switch", momentary);
				return ei;
			}
			return null;
		}

		public void setEditValue(int n, EditInfo ei) {
			if (n == 0) {
				momentary = ei.checkbox.getState();
			}
		}*/

		public override int getShortcut() {
			return 's';
		}
	}
}