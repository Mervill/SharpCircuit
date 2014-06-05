using System;
using System.Collections;
using System.Collections.Generic;

namespace Circuts {

	public class LEDElm : DiodeElm {
		public double colorR, colorG, colorB;

		public LEDElm(int xx, int yy, CirSim s) : base(xx, yy,s) {
			fwdrop = 2.1024259;
			setup();
			colorR = 1;
			colorG = colorB = 0;
		}

		public override int getDumpType() {
			return 162;
		}

		public override String dump() {
			return base.dump() + " " + colorR + " " + colorG + " " + colorB;
		}

		public Point ledLead1, ledLead2, ledCenter;

		public override void setPoints() {
			base.setPoints();
			int cr = 12;
			ledLead1 = interpPoint(point1, point2, .5 - cr / dn);
			ledLead2 = interpPoint(point1, point2, .5 + cr / dn);
			ledCenter = interpPoint(point1, point2, .5);
		}

		/*public override void draw(Graphics g) {
			if (needsHighlight() || this == sim.dragElm) {
				super.draw(g);
				return;
			}
			setVoltageColor(g, volts[0]);
			drawThickLine(g, point1, ledLead1);
			setVoltageColor(g, volts[1]);
			drawThickLine(g, ledLead2, point2);

			g.setColor(Color.gray);
			int cr = 12;
			drawThickCircle(g, ledCenter.x, ledCenter.y, cr);
			cr -= 4;
			double w = 255 * current / .01;
			if (w > 255) {
				w = 255;
			}
			Color cc = new Color((int) (colorR * w), (int) (colorG * w),
					(int) (colorB * w));
			g.setColor(cc);
			g.fillOval(ledCenter.x - cr, ledCenter.y - cr, cr * 2, cr * 2);
			setBbox(point1, point2, cr);
			updateDotCount();
			drawDots(g, point1, ledLead1, curcount);
			drawDots(g, point2, ledLead2, -curcount);
			drawPosts(g);
		}*/

		public override void getInfo(String[] arr) {
			base.getInfo(arr);
			arr[0] = "LED";
		}

		/*public EditInfo getEditInfo(int n) {
			if (n == 0) {
				return super.getEditInfo(n);
			}
			if (n == 1) {
				return new EditInfo("Red Value (0-1)", colorR, 0, 1)
						.setDimensionless();
			}
			if (n == 2) {
				return new EditInfo("Green Value (0-1)", colorG, 0, 1)
						.setDimensionless();
			}
			if (n == 3) {
				return new EditInfo("Blue Value (0-1)", colorB, 0, 1)
						.setDimensionless();
			}
			return null;
		}

		public void setEditValue(int n, EditInfo ei) {
			if (n == 0) {
				super.setEditValue(0, ei);
			}
			if (n == 1) {
				colorR = ei.value;
			}
			if (n == 2) {
				colorG = ei.value;
			}
			if (n == 3) {
				colorB = ei.value;
			}
		}*/

		public override int getShortcut() {
			return 'l';
		}
	}
}