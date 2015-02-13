using System;
using System.Collections;
using System.Collections.Generic;

namespace SharpCircuit {
	
	public class InductorElm : CircuitElement {

		//public ElementLead leadIn 	{ get { return lead0; }}
		//public ElementLead leadOut 	{ get { return lead1; }}

		public Inductor ind;

		/// <summary>
		/// Inductance (H)
		/// </summary>
		public double inductance {
			get {
				return _inductance;
			}
			set {
				_inductance = value;
				ind.setup(_inductance,current,true);
			}
		}

		private double _inductance;

		public InductorElm() {
			ind = new Inductor(sim);
			inductance = 1;
		}

		public InductorElm(double induc) {
			ind = new Inductor(sim);
			inductance = induc;
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