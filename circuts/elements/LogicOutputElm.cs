using System;
using System.Collections;
using System.Collections.Generic;

namespace Circuts {

	public class LogicOutputElm : CircuitElm {
		public int FLAG_TERNARY = 1;
		public int FLAG_NUMERIC = 2;
		public int FLAG_PULLDOWN = 4;
		public double threshold;
		public String value;

		public LogicOutputElm(int xx, int yy, CirSim s) : base(xx, yy, s) {
			threshold = 2.5;
		}

		public override String dump() {
			return base.dump() + " " + threshold;
		}

		public override int getDumpType() {
			return 'M';
		}

		public override int getPostCount() {
			return 1;
		}

		public bool isTernary() {
			return (flags & FLAG_TERNARY) != 0;
		}

		public bool isNumeric() {
			return (flags & (FLAG_TERNARY | FLAG_NUMERIC)) != 0;
		}

		public bool needsPullDown() {
			return (flags & FLAG_PULLDOWN) != 0;
		}

		public override void setPoints() {
			base.setPoints();
			lead1 = interpPoint(point1, point2, 1 - 12 / dn);
		}

		/*public override void draw(Graphics g) {
			Font f = new Font("SansSerif", Font.BOLD, 20);
			g.setFont(f);
			// g.setColor(needsHighlight() ? selectColor : lightGrayColor);
			g.setColor(lightGrayColor);
			String s = (volts[0] < threshold) ? "L" : "H";
			if (isTernary()) {
				if (volts[0] > 3.75) {
					s = "2";
				} else if (volts[0] > 1.25) {
					s = "1";
				} else {
					s = "0";
				}
			} else if (isNumeric()) {
				s = (volts[0] < threshold) ? "0" : "1";
			}
			value = s;
			setBbox(point1, lead1, 0);
			drawCenteredText(g, s, x2, y2, true);
			setVoltageColor(g, volts[0]);
			drawThickLine(g, point1, lead1);
			drawPosts(g);
		}*/

		public override void stamp() {
			if (needsPullDown()) {
				sim.stampResistor(nodes[0], 0, 1e6);
			}
		}

		public override double getVoltageDiff() {
			return volts[0];
		}

		public override void getInfo(String[] arr) {
			arr[0] = "logic output";
			arr[1] = (volts[0] < threshold) ? "low" : "high";
			if (isNumeric()) {
				arr[1] = value;
			}
			arr[2] = "V = " + getVoltageText(volts[0]);
		}

		/*public EditInfo getEditInfo(int n) {
			if (n == 0) {
				return new EditInfo("Threshold", threshold, 10, -10);
			}
			if (n == 1) {
				EditInfo ei = new EditInfo("", 0, -1, -1);
				ei.checkbox = new Checkbox("Current Required", needsPullDown());
				return ei;
			}
			return null;
		}

		public void setEditValue(int n, EditInfo ei) {
			if (n == 0) {
				threshold = ei.value;
			}
			if (n == 1) {
				if (ei.checkbox.getState()) {
					flags = FLAG_PULLDOWN;
				} else {
					flags &= ~FLAG_PULLDOWN;
				}
			}
		}*/

		public override int getShortcut() {
			return 'o';
		}
	}
}