using System;
using System.Collections;
using System.Collections.Generic;

namespace Circuits {

	public class InverterElm : CircuitElement {
		public double slewRate; // V/ns

		public InverterElm(int xx, int yy, CirSim s) : base(xx, yy, s) {
			noDiagonal = true;
			slewRate = .5;
		}

		/*public override void draw(Graphics g) {
			drawPosts(g);
			draw2Leads(g);
			g.setColor(needsHighlight() ? selectColor : lightGrayColor);
			drawThickPolygon(g, gatePoly);
			drawThickCircle(g, pcircle.x, pcircle.y, 3);
			curcount = updateDotCount(current, curcount);
			drawDots(g, lead2, point2, curcount);
		}*/

//		public override void setPoints() {
//			base.setPoints();
//			int hs = 16;
//			int ww = 16;
//			if (ww > dn / 2) {
//				ww = (int) (dn / 2);
//			}
//			lead1 = interpPoint(point1, point2, .5 - ww / dn);
//			lead2 = interpPoint(point1, point2, .5 + (ww + 2) / dn);
//			pcircle = interpPoint(point1, point2, .5 + (ww - 2) / dn);
//			Point[] triPoints = newPointArray(3);
//			interpPoint2(lead1, lead2, triPoints[0], triPoints[1], 0, hs);
//			triPoints[2] = interpPoint(point1, point2, .5 + (ww - 5) / dn);
//			//gatePoly = createPolygon(triPoints);
//			//setBbox(point1, point2, hs);
//		}

		public override int getVoltageSourceCount() {
			return 1;
		}

		public override void stamp() {
			sim.stampVoltageSource(0, nodes[1], voltSource);
		}

		public override void doStep() {
			double v0 = volts[1];
			double @out = volts[0] > 2.5 ? 0 : 5;
			double maxStep = slewRate * sim.timeStep * 1e9;
			@out = Math.Max(Math.Min(v0 + maxStep, @out), v0 - maxStep);
			sim.updateVoltageSource(0, nodes[1], voltSource, @out);
		}

		public override double getVoltageDiff() {
			return volts[0];
		}

		public override void getInfo(String[] arr) {
			arr[0] = "inverter";
			arr[1] = "Vi = " + getVoltageText(volts[0]);
			arr[2] = "Vo = " + getVoltageText(volts[1]);
		}

		/*public EditInfo getEditInfo(int n) {
			if (n == 0) {
				return new EditInfo("Slew Rate (V/ns)", slewRate, 0, 0);
			}
			return null;
		}

		public void setEditValue(int n, EditInfo ei) {
			slewRate = ei.value;
		}*/

		// there is no current path through the inverter input, but there
		// is an indirect path through the output to ground.
		public override bool getConnection(int n1, int n2) {
			return false;
		}

		public override bool hasGroundConnection(int n1) {
			return (n1 == 1);
		}
		
	}
}