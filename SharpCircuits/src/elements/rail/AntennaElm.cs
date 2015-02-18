using System;
using System.Collections;
using System.Collections.Generic;

namespace SharpCircuit {

	public class AntennaElm : RailElm {

		private double fmphase;

		public AntennaElm() : base(WaveType.DC) {
			
		}

		public override void stamp(Circuit sim) {
			sim.stampVoltageSource(0, lead_node[0], voltSource);
		}

		public override void step(Circuit sim) {
			sim.updateVoltageSource(0, lead_node[0], voltSource, getVoltage(sim));
		}

		public override double getVoltage(Circuit sim) {
			fmphase += 2 * pi * (2200 + Math.Sin(2 * pi * sim.time * 13) * 100) * sim.timeStep;
			double fm = 3 * Math.Sin(fmphase);
			return Math.Sin(2 * pi * sim.time * 3000)
					* (1.3 + Math.Sin(2 * pi * sim.time * 12)) * 3
					+ Math.Sin(2 * pi * sim.time * 2710)
					* (1.3 + Math.Sin(2 * pi * sim.time * 13)) * 3
					+ Math.Sin(2 * pi * sim.time * 2433)
					* (1.3 + Math.Sin(2 * pi * sim.time * 14)) * 3 + fm;
		}

	}
}