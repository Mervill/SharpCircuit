using System;
using System.Collections;
using System.Collections.Generic;

namespace Circuits {

	public class InductorElm : CircuitElm {
		public Inductor ind;
		public double inductance;

		public InductorElm(int xx, int yy,CirSim s) : base(xx, yy, s) {
			ind = new Inductor(sim);
			inductance = 1;
			ind.setup(inductance, current, flags);
		}

//		public override void setPoints() {
//			base.setPoints();
//			calcLeads(32);
//		}

		/*public override void draw(Graphics g) {
			double v1 = volts[0];
			double v2 = volts[1];
			int hs = 8;
			setBbox(point1, point2, hs);
			draw2Leads(g);
			setPowerColor(g, false);
			drawCoil(g, 8, lead1, lead2, v1, v2);
			if (sim.showValuesCheckItem.getState()) {
				String s = getShortUnitText(inductance, "H");
				drawValues(g, s, hs);
			}
			doDots(g);
			drawPosts(g);
		}*/

		public override void reset() {
			current = volts[0] = volts[1] = curcount = 0;
			ind.reset();
		}

		public override void stamp() {
			ind.stamp(nodes[0], nodes[1]);
		}

		public override void startIteration() {
			ind.startIteration(volts[0] - volts[1]);
		}

		public override bool nonLinear() {
			return ind.nonLinear();
		}

		public override void calculateCurrent() {
			double voltdiff = volts[0] - volts[1];
			current = ind.calculateCurrent(voltdiff);
		}

		public override void doStep() {
			double voltdiff = volts[0] - volts[1];
			ind.doStep(voltdiff);
		}

		public override void getInfo(String[] arr) {
			arr[0] = "inductor";
			getBasicInfo(arr);
			arr[3] = "L = " + getUnitText(inductance, "H");
			arr[4] = "P = " + getUnitText(getPower(), "W");
		}

		/*public EditInfo getEditInfo(int n) {
			if (n == 0) {
				return new EditInfo("Inductance (H)", inductance, 0, 0);
			}
			if (n == 1) {
				EditInfo ei = new EditInfo("", 0, -1, -1);
				ei.checkbox = new Checkbox("Trapezoidal Approximation",
						ind.isTrapezoidal());
				return ei;
			}
			return null;
		}

		public void setEditValue(int n, EditInfo ei) {
			if (n == 0) {
				inductance = ei.value;
			}
			if (n == 1) {
				if (ei.checkbox.getState()) {
					flags &= ~Inductor.FLAG_BACK_EULER;
				} else {
					flags |= Inductor.FLAG_BACK_EULER;
				}
			}
			ind.setup(inductance, current, flags);
		}*/
	}
}