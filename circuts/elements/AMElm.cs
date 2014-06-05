using System;
using System.Collections;
using System.Collections.Generic;

namespace Circuts {

	// contributed by Edward Calver

	public class AMElm : CircuitElm {
		public static int FLAG_COS = 2;
		public double carrierfreq, signalfreq, maxVoltage, freqTimeZero;

		public AMElm(int xx, int yy, CirSim s) : base (xx,yy,s) {
			maxVoltage = 5;
			carrierfreq = 1000;
			signalfreq = 40;
			reset();
		}

		public override int getDumpType() {
			return 200;
		}

		public override String dump() {
			return base.dump() + " " + carrierfreq + " " + signalfreq + " " + maxVoltage;
		}

		/*
		 * void setCurrent(double c) { current = c;
		 * System.out.print("v current set to " + c + "\n"); }
		 */

		public override void reset() {
			freqTimeZero = 0;
			curcount = 0;
		}

		public override int getPostCount() {
			return 1;
		}

		public override void stamp() {
			sim.stampVoltageSource(0, nodes[0], voltSource);
		}

		public override void doStep() {
			sim.updateVoltageSource(0, nodes[0], voltSource, getVoltage());
		}

		public double getVoltage() {
			double w = 2 * pi * (sim.t - freqTimeZero);
			return ((Math.Sin(w * signalfreq) + 1) / 2) * Math.Sin(w * carrierfreq) * maxVoltage;
		}

		public int circleSize = 17;

		/*public override void draw(Graphics g) {
			setBbox(point1, point2, circleSize);
			setVoltageColor(g, volts[0]);
			drawThickLine(g, point1, lead1);

			Font f = new Font("SansSerif", 0, 12);
			g.setFont(f);
			g.setColor(needsHighlight() ? selectColor : whiteColor);
			setPowerColor(g, false);
			getVoltage();
			String s = "AM";
			drawCenteredText(g, s, x2, y2, true);
			drawWaveform(g, point2);
			drawPosts(g);
			curcount = updateDotCount(-current, curcount);
			if (sim.dragElm != this) {
				drawDots(g, point1, lead1, curcount);
			}
		}

		public override void drawWaveform(Graphics g, Point center) {
			g.setColor(needsHighlight() ? selectColor : Color.gray);
			setPowerColor(g, false);
			int xc = center.x;
			int yc = center.y;
			drawThickCircle(g, xc, yc, circleSize);
			adjustBbox(xc - circleSize, yc - circleSize, xc + circleSize, yc
					+ circleSize);
		}*/

		public override void setPoints() {
			base.setPoints();
			lead1 = interpPoint(point1, point2, 1 - circleSize / dn);
		}

		public override double getVoltageDiff() {
			return volts[0];
		}

		public override bool hasGroundConnection(int n1) {
			return true;
		}

		public override int getVoltageSourceCount() {
			return 1;
		}

		public override double getPower() {
			return -getVoltageDiff() * current;
		}

		public override void getInfo(String[] arr) {
			arr[0] = "AM Source";
			arr[1] = "I = " + getCurrentText(getCurrent());
			arr[2] = "V = " + getVoltageText(getVoltageDiff());
			arr[3] = "cf = " + getUnitText(carrierfreq, "Hz");
			arr[4] = "sf = " + getUnitText(signalfreq, "Hz");
			arr[5] = "Vmax = " + getVoltageText(maxVoltage);
		}

		/*public EditInfo getEditInfo(int n) {
			if (n == 0) {
				return new EditInfo("Max Voltage", maxVoltage, -20, 20);
			}
			if (n == 1) {
				return new EditInfo("Carrier Frequency (Hz)", carrierfreq, 4, 500);
			}
			if (n == 2) {
				return new EditInfo("Signal Frequency (Hz)", signalfreq, 4, 500);
			}

			return null;
		}

		public void setEditValue(int n, EditInfo ei) {
			if (n == 0) {
				maxVoltage = ei.value;
			}
			if (n == 1) {
				carrierfreq = ei.value;
			}
			if (n == 2) {
				signalfreq = ei.value;
			}
		}*/
	}
}