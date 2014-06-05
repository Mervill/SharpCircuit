using System;
using System.Collections;
using System.Collections.Generic;

namespace Circuts {

	public class CurrentElm : CircuitElm {
		
		public double currentValue;

		public CurrentElm(int xx, int yy, CirSim s) : base(xx, yy, s) {
			currentValue = 0.01;
		}

		public override String dump() {
			return base.dump() + " " + currentValue;
		}

		public override int getDumpType() {
			return 'i';
		}

		//Polygon arrow;
		//Point ashaft1, ashaft2;
		//Point center;

		public override void setPoints() {
			base.setPoints();
			calcLeads(26);
			//ashaft1 = interpPoint(lead1, lead2, 0.25);
			//ashaft2 = interpPoint(lead1, lead2, 0.6);
			//center = interpPoint(lead1, lead2, 0.5);
			//Point p2 = interpPoint(lead1, lead2, 0.75);
			//arrow = calcArrow(center, p2, 4, 4);
		}

		/*public override void draw(Graphics g) {
			int cr = 12;
			draw2Leads(g);
			setVoltageColor(g, (volts[0] + volts[1]) / 2);
			setPowerColor(g, false);

			drawThickCircle(g, center.x, center.y, cr);
			drawThickLine(g, ashaft1, ashaft2);

			g.fillPolygon(arrow);
			setBbox(point1, point2, cr);
			doDots(g);
			if (sim.showValuesCheckItem.getState()) {
				String s = getShortUnitText(currentValue, "A");
				if (dx == 0 || dy == 0) {
					drawValues(g, s, cr);
				}
			}
			drawPosts(g);
		}*/

		public override void stamp() {
			current = currentValue;
			sim.stampCurrentSource(nodes[0], nodes[1], current);
		}

		/*public EditInfo getEditInfo(int n) {
			if (n == 0) {
				return new EditInfo("Current (A)", currentValue, 0, .1);
			}
			return null;
		}

		public void setEditValue(int n, EditInfo ei) {
			currentValue = ei.value;
		}*/

		public override void getInfo(String[] arr) {
			arr[0] = "current source";
			getBasicInfo(arr);
		}

		public override double getVoltageDiff() {
			return volts[1] - volts[0];
		}
	}
}