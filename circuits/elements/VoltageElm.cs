using System;
using System.Collections;
using System.Collections.Generic;

namespace Circuits {

	public class VoltageElm : CircuitElement {
		public static int FLAG_COS = 2;
		public int waveform;
		public static int WF_DC = 0;
		public static int WF_AC = 1;
		public static int WF_SQUARE = 2;
		public static int WF_TRIANGLE = 3;
		public static int WF_SAWTOOTH = 4;
		public static int WF_PULSE = 5;
		public static int WF_VAR = 6;
		public double frequency, maxVoltage, freqTimeZero, bias, phaseShift, dutyCycle;

		public VoltageElm( int wf, CirSim s) : base(s) {
			waveform = wf;
			maxVoltage = 5;
			frequency = 40;
			dutyCycle = 0.5;
			reset();
		}

		/*
		 * void setCurrent(double c) { current = c;
		 * System.out.print("v current set to " + c + "\n"); }
		 */

		public override void reset() {
			freqTimeZero = 0;
		}

		public double triangleFunc(double x) {
			if (x < pi) {
				return x * (2 / pi) - 1;
			}
			return 1 - (x - pi) * (2 / pi);
		}

		public override void stamp() {
			if (waveform == WF_DC) {
				sim.stampVoltageSource(nodes[0], nodes[1], voltSource, getVoltage());
			} else {
				sim.stampVoltageSource(nodes[0], nodes[1], voltSource);
			}
		}

		public override void doStep() {
			if (waveform != WF_DC) {
				sim.updateVoltageSource(nodes[0], nodes[1], voltSource, getVoltage());
			}
		}

		public virtual double getVoltage() {
			double w = 2 * pi * (sim.time - freqTimeZero) * frequency + phaseShift;
			switch (waveform) {
			case 0:
				return maxVoltage + bias;
			case 1:
				return Math.Sin(w) * maxVoltage + bias;
			case 2:
				return bias + ((w % (2 * pi) > (2 * pi * dutyCycle)) ? -maxVoltage : maxVoltage);
			case 3:
				return bias + triangleFunc(w % (2 * pi)) * maxVoltage;
			case 4:
				return bias + (w % (2 * pi)) * (maxVoltage / pi) - maxVoltage;
			case 5:
				return ((w % (2 * pi)) < 1) ? maxVoltage + bias : bias;
			default:
				return 0;
			}
		}

		public int circleSize = 17;

//		public override void setPoints() {
//			base.setPoints();
//			calcLeads((waveform == WF_DC || waveform == WF_VAR) ? 8 : circleSize * 2);
//		}

		/*public void draw(Graphics g) {
			setBbox(x, y, x2, y2);
			draw2Leads(g);
			if (waveform == WF_DC) {
				setPowerColor(g, false);
				setVoltageColor(g, volts[0]);
				interpPoint2(lead1, lead2, ps1, ps2, 0, 10);
				drawThickLine(g, ps1, ps2);
				setVoltageColor(g, volts[1]);
				int hs = 16;
				setBbox(point1, point2, hs);
				interpPoint2(lead1, lead2, ps1, ps2, 1, hs);
				drawThickLine(g, ps1, ps2);
			} else {
				setBbox(point1, point2, circleSize);
				interpPoint(lead1, lead2, ps1, .5);
				drawWaveform(g, ps1);
			}
			updateDotCount();
			if (sim.dragElm != this) {
				if (waveform == WF_DC) {
					drawDots(g, point1, point2, curcount);
				} else {
					drawDots(g, point1, lead1, curcount);
					drawDots(g, point2, lead2, -curcount);
				}
			}
			drawPosts(g);
		}*/

		/*public void drawWaveform(Graphics g, Point center) {
			g.setColor(needsHighlight() ? selectColor : Color.gray);
			setPowerColor(g, false);
			int xc = center.x;
			int yc = center.y;
			drawThickCircle(g, xc, yc, circleSize);
			int wl = 8;
			adjustBbox(xc - circleSize, yc - circleSize, xc + circleSize, yc
					+ circleSize);
			int xc2;
			switch (waveform) {
			case WF_DC: {
				break;
			}
			case WF_SQUARE:
				xc2 = (int) (wl * 2 * dutyCycle - wl + xc);
				xc2 = max(xc - wl + 3, min(xc + wl - 3, xc2));
				drawThickLine(g, xc - wl, yc - wl, xc - wl, yc);
				drawThickLine(g, xc - wl, yc - wl, xc2, yc - wl);
				drawThickLine(g, xc2, yc - wl, xc2, yc + wl);
				drawThickLine(g, xc + wl, yc + wl, xc2, yc + wl);
				drawThickLine(g, xc + wl, yc, xc + wl, yc + wl);
				break;
			case WF_PULSE:
				yc += wl / 2;
				drawThickLine(g, xc - wl, yc - wl, xc - wl, yc);
				drawThickLine(g, xc - wl, yc - wl, xc - wl / 2, yc - wl);
				drawThickLine(g, xc - wl / 2, yc - wl, xc - wl / 2, yc);
				drawThickLine(g, xc - wl / 2, yc, xc + wl, yc);
				break;
			case WF_SAWTOOTH:
				drawThickLine(g, xc, yc - wl, xc - wl, yc);
				drawThickLine(g, xc, yc - wl, xc, yc + wl);
				drawThickLine(g, xc, yc + wl, xc + wl, yc);
				break;
			case WF_TRIANGLE: {
				int xl = 5;
				drawThickLine(g, xc - xl * 2, yc, xc - xl, yc - wl);
				drawThickLine(g, xc - xl, yc - wl, xc, yc);
				drawThickLine(g, xc, yc, xc + xl, yc + wl);
				drawThickLine(g, xc + xl, yc + wl, xc + xl * 2, yc);
				break;
			}
			case WF_AC: {
				int i;
				int xl = 10;
				int ox = -1, oy = -1;
				for (i = -xl; i <= xl; i++) {
					int yy = yc + (int) (.95 * Math.sin(i * pi / xl) * wl);
					if (ox != -1) {
						drawThickLine(g, ox, oy, xc + i, yy);
					}
					ox = xc + i;
					oy = yy;
				}
				break;
			}
			}
			if (sim.showValuesCheckItem.getState()) {
				String s = getShortUnitText(frequency, "Hz");
				if (dx == 0 || dy == 0) {
					drawValues(g, s, circleSize);
				}
			}
		}*/

		public override int getVoltageSourceCount() {
			return 1;
		}

		public override double getPower() {
			return -getVoltageDiff() * current;
		}

		public override double getVoltageDiff() {
			return volts[1] - volts[0];
		}

		public override void getInfo(String[] arr) {
			switch (waveform) {
			case 0:
			case 6:
				arr[0] = "voltage source";
				break;
			case 1:
				arr[0] = "A/C source";
				break;
			case 2:
				arr[0] = "square wave gen";
				break;
			case 5:
				arr[0] = "pulse gen";
				break;
			case 4:
				arr[0] = "sawtooth gen";
				break;
			case 3:
				arr[0] = "triangle gen";
				break;
			}
			arr[1] = "I = " + getCurrentText(getCurrent());
			arr[2] = ((this is RailElm) ? "V = " : "Vd = ") + getVoltageText(getVoltageDiff());
			if (waveform != WF_DC && waveform != WF_VAR) {
				arr[3] = "f = " + getUnitText(frequency, "Hz");
				arr[4] = "Vmax = " + getVoltageText(maxVoltage);
				int i = 5;
				if (bias != 0) {
					arr[i++] = "Voff = " + getVoltageText(bias);
				} else if (frequency > 500) {
					arr[i++] = "wavelength = "
							+ getUnitText(2.9979e8 / frequency, "m");
				}
				arr[i++] = "P = " + getUnitText(getPower(), "W");
			}
		}

		/*public EditInfo getEditInfo(int n) {
			if (n == 0) {
				return new EditInfo(waveform == WF_DC ? "Voltage" : "Max Voltage",
						maxVoltage, -20, 20);
			}
			if (n == 1) {
				EditInfo ei = new EditInfo("Waveform", waveform, -1, -1);
				ei.choice = new Choice();
				ei.choice.add("D/C");
				ei.choice.add("A/C");
				ei.choice.add("Square Wave");
				ei.choice.add("Triangle");
				ei.choice.add("Sawtooth");
				ei.choice.add("Pulse");
				ei.choice.select(waveform);
				return ei;
			}
			if (waveform == WF_DC) {
				return null;
			}
			if (n == 2) {
				return new EditInfo("Frequency (Hz)", frequency, 4, 500);
			}
			if (n == 3) {
				return new EditInfo("DC Offset (V)", bias, -20, 20);
			}
			if (n == 4) {
				return new EditInfo("Phase Offset (degrees)",
						phaseShift * 180 / pi, -180, 180).setDimensionless();
			}
			if (n == 5 && waveform == WF_SQUARE) {
				return new EditInfo("Duty Cycle", dutyCycle * 100, 0, 100)
						.setDimensionless();
			}
			return null;
		}

		public void setEditValue(int n, EditInfo ei) {
			if (n == 0) {
				maxVoltage = ei.value;
			}
			if (n == 3) {
				bias = ei.value;
			}
			if (n == 2) {
				// adjust time zero to maintain continuity ind the waveform
				// even though the frequency has changed.
				double oldfreq = frequency;
				frequency = ei.value;
				double maxfreq = 1 / (8 * sim.timeStep);
				if (frequency > maxfreq) {
					frequency = maxfreq;
				}
				freqTimeZero = sim.t - oldfreq * (sim.t - freqTimeZero) / frequency;
			}
			if (n == 1) {
				int ow = waveform;
				waveform = ei.choice.getSelectedIndex();
				if (waveform == WF_DC && ow != WF_DC) {
					ei.newDialog = true;
					bias = 0;
				} else if (waveform != WF_DC && ow == WF_DC) {
					ei.newDialog = true;
				}
				if ((waveform == WF_SQUARE || ow == WF_SQUARE) && waveform != ow) {
					ei.newDialog = true;
				}
				setPoints();
			}
			if (n == 4) {
				phaseShift = ei.value * pi / 180;
			}
			if (n == 5) {
				dutyCycle = ei.value * .01;
			}
		}*/
	}
}