// stub ThermistorElm based on SparkGapElm
// FIXME need to uncomment ThermistorElm line from CirSim.java
// FIXME need to add ThermistorElm.java to srclist

using System;
using System.Collections;
using System.Collections.Generic;

namespace Circuits {

	public class ThermistorElm : CircuitElm {
		public double minresistance, maxresistance;
		public double resistance;
		public double slider;

		public ThermistorElm(int xx, int yy, CirSim s) : base(xx, yy, s) {
			maxresistance = 1e9;
			minresistance = 1e3;
		}

		public override bool nonLinear() {
			return true;
		}

//		public override void setPoints() {
//			base.setPoints();
//			calcLeads(32);
//			ps3 = new Point();
//			ps4 = new Point();
//		}

		/*public override void draw(Graphics g) {
			setBbox(point1, point2, 6);
			draw2Leads(g);
			// FIXME need to draw properly, see ResistorElm.java
			setPowerColor(g, true);
			doDots(g);
			drawPosts(g);
		}*/

		public override void calculateCurrent() {
			double vd = volts[0] - volts[1];
			current = vd / resistance;
		}

		public override void startIteration() {
			// FIXME set resistance as appropriate, using slider.getValue()
			resistance = minresistance;
			// System.out.print(this + " res current set to " + current + "\n");
		}

		public override void doStep() {
			sim.stampResistor(nodes[0], nodes[1], resistance);
		}

		public override void stamp() {
			sim.stampNonLinear(nodes[0]);
			sim.stampNonLinear(nodes[1]);
		}

		public override void getInfo(String[] arr) {
			// FIXME
			arr[0] = "spark gap";
			getBasicInfo(arr);
			arr[3] = "R = " + getUnitText(resistance, CirSim.ohmString);
			arr[4] = "Ron = " + getUnitText(minresistance, CirSim.ohmString);
			arr[5] = "Roff = " + getUnitText(maxresistance, CirSim.ohmString);
		}

		/*public EditInfo getEditInfo(int n) {
			// ohmString doesn't work here on linux
			if (n == 0) {
				return new EditInfo("Min resistance (ohms)", minresistance, 0, 0);
			}
			if (n == 1) {
				return new EditInfo("Max resistance (ohms)", maxresistance, 0, 0);
			}
			return null;
		}

		public void setEditValue(int n, EditInfo ei) {
			if (ei.value > 0 && n == 0) {
				minresistance = ei.value;
			}
			if (ei.value > 0 && n == 1) {
				maxresistance = ei.value;
			}
		}*/
	}
}