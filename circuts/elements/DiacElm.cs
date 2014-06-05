// stub implementation of DiacElm, based on SparkGapElm
// FIXME need to add DiacElm.java to srclist
// FIXME need to uncomment DiacElm line from CirSim.java

using System;
using System.Collections;
using System.Collections.Generic;

namespace Circuts {

	public class DiacElm : CircuitElm {
		public double onresistance, offresistance, breakdown, holdcurrent;
		public bool state;

		public DiacElm(int xx, int yy, CirSim s) : base(xx, yy, s) {
			// FIXME need to adjust defaults to make sense for diac
			offresistance = 1e9;
			onresistance = 1e3;
			breakdown = 1e3;
			holdcurrent = 0.001;
			state = false;
		}

		public override bool nonLinear() {
			return true;
		}

		public Point ps3, ps4;

		public override void setPoints() {
			base.setPoints();
			calcLeads(32);
			ps3 = new Point();
			ps4 = new Point();
		}

		/*public override void draw(Graphics g) {
			setBbox(point1, point2, 6);
			draw2Leads(g);
			setPowerColor(g, true);
			doDots(g);
			drawPosts(g);
		}*/

		public override void calculateCurrent() {
			double vd = volts[0] - volts[1];
			if (state) {
				current = vd / onresistance;
			} else {
				current = vd / offresistance;
			}
		}

		public override void startIteration() {
			double vd = volts[0] - volts[1];
			if (Math.Abs(current) < holdcurrent) {
				state = false;
			}
			if (Math.Abs(vd) > breakdown) {
				state = true;
				// System.out.print(this + " res current set to " + current + "\n");
			}
		}

		public override void doStep() {
			if (state) {
				sim.stampResistor(nodes[0], nodes[1], onresistance);
			} else {
				sim.stampResistor(nodes[0], nodes[1], offresistance);
			}
		}

		public override void stamp() {
			sim.stampNonLinear(nodes[0]);
			sim.stampNonLinear(nodes[1]);
		}

		public override void getInfo(String[] arr) {
			// FIXME
			arr[0] = "spark gap";
			getBasicInfo(arr);
			arr[3] = state ? "on" : "off";
			arr[4] = "Ron = " + getUnitText(onresistance, CirSim.ohmString);
			arr[5] = "Roff = " + getUnitText(offresistance, CirSim.ohmString);
			arr[6] = "Vbrkdn = " + getUnitText(breakdown, "V");
			arr[7] = "Ihold = " + getUnitText(holdcurrent, "A");
		}

		/*public EditInfo getEditInfo(int n) {
			if (n == 0) {
				return new EditInfo("On resistance (ohms)", onresistance, 0, 0);
			}
			if (n == 1) {
				return new EditInfo("Off resistance (ohms)", offresistance, 0, 0);
			}
			if (n == 2) {
				return new EditInfo("Breakdown voltage (volts)", breakdown, 0, 0);
			}
			if (n == 3) {
				return new EditInfo("Hold current (amps)", holdcurrent, 0, 0);
			}
			return null;
		}

		public void setEditValue(int n, EditInfo ei) {
			if (ei.value > 0 && n == 0) {
				onresistance = ei.value;
			}
			if (ei.value > 0 && n == 1) {
				offresistance = ei.value;
			}
			if (ei.value > 0 && n == 2) {
				breakdown = ei.value;
			}
			if (ei.value > 0 && n == 3) {
				holdcurrent = ei.value;
			}
		}*/
	}
}