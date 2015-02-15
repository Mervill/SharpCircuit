using System;
using System.Collections;
using System.Collections.Generic;

namespace SharpCircuit {

	public class InductorElm : CircuitElement {

		public Circuit.Lead leadIn { get { return lead0; } }
		public Circuit.Lead leadOut { get { return lead1; } }

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
				ind.setup(_inductance, current, true);
			}
		}

		private double _inductance;

		public InductorElm() : base() {
			ind = new Inductor();
			inductance = 1;
		}

		public InductorElm(double induc) : base() {
			ind = new Inductor();
			inductance = induc;
		}

		public override void reset() {
			current = lead_volt[0] = lead_volt[1] = 0;
			ind.reset();
		}

		public override void stamp(Circuit sim) {
			ind.stamp(sim, lead_node[0], lead_node[1]);
		}

		public override void startIteration(double timeStep) {
			ind.startIteration(lead_volt[0] - lead_volt[1]);
		}

		public override bool nonLinear() {
			return ind.nonLinear();
		}

		public override void calculateCurrent() {
			double voltdiff = lead_volt[0] - lead_volt[1];
			current = ind.calculateCurrent(voltdiff);
		}

		public override void doStep(Circuit sim) {
			double voltdiff = lead_volt[0] - lead_volt[1];
			ind.doStep(sim, voltdiff);
		}

		public override void getInfo(String[] arr) {
			arr[0] = "inductor";
			getBasicInfo(arr);
			arr[3] = "L = " + getUnitText(inductance, "H");
			arr[4] = "P = " + getUnitText(getPower(), "W");
		}

	}
}