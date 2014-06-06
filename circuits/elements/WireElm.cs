using System;
using System.Collections;
using System.Collections.Generic;

namespace Circuits {

	public class WireElm : CircuitElm {

		public static int FLAG_SHOWCURRENT = 1;
		public static int FLAG_SHOWVOLTAGE = 2;

		public WireElm(int xx, int yy,CirSim s) : base(xx, yy,s) {
			
		}

		/*public override void draw(Graphics g) {
			setVoltageColor(g, volts[0]);
			drawThickLine(g, point1, point2);
			doDots(g);
			setBbox(point1, point2, 3);
			if (mustShowCurrent()) {
				String s = getShortUnitText(Math.abs(getCurrent()), "A");
				drawValues(g, s, 4);
			} else if (mustShowVoltage()) {
				String s = getShortUnitText(volts[0], "V");
				drawValues(g, s, 4);
			}
			drawPosts(g);
		}*/

		public override void stamp() {
			sim.stampVoltageSource(nodes[0], nodes[1], voltSource, 0);
		}

		public bool mustShowCurrent() {
			return (flags & FLAG_SHOWCURRENT) != 0;
		}

		public bool mustShowVoltage() {
			return (flags & FLAG_SHOWVOLTAGE) != 0;
		}

		public override int getVoltageSourceCount() {
			return 1;
		}

		public override void getInfo(String[] arr) {
			arr[0] = "wire";
			arr[1] = "I = " + getCurrentDText(getCurrent());
			arr[2] = "V = " + getVoltageText(volts[0]);
		}

		public override double getPower() {
			return 0;
		}

		public override double getVoltageDiff() {
			return volts[0];
		}

		public override bool isWire() {
			return true;
		}

		/*public EditInfo getEditInfo(int n) {
			if (n == 0) {
				EditInfo ei = new EditInfo("", 0, -1, -1);
				ei.checkbox = new Checkbox("Show Current", mustShowCurrent());
				return ei;
			}
			if (n == 1) {
				EditInfo ei = new EditInfo("", 0, -1, -1);
				ei.checkbox = new Checkbox("Show Voltage", mustShowVoltage());
				return ei;
			}
			return null;
		}

		public void setEditValue(int n, EditInfo ei) {
			if (n == 0) {
				if (ei.checkbox.getState()) {
					flags = FLAG_SHOWCURRENT;
				} else {
					flags &= ~FLAG_SHOWCURRENT;
				}
			}
			if (n == 1) {
				if (ei.checkbox.getState()) {
					flags = FLAG_SHOWVOLTAGE;
				} else {
					flags &= ~FLAG_SHOWVOLTAGE;
				}
			}
		}*/

	}
}