using System;
using System.Collections;
using System.Collections.Generic;

namespace Circuts {

	public class AntennaElm : RailElm {
		
		public AntennaElm(int xx, int yy, CirSim s) : base(xx, yy, WF_DC, s) {
			
		}

		public double fmphase;

		public override void stamp() {
			sim.stampVoltageSource(0, nodes[0], voltSource);
		}

		public override void doStep() {
			sim.updateVoltageSource(0, nodes[0], voltSource, getVoltage());
		}

		public override double getVoltage() {
			fmphase += 2 * pi * (2200 + Math.Sin(2 * pi * sim.t * 13) * 100)
					* sim.timeStep;
			double fm = 3 * Math.Sin(fmphase);
			return Math.Sin(2 * pi * sim.t * 3000)
					* (1.3 + Math.Sin(2 * pi * sim.t * 12)) * 3
					+ Math.Sin(2 * pi * sim.t * 2710)
					* (1.3 + Math.Sin(2 * pi * sim.t * 13)) * 3
					+ Math.Sin(2 * pi * sim.t * 2433)
					* (1.3 + Math.Sin(2 * pi * sim.t * 14)) * 3 + fm;
		}

	}
}