using System;
using System.Collections;
using System.Collections.Generic;

namespace Circuts {

	public class AnalogSwitch2Elm : AnalogSwitchElm {
		
		public AnalogSwitch2Elm(int xx, int yy, CirSim s) : base(xx, yy, s) {

		}

		public int openhs = 16;
		public Point[] swposts, swpoles;
		public Point ctlPoint;

		public override void setPoints() {
			base.setPoints();
			calcLeads(32);
			swposts = newPointArray(2);
			swpoles = newPointArray(2);
			interpPoint2(lead1, lead2, swpoles[0], swpoles[1], 1, openhs);
			interpPoint2(point1, point2, swposts[0], swposts[1], 1, openhs);
			ctlPoint = interpPoint(point1, point2, .5, openhs);
		}

		public override int getPostCount() {
			return 4;
		}

		/*public override void draw(Graphics g) {
			setBbox(point1, point2, openhs);

			// draw first lead
			setVoltageColor(g, volts[0]);
			drawThickLine(g, point1, lead1);

			// draw second lead
			setVoltageColor(g, volts[1]);
			drawThickLine(g, swpoles[0], swposts[0]);

			// draw third lead
			setVoltageColor(g, volts[2]);
			drawThickLine(g, swpoles[1], swposts[1]);

			// draw switch
			g.setColor(lightGrayColor);
			int position = (open) ? 1 : 0;
			drawThickLine(g, lead1, swpoles[position]);

			updateDotCount();
			drawDots(g, point1, lead1, curcount);
			drawDots(g, swpoles[position], swposts[position], curcount);
			drawPosts(g);
		}*/

		public override void calculateCurrent() {
			if (open) {
				current = (volts[0] - volts[2]) / r_on;
			} else {
				current = (volts[0] - volts[1]) / r_on;
			}
		}

		public override void stamp() {
			sim.stampNonLinear(nodes[0]);
			sim.stampNonLinear(nodes[1]);
			sim.stampNonLinear(nodes[2]);
		}

		public override void doStep() {
			open = (volts[3] < 2.5);
			if ((flags & FLAG_INVERT) != 0) {
				open = !open;
			}
			if (open) {
				sim.stampResistor(nodes[0], nodes[2], r_on);
				sim.stampResistor(nodes[0], nodes[1], r_off);
			} else {
				sim.stampResistor(nodes[0], nodes[1], r_on);
				sim.stampResistor(nodes[0], nodes[2], r_off);
			}
		}

		public override bool getConnection(int n1, int n2) {
			if (n1 == 3 || n2 == 3) {
				return false;
			}
			return true;
		}

		public override void getInfo(String[] arr) {
			arr[0] = "analog switch (SPDT)";
			arr[1] = "I = " + getCurrentDText(getCurrent());
		}
	}
}