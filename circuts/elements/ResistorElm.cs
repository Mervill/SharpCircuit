using System;
using System.Collections;
using System.Collections.Generic;

namespace Circuts {

	public class ResistorElm : CircuitElm {
		public double resistance;

		public ResistorElm(int xx, int yy,CirSim s) : base(xx, yy, s) {
			resistance = 100;
		}

		public override int getDumpType() {
			return 'r';
		}

		public override String dump() {
			return base.dump() + " " + resistance;
		}

		public Point ps3, ps4;

		public override void setPoints() {
			base.setPoints();
			calcLeads(32);
			ps3 = new Point();
			ps4 = new Point();
		}

		/*public void draw(Graphics g) {
			int segments = 16;
			int i;
			int ox = 0;
			int hs = sim.euroResistorCheckItem.getState() ? 6 : 8;
			double v1 = volts[0];
			double v2 = volts[1];
			setBbox(point1, point2, hs);
			draw2Leads(g);
			setPowerColor(g, true);
			double segf = 1. / segments;
			if (!sim.euroResistorCheckItem.getState()) {
				// draw zigzag
				for (i = 0; i != segments; i++) {
					int nx = 0;
					switch (i & 3) {
					case 0:
						nx = 1;
						break;
					case 2:
						nx = -1;
						break;
					default:
						nx = 0;
						break;
					}
					double v = v1 + (v2 - v1) * i / segments;
					setVoltageColor(g, v);
					interpPoint(lead1, lead2, ps1, i * segf, hs * ox);
					interpPoint(lead1, lead2, ps2, (i + 1) * segf, hs * nx);
					drawThickLine(g, ps1, ps2);
					ox = nx;
				}
			} else {
				// draw rectangle
				setVoltageColor(g, v1);
				interpPoint2(lead1, lead2, ps1, ps2, 0, hs);
				drawThickLine(g, ps1, ps2);
				for (i = 0; i != segments; i++) {
					double v = v1 + (v2 - v1) * i / segments;
					setVoltageColor(g, v);
					interpPoint2(lead1, lead2, ps1, ps2, i * segf, hs);
					interpPoint2(lead1, lead2, ps3, ps4, (i + 1) * segf, hs);
					drawThickLine(g, ps1, ps3);
					drawThickLine(g, ps2, ps4);
				}
				interpPoint2(lead1, lead2, ps1, ps2, 1, hs);
				drawThickLine(g, ps1, ps2);
			}
			if (sim.showValuesCheckItem.getState()) {
				String s = getShortUnitText(resistance, "");
				drawValues(g, s, hs);
			}
			doDots(g);
			drawPosts(g);
		}*/

		public override void calculateCurrent() {
			current = (volts[0] - volts[1]) / resistance;
			// System.out.print(this + " res current set to " + current + "\n");
		}

		public override void stamp() {
			sim.stampResistor(nodes[0], nodes[1], resistance);
		}

		public override void getInfo(String[] arr) {
			arr[0] = "resistor";
			getBasicInfo(arr);
			arr[3] = "R = " + getUnitText(resistance, CirSim.ohmString);
			arr[4] = "P = " + getUnitText(getPower(), "W");
		}

		/*public override  EditInfo getEditInfo(int n) {
			// ohmString doesn't work here on linux
			if (n == 0) {
				return new EditInfo("Resistance (ohms)", resistance, 0, 0);
			}
			return null;
		}

		public override public void setEditValue(int n, EditInfo ei) {
			if (ei.value > 0) {
				resistance = ei.value;
			}
		}*/

		public override int getShortcut() {
			return 'r';
		}
	}
}