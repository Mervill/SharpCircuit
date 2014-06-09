using System;
using System.Collections;
using System.Collections.Generic;

namespace Circuits {

	public class OrGateElm : GateElm {

		public OrGateElm( CirSim s) : base(s) {

		}

		public override String getGateName() {
			return "OR gate";
		}

//		public override void setPoints() {
//			base.setPoints();
//
//			// 0-15 = top curve, 16 = right, 17-32=bottom curve,
//			// 33-37 = left curve
//			Point[] triPoints = newPointArray(38);
//			if (this is XorGateElm) {
//				linePoints = new Point[5];
//			}
//			int i;
//			for (i = 0; i != 16; i++) {
//				double a = i / 16.0;
//				double b = 1 - a * a;
//				interpPoint2(lead1, lead2, triPoints[i], triPoints[32 - i],
//						.5 + a / 2, b * hs2);
//			}
//			double ww2 = (ww == 0) ? dn * 2 : ww * 2;
//			for (i = 0; i != 5; i++) {
//				double a = (i - 2) / 2.0;
//				double b = 4 * (1 - a * a) - 2;
//				interpPoint(lead1, lead2, triPoints[33 + i], b / (ww2), a * hs2);
//				if (this is XorGateElm) {
//					linePoints[i] = interpPoint(lead1, lead2, (b - 5) / (ww2), a
//							* hs2);
//				}
//			}
//			triPoints[16] = new Point(lead2);
//			if (isInverting()) {
//				pcircle = interpPoint(point1, point2, .5 + (ww + 4) / dn);
//				lead2 = interpPoint(point1, point2, .5 + (ww + 8) / dn);
//			}
//			//gatePoly = createPolygon(triPoints);
//		}

		public override bool calcFunction() {
			int i;
			bool f = false;
			for (i = 0; i != inputCount; i++) {
				f |= getInput(i);
			}
			return f;
		}

	}
}