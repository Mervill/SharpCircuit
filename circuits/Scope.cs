using System;
using System.Collections;
using System.Collections.Generic;

namespace Circuits {

	public class Scope {

		public int FLAG_YELM = 32;

		public static int VAL_POWER = 1;
		public static int VAL_IB = 1;
		public static int VAL_IC = 2;
		public static int VAL_IE = 3;
		public static int VAL_VBE = 4;
		public static int VAL_VBC = 5;
		public static int VAL_VCE = 6;
		public static int VAL_R = 2;

		public double[] minV, maxV;
		public double minMaxV;
		public double[] minI, maxI;
		public double minMaxI;
		public int scopePointCount = 128;
		public int ptr, ctr, speed, position;
		public int value, ivalue;
		public bool showI, showV, showMax, showMin, showFreq, lockScale;
		public CircuitElement elm, xElm, yElm;
		public double[] pixels;
		public int draw_ox, draw_oy;
		public float[] dpixels;

		//private CirSim sim;

		public Scope(CirSim s) {
			reset();
			//sim = s;
		}

		public void showCurrent(bool b) {
			showI = b;
			value = ivalue = 0;
		}

		public void showVoltage(bool b) {
			showV = b;
			value = ivalue = 0;
		}

		public void _showMax(bool b) {
			showMax = b;
		}

		public void _showMin(bool b) {
			showMin = b;
		}

		public void _showFreq(bool b) {
			showFreq = b;
		}

		public void setLockScale(bool b) {
			lockScale = b;
		}

		public void resetGraph() {
			scopePointCount = 1;
			minV = new double[scopePointCount];
			maxV = new double[scopePointCount];
			minI = new double[scopePointCount];
			maxI = new double[scopePointCount];
			ptr = ctr = 0;
		}

		public bool active() {
			return elm != null;
		}

		public void reset() {
			resetGraph();
			minMaxV = 5;
			minMaxI = .1;
			speed = 64;
			showI = showV = showMax = true;
			showFreq = lockScale = showMin = false;

			// no showI for Output
			if(elm != null && (elm is OutputElm || elm is LogicOutputElm || elm is ProbeElm))
				showI = false;

			value = ivalue = 0;
			if(elm is TransistorElm)
				value = VAL_VCE;

		}

		public void setElm(CircuitElement ce) {
			elm = ce;
			reset();
		}

		/*public void timeStep() {
			if (elm == null) {
				return;
			}
			double v = elm.getScopeValue(value);
			if (v < minV[ptr]) {
				minV[ptr] = v;
			}
			if (v > maxV[ptr]) {
				maxV[ptr] = v;
			}
			double i = 0;
			if (value == 0 || ivalue != 0) {
				i = (ivalue == 0) ? elm.getCurrent() : elm.getScopeValue(ivalue);
				if (i < minI[ptr]) {
					minI[ptr] = i;
				}
				if (i > maxI[ptr]) {
					maxI[ptr] = i;
				}
			}

			if (plot2d && dpixels != null) {
				while (v > minMaxV || v < -minMaxV) {
					minMaxV *= 2;
				}
				double yval = i;
				if (plotXY) {
					yval = (yElm == null) ? 0 : yElm.getVoltageDiff();
				}
				while (yval > minMaxI || yval < -minMaxI) {
					minMaxI *= 2;
				}
				double xa = v / minMaxV;
				double ya = yval / minMaxI;
				int x = (int) (rect.width * (1 + xa) * .499);
				int y = (int) (rect.height * (1 - ya) * .499);
				drawTo(x, y);
			} else {
				ctr++;
				if (ctr >= speed) {
					ptr = (ptr + 1) & (scopePointCount - 1);
					minV[ptr] = maxV[ptr] = v;
					minI[ptr] = maxI[ptr] = i;
					ctr = 0;
				}
			}
		}

		public void drawTo(int x2, int y2) {
			if (draw_ox == -1) {
				draw_ox = x2;
				draw_oy = y2;
			}
			// need to draw a line from x1,y1 to x2,y2
			if (draw_ox == x2 && draw_oy == y2) {
				dpixels[x2 + rect.width * y2] = 1;
			} else if (CircuitElm.abs(y2 - draw_oy) > CircuitElm.abs(x2 - draw_ox)) {
				// y difference is greater, so we step along y's
				// from min to max y and calculate x for each step
				int sgn = CircuitElm.sign(y2 - draw_oy);
				int x, y;
				for (y = draw_oy; y != y2 + sgn; y += sgn) {
					x = draw_ox + (x2 - draw_ox) * (y - draw_oy) / (y2 - draw_oy);
					dpixels[x + rect.width * y] = 1;
				}
			} else {
				// x difference is greater, so we step along x's
				// from min to max x and calculate y for each step
				int sgn = CircuitElm.sign(x2 - draw_ox);
				int x, y;
				for (x = draw_ox; x != x2 + sgn; x += sgn) {
					y = draw_oy + (y2 - draw_oy) * (x - draw_ox) / (x2 - draw_ox);
					dpixels[x + rect.width * y] = 1;
				}
			}
			draw_ox = x2;
			draw_oy = y2;
		}

		public void adjustScale(double x) {
			minMaxV *= x;
			minMaxI *= x;
		}
		*/

		public void speedUp() {
			if (speed > 1) {
				speed /= 2;
				resetGraph();
			}
		}

		public void slowDown() {
			speed *= 2;
			resetGraph();
		}

		public void setValue(int x) {
			reset();
			value = x;
		}

	}
}
