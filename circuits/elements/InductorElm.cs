using System;
using System.Collections;
using System.Collections.Generic;

namespace Circuits {

	public class InductorElm : CircuitElement {

		public Inductor ind;

		/// <summary>
		/// Inductance (H)
		/// </summary>
		public double Inductance{ get; set; }

		public InductorElm(CirSim s) : base(s) {
			ind = new Inductor(sim);
			Inductance = 1;
			ind.setup(Inductance, current, flags);
		}

		public override void reset() {
			current = volts[0] = volts[1] = 0;
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
			arr[3] = "L = " + getUnitText(Inductance, "H");
			arr[4] = "P = " + getUnitText(getPower(), "W");
		}

		/*public EditInfo getEditInfo(int n) {
			if (n == 1) {
				EditInfo ei = new EditInfo("", 0, -1, -1);
				ei.checkbox = new Checkbox("Trapezoidal Approximation",ind.isTrapezoidal());
				return ei;
			}
			return null;
		}

		public void setEditValue(int n, EditInfo ei) {
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