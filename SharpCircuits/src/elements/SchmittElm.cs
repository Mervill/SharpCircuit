using System;
using System.Collections;
using System.Collections.Generic;

namespace SharpCircuit {

	// Contributed by Edward Calver.
	
	public class SchmittElm : InvertingSchmittElm {

		public SchmittElm() {

		}

		public override void doStep(CirSim sim) {
			double v0 = volts[1];
			double @out;
			if (state) {// Output is high
				if (volts[0] > upperTrigger)// Input voltage high enough to set
											// output high
				{
					state = false;
					@out = 5;
				} else {
					@out = 0;
				}
			} else {// Output is low
				if (volts[0] < lowerTrigger)// Input voltage low enough to set
											// output low
				{
					state = true;
					@out = 0;
				} else {
					@out = 5;
				}
			}

			double maxStep = slewRate * sim.timeStep * 1e9;
			@out = Math.Max(Math.Min(v0 + maxStep, @out), v0 - maxStep);
			sim.updateVoltageSource(0, nodes[1], voltSource, @out);
		}

		public override void getInfo(String[] arr) {
			arr[0] = "Schmitt";
		}

	}
}