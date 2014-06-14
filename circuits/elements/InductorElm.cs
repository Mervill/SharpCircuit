using System;
using System.Collections;
using System.Collections.Generic;

namespace Circuits {

	// Initializers	[X]
	// Properties	[X]
	// Leads		[X]
	// Test Basic	[X]
	// Test Prop	[_]
	public class InductorElm : CircuitElement {

		public Inductor ind;

		/// <summary>
		/// Inductance (H)
		/// </summary>
		[System.ComponentModel.DefaultValue(1)]
		public double inductance{ get; set; }

		public InductorElm(CirSim s) : base(s) {
			ind = new Inductor(sim);
			ind.setup(inductance, current, true);
		}

		public InductorElm(CirSim s,double induc) : base(s) {
			ind = new Inductor(sim);
			ind.setup(induc, current, true);
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
			arr[3] = "L = " + getUnitText(inductance, "H");
			arr[4] = "P = " + getUnitText(getPower(), "W");
		}

	}
}