using System;
using System.Collections;
using System.Collections.Generic;

namespace Circuts {

	public class GroundElm : CircuitElm {

		public GroundElm(int xx, int yy,CirSim s) : base(xx, yy, s) {
			
		}

		public override int getDumpType() {
			return 'g';
		}

		public override int getPostCount() {
			return 1;
		}

		/*public override void draw(Graphics g) {
			setVoltageColor(g, 0);
			drawThickLine(g, point1, point2);
			int i;
			for (i = 0; i != 3; i++) {
				int a = 10 - i * 4;
				int b = i * 5; // -10;
				interpPoint2(point1, point2, ps1, ps2, 1 + b / dn, a);
				drawThickLine(g, ps1, ps2);
			}
			doDots(g);
			interpPoint(point1, point2, ps2, 1 + 11. / dn);
			setBbox(point1, ps2, 11);
			drawPost(g, x, y, nodes[0]);
		}*/

		public override void setCurrent(int x, double c) {
			current = -c;
		}
		
		public override void stamp() {
			sim.stampVoltageSource(0, nodes[0], voltSource, 0);
		}

		public override double getVoltageDiff() {
			return 0;
		}

		public override int getVoltageSourceCount() {
			return 1;
		}

		public override void getInfo(String[] arr) {
			arr[0] = "ground";
			arr[1] = "I = " + getCurrentText(getCurrent());
		}

		public override bool hasGroundConnection(int n1) {
			return true;
		}

		public override int getShortcut() {
			return 'g';
		}
	}
}