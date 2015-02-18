using System;
using System.Collections;
using System.Collections.Generic;

namespace SharpCircuit {

	// Contributed by Edward Calver.

	public class InvertingSchmittElm : CircuitElement {

		public Circuit.Lead leadIn { get { return lead0; } }
		public Circuit.Lead leadOut { get { return lead1; } }

		/// <summary>
		/// Slew Rate (V/ns)
		/// </summary>
		public double slewRate { get; private set; }

		/// <summary>
		/// Lower threshold (V)
		/// </summary>
		public double lowerTrigger { get; private set; }

		/// <summary>
		/// Upper threshold (V)
		/// </summary>
		public double upperTrigger { get; private set; }

		protected bool state;

		public InvertingSchmittElm() : base() {
			slewRate = 0.5;
			state = false;
			lowerTrigger = 1.66;
			upperTrigger = 3.33;
		}

		public override int getVoltageSourceCount() {
			return 1;
		}

		public override void stamp(Circuit sim) {
			sim.stampVoltageSource(0, lead_node[1], voltSource);
		}

		public override void doStep(Circuit sim) {
			double v0 = lead_volt[1];
			double @out;
			if(state) {
				// Output is high
				if(lead_volt[0] > upperTrigger) {
					// Input voltage high enough to set output low
					state = false;
					@out = 0;
				} else {
					@out = 5;
				}
			} else {
				// Output is low
				if(lead_volt[0] < lowerTrigger) {
					// Input voltage low enough to set output high
					state = true;
					@out = 5;
				} else {
					@out = 0;
				}
			}

			double maxStep = slewRate * sim.timeStep * 1e9;
			@out = Math.Max(Math.Min(v0 + maxStep, @out), v0 - maxStep);
			sim.updateVoltageSource(0, lead_node[1], voltSource, @out);
		}

		public override double getVoltageDiff() {
			return lead_volt[0];
		}

		public override void getInfo(String[] arr) {
			arr[0] = "InvertingSchmitt";
			arr[1] = "Vi = " + getVoltageText(lead_volt[0]);
			arr[2] = "Vo = " + getVoltageText(lead_volt[1]);
		}

		// There is no current path through the InvertingSchmitt input, but there
		// is an indirect path through the output to ground.
		public override bool leadsAreConnected(int n1, int n2) {
			return false;
		}

		public override bool leadIsGround(int n1) {
			return (n1 == 1);
		}
	}
}