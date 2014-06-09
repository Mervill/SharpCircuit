using System;
using System.Collections;
using System.Collections.Generic;

namespace Circuits {

	public class OutputElm : CircuitElement {

		public OutputElm( CirSim s) : base(s) {

		}

		public override int getLeadCount() {
			return 1;
		}

//		public override void setPoints() {
//			base.setPoints();
//			lead1 = new Point();
//		}

		/*public override void draw(Graphics g) {
			boolean selected = (needsHighlight() || sim.plotYElm == this);
			Font f = new Font("SansSerif", selected ? Font.BOLD : 0, 14);
			g.setFont(f);
			g.setColor(selected ? selectColor : whiteColor);
			String s = (flags & FLAG_VALUE) != 0 ? getVoltageText(volts[0]) : "out";
			FontMetrics fm = g.getFontMetrics();
			if (this == sim.plotXElm) {
				s = "X";
			}
			if (this == sim.plotYElm) {
				s = "Y";
			}
			interpPoint(point1, point2, lead1, 1 - (fm.stringWidth(s) / 2 + 8) / dn);
			setBbox(point1, lead1, 0);
			drawCenteredText(g, s, x2, y2, true);
			setVoltageColor(g, volts[0]);
			if (selected) {
				g.setColor(selectColor);
			}
			drawThickLine(g, point1, lead1);
			drawPosts(g);
		}*/

		public override double getVoltageDiff() {
			return volts[0];
		}

		public override void getInfo(String[] arr) {
			arr[0] = "output";
			arr[1] = "V = " + getVoltageText(volts[0]);
		}

		/*public EditInfo getEditInfo(int n) {
			if (n == 0) {
				EditInfo ei = new EditInfo("", 0, -1, -1);
				ei.checkbox = new Checkbox("Show Voltage",
						(flags & FLAG_VALUE) != 0);
				return ei;
			}
			return null;
		}

		public void setEditValue(int n, EditInfo ei) {
			if (n == 0) {
				flags = (ei.checkbox.getState()) ? (flags | FLAG_VALUE)
						: (flags & ~FLAG_VALUE);
			}
		}*/
	}
}