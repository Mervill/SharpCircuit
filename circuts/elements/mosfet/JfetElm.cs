using System;
using System.Collections;
using System.Collections.Generic;

namespace Circuts {

	public class JfetElm : MosfetElm {
		
		public JfetElm(int xx, int yy, bool pnpflag, CirSim s) : base(xx, yy, pnpflag, s) {
			noDiagonal = true;
		}

		//public Polygon gatePoly;
		public Point gatePt;

		/*public override void draw(Graphics g) {
			setBbox(point1, point2, hs);
			setVoltageColor(g, volts[1]);
			drawThickLine(g, src[0], src[1]);
			drawThickLine(g, src[1], src[2]);
			setVoltageColor(g, volts[2]);
			drawThickLine(g, drn[0], drn[1]);
			drawThickLine(g, drn[1], drn[2]);
			setVoltageColor(g, volts[0]);
			drawThickLine(g, point1, gatePt);
			g.fillPolygon(arrowPoly);
			setPowerColor(g, true);
			g.fillPolygon(gatePoly);
			curcount = updateDotCount(-ids, curcount);
			if (curcount != 0) {
				drawDots(g, src[0], src[1], curcount);
				drawDots(g, src[1], src[2], curcount + 8);
				drawDots(g, drn[0], drn[1], -curcount);
				drawDots(g, drn[1], drn[2], -(curcount + 8));
			}
			drawPosts(g);
		}*/

		public override void setPoints() {
			base.setPoints();

			// find the coordinates of the various points we need to draw
			// the JFET.
			int hs2 = hs * dsign;
			src = newPointArray(3);
			drn = newPointArray(3);
			interpPoint2(point1, point2, src[0], drn[0], 1, hs2);
			interpPoint2(point1, point2, src[1], drn[1], 1, hs2 / 2);
			interpPoint2(point1, point2, src[2], drn[2], 1 - 10 / dn, hs2 / 2);

			gatePt = interpPoint(point1, point2, 1 - 14 / dn);

			Point[] ra = newPointArray(4);
			interpPoint2(point1, point2, ra[0], ra[1], 1 - 13 / dn, hs);
			interpPoint2(point1, point2, ra[2], ra[3], 1 - 10 / dn, hs);
			//gatePoly = createPolygon(ra[0], ra[1], ra[3], ra[2]);
			//if (pnp == -1) {
			//	Point x = interpPoint(gatePt, point1, 18 / dn);
			//	arrowPoly = calcArrow(gatePt, x, 8, 3);
			//} else {
			//	arrowPoly = calcArrow(point1, gatePt, 8, 3);
			//}
		}

		// these values are taken from Hayes+Horowitz p155
		public override double getDefaultThreshold() {
			return -4;
		}

		public override double getBeta() {
			return .00125;
		}

		public override void getInfo(String[] arr) {
			getFetInfo(arr, "JFET");
		}
	}
}