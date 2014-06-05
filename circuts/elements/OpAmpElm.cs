using System;
using System.Collections;
using System.Collections.Generic;

namespace Circuts {

	public class OpAmpElm : CircuitElm {
		public int opsize, opheight, opwidth, opaddtext;
		public double maxOut, minOut, gain, gbw;
		//public bool reset;
		public int FLAG_SWAP = 1;
		public int FLAG_SMALL = 2;
		public int FLAG_LOWGAIN = 4;

		public OpAmpElm(int xx, int yy, CirSim s) : base (xx,yy,s) {
			noDiagonal = true;
			maxOut = 15;
			minOut = -15;
			gbw = 1e6;
			setSize(sim.smallGridCheckItem ? 1 : 2);
			setGain();
		}

		void setGain() {
			// gain of 100000 breaks e-amp-dfdx.txt
			// gain was 1000, but it broke amp-schmitt.txt
			gain = ((flags & FLAG_LOWGAIN) != 0) ? 1000 : 100000;
		}

		public override String dump() {
			return base.dump() + " " + maxOut + " " + minOut + " " + gbw;
		}

		public override bool nonLinear() {
			return true;
		}

		/*public override void draw(Graphics g) {
			setBbox(point1, point2, opheight * 2);
			setVoltageColor(g, volts[0]);
			drawThickLine(g, in1p[0], in1p[1]);
			setVoltageColor(g, volts[1]);
			drawThickLine(g, in2p[0], in2p[1]);
			g.setColor(needsHighlight() ? selectColor : lightGrayColor);
			setPowerColor(g, true);
			drawThickPolygon(g, triangle);
			g.setFont(plusFont);
			drawCenteredText(g, "-", textp[0].x, textp[0].y - 2, true);
			drawCenteredText(g, "+", textp[1].x, textp[1].y, true);
			setVoltageColor(g, volts[2]);
			drawThickLine(g, lead2, point2);
			curcount = updateDotCount(current, curcount);
			drawDots(g, point2, lead2, curcount);
			drawPosts(g);
		}*/

		public override double getPower() {
			return volts[2] * current;
		}

		public Point[] in1p, in2p, textp;
		//public Polygon triangle;
		//public Font plusFont;

		void setSize(int s) {
			opsize = s;
			opheight = 8 * s;
			opwidth = 13 * s;
			flags = (flags & ~FLAG_SMALL) | ((s == 1) ? FLAG_SMALL : 0);
		}

		public override void setPoints() {
			base.setPoints();
			if (dn > 150 && this == sim.dragElm) {
				setSize(2);
			}
			int ww = opwidth;
			if (ww > dn / 2) {
				ww = (int) (dn / 2);
			}
			calcLeads(ww * 2);
			int hs = opheight * dsign;
			if ((flags & FLAG_SWAP) != 0) {
				hs = -hs;
			}
			in1p = newPointArray(2);
			in2p = newPointArray(2);
			textp = newPointArray(2);
			interpPoint2(point1, point2, in1p[0], in2p[0], 0, hs);
			interpPoint2(lead1, lead2, in1p[1], in2p[1], 0, hs);
			interpPoint2(lead1, lead2, textp[0], textp[1], .2, hs);
			Point[] tris = newPointArray(2);
			interpPoint2(lead1, lead2, tris[0], tris[1], 0, hs * 2);
			//triangle = createPolygon(tris[0], tris[1], lead2);
			//plusFont = new Font("SansSerif", 0, opsize == 2 ? 14 : 10);
		}

		public override int getPostCount() {
			return 3;
		}

		public override Point getPost(int n) {
			return (n == 0) ? in1p[0] : (n == 1) ? in2p[0] : point2;
		}

		public override int getVoltageSourceCount() {
			return 1;
		}

		public override void getInfo(String[] arr) {
			arr[0] = "op-amp";
			arr[1] = "V+ = " + getVoltageText(volts[1]);
			arr[2] = "V- = " + getVoltageText(volts[0]);
			// sometimes the voltage goes slightly outside range, to make
			// convergence easier. so we hide that here.
			double vo = Math.Max(Math.Min(volts[2], maxOut), minOut);
			arr[3] = "Vout = " + getVoltageText(vo);
			arr[4] = "Iout = " + getCurrentText(getCurrent());
			arr[5] = "range = " + getVoltageText(minOut) + " to "
					+ getVoltageText(maxOut);
		}

		public double lastvd;

		public override void stamp() {
			int vn = sim.nodeList.Count + voltSource;
			sim.stampNonLinear(vn);
			sim.stampMatrix(nodes[2], vn, 1);
		}

		public override void doStep() {
			double vd = volts[1] - volts[0];
			if (Math.Abs(lastvd - vd) > .1) {
				sim.converged = false;
			} else if (volts[2] > maxOut + .1 || volts[2] < minOut - .1) {
				sim.converged = false;
			}
			double x = 0;
			int vn = sim.nodeList.Count + voltSource;
			double dx = 0;
			if (vd >= maxOut / gain && (lastvd >= 0 || sim.getrand(4) == 1)) {
				dx = 1e-4;
				x = maxOut - dx * maxOut / gain;
			} else if (vd <= minOut / gain && (lastvd <= 0 || sim.getrand(4) == 1)) {
				dx = 1e-4;
				x = minOut - dx * minOut / gain;
			} else {
				dx = gain;
				// System.out.println("opamp " + vd + " " + volts[2] + " " + dx +
				// " " +
				// x + " " + lastvd + " " + sim.converged);
			}

			// newton-raphson
			sim.stampMatrix(vn, nodes[0], dx);
			sim.stampMatrix(vn, nodes[1], -dx);
			sim.stampMatrix(vn, nodes[2], 1);
			sim.stampRightSide(vn, x);

			lastvd = vd;
			/*
			 * if (sim.converged) System.out.println((volts[1]-volts[0]) + " " +
			 * volts[2] + " " + initvd);
			 */
		}

		// there is no current path through the op-amp inputs, but there
		// is an indirect path through the output to ground.
		public override bool getConnection(int n1, int n2) {
			return false;
		}

		public override bool hasGroundConnection(int n1) {
			return (n1 == 2);
		}

		public override double getVoltageDiff() {
			return volts[2] - volts[1];
		}

		public override int getDumpType() {
			return 'a';
		}

		/*public EditInfo getEditInfo(int n) {
			if (n == 0) {
				return new EditInfo("Max Output (V)", maxOut, 1, 20);
			}
			if (n == 1) {
				return new EditInfo("Min Output (V)", minOut, -20, 0);
			}
			return null;
		}

		public void setEditValue(int n, EditInfo ei) {
			if (n == 0) {
				maxOut = ei.value;
			}
			if (n == 1) {
				minOut = ei.value;
			}
		}*/
	}
}