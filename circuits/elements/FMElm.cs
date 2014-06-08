using System;
using System.Collections;
using System.Collections.Generic;

namespace Circuits {

	// contributed by Edward Calver

	public class FMElm : CircuitElement {
		static int FLAG_COS = 2;
		double carrierfreq, signalfreq, maxVoltage, freqTimeZero, deviation;
		double lasttime = 0;
		double funcx = 0;

		public FMElm(int xx, int yy, CirSim s) : base (xx, yy, s) {
			deviation = 200;
			maxVoltage = 5;
			carrierfreq = 800;
			signalfreq = 40;
			reset();
		}

		/*
		 * void setCurrent(double c) { current = c;
		 * System.out.print("v current set to " + c + "\n"); }
		 */

		public override void reset() {
			freqTimeZero = 0;
			curcount = 0;
		}

		public override int getLeadCount() {
			return 1;
		}

		public override void stamp() {
			sim.stampVoltageSource(0, nodes[0], voltSource);
		}

		public override void doStep() {
			sim.updateVoltageSource(0, nodes[0], voltSource, getVoltage());
		}

		double getVoltage() {
			double deltaT = sim.t - lasttime;
			lasttime = sim.t;
			double signalamplitude = Math.Sin((2 * pi * (sim.t - freqTimeZero)) * signalfreq);
			funcx += deltaT * (carrierfreq + (signalamplitude * deviation));
			double w = 2 * pi * funcx;
			return Math.Sin(w) * maxVoltage;
		}

		public int circleSize = 17;

		/*public override void void draw(Graphics g) {
			setBbox(point1, point2, circleSize);
			setVoltageColor(g, volts[0]);
			drawThickLine(g, point1, lead1);

			Font f = new Font("SansSerif", 0, 12);
			g.setFont(f);
			g.setColor(needsHighlight() ? selectColor : whiteColor);
			setPowerColor(g, false);
			getVoltage();
			String s = "FM";
			drawCenteredText(g, s, x2, y2, true);
			drawWaveform(g, point2);
			drawPosts(g);
			curcount = updateDotCount(-current, curcount);
			if (sim.dragElm != this) {
				drawDots(g, point1, lead1, curcount);
			}
		}

		void drawWaveform(Graphics g, Point center) {
			g.setColor(needsHighlight() ? selectColor : Color.gray);
			setPowerColor(g, false);
			int xc = center.x;
			int yc = center.y;
			drawThickCircle(g, xc, yc, circleSize);
			adjustBbox(xc - circleSize, yc - circleSize, xc + circleSize, yc
					+ circleSize);
		}*/

//		public override void setPoints() {
//			base.setPoints();
//			lead1 = interpPoint(point1, point2, 1 - circleSize / dn);
//		}

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

			arr[0] = "FM Source";
			arr[1] = "I = " + getCurrentText(getCurrent());
			arr[2] = "V = " + getVoltageText(getVoltageDiff());
			arr[3] = "cf = " + getUnitText(carrierfreq, "Hz");
			arr[4] = "sf = " + getUnitText(signalfreq, "Hz");
			arr[5] = "dev =" + getUnitText(deviation, "Hz");
			arr[6] = "Vmax = " + getVoltageText(maxVoltage);
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
			if (n == 3) {
				return new EditInfo("Deviation (Hz)", deviation, 4, 500);
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
			if (n == 3) {
				deviation = ei.value;
			}
		}*/
	}
}