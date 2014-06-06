using System;
using System.Collections;
using System.Collections.Generic;

namespace Circuits {

	public class RailElm : VoltageElm {
		
		public  int FLAG_CLOCK = 1;

		public RailElm(int xx, int yy, int wf, CirSim s) : base(xx, yy, wf, s) {

		}

		public override int getPostCount() {
			return 1;
		}

//		public override void setPoints() {
//			base.setPoints();
//			lead1 = interpPoint(point1, point2, 1 - circleSize / dn);
//		}

		/*public override void draw(Graphics g) {
			setBbox(point1, point2, circleSize);
			setVoltageColor(g, volts[0]);
			drawThickLine(g, point1, lead1);
			boolean clock = waveform == WF_SQUARE && (flags & FLAG_CLOCK) != 0;
			if (waveform == WF_DC || waveform == WF_VAR || clock) {
				Font f = new Font("SansSerif", 0, 12);
				g.setFont(f);
				g.setColor(needsHighlight() ? selectColor : whiteColor);
				setPowerColor(g, false);
				double v = getVoltage();
				String s = getShortUnitText(v, "V");
				if (Math.abs(v) < 1) {
					s = showFormat.format(v) + "V";
				}
				if (getVoltage() > 0) {
					s = "+" + s;
				}
				if (this is AntennaElm) {
					s = "Ant";
				}
				if (clock) {
					s = "CLK";
				}
				drawCenteredText(g, s, x2, y2, true);
			} else {
				drawWaveform(g, point2);
			}
			drawPosts(g);
			curcount = updateDotCount(-current, curcount);
			if (sim.dragElm != this) {
				drawDots(g, point1, lead1, curcount);
			}
		}*/

		public override double getVoltageDiff() {
			return volts[0];
		}

		public override void stamp() {
			if (waveform == VoltageElm.WF_DC) {
				sim.stampVoltageSource(0, nodes[0], voltSource, getVoltage());
			} else {
				sim.stampVoltageSource(0, nodes[0], voltSource);
			}
		}

		public override void doStep() {
			if (waveform != VoltageElm.WF_DC) {
				sim.updateVoltageSource(0, nodes[0], voltSource, getVoltage());
			}
		}

		public override bool hasGroundConnection(int n1) {
			return true;
		}

	}
}