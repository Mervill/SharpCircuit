using System;
using System.Collections;
using System.Collections.Generic;

namespace SharpCircuit {

	public class TunnelDiodeElm : CircuitElement {

		private static readonly double pvp = 0.1;
		private static readonly double pip = 4.7e-3;
		private static readonly double pvv = 0.37;
		private static readonly double pvt = 0.026;
		private static readonly double pvpp = 0.525;
		private static readonly double piv = 370e-6;

		public Circuit.Lead leadIn { get { return lead0; } }
		public Circuit.Lead leadOut { get { return lead1; } }

		private double lastvoltdiff;
		
		public override bool nonLinear() {
			return true;
		}

		public override void reset() {
			lastvoltdiff = lead_volt[0] = lead_volt[1] = 0;
		}

		public double limitStep(double vnew, double vold) {
			// Prevent voltage changes of more than 1V when iterating. Wow, I
			// thought it would be much harder than this to prevent convergence problems.
			if(vnew > vold + 1) return vold + 1;
			if(vnew < vold - 1) return vold - 1;
			return vnew;
		}

		public override void stamp(Circuit sim) {
			sim.stampNonLinear(lead_node[0]);
			sim.stampNonLinear(lead_node[1]);
		}

		public override void step(Circuit sim) {
			double voltdiff = lead_volt[0] - lead_volt[1];
			if(Math.Abs(voltdiff - lastvoltdiff) > 0.01)
				sim.converged = false;
			voltdiff = limitStep(voltdiff, lastvoltdiff);
			lastvoltdiff = voltdiff;
			double i = pip * Math.Exp(-pvpp / pvt) * (Math.Exp(voltdiff / pvt) - 1)
					+ pip * (voltdiff / pvp) * Math.Exp(1 - voltdiff / pvp) + piv
					* Math.Exp(voltdiff - pvv);
			double geq = pip * Math.Exp(-pvpp / pvt) * Math.Exp(voltdiff / pvt)
					/ pvt + pip * Math.Exp(1 - voltdiff / pvp) / pvp
					- Math.Exp(1 - voltdiff / pvp) * pip * voltdiff / (pvp * pvp)
					+ Math.Exp(voltdiff - pvv) * piv;
			double nc = i - geq * voltdiff;
			sim.stampConductance(lead_node[0], lead_node[1], geq);
			sim.stampCurrentSource(lead_node[0], lead_node[1], nc);
		}

		public override void calculateCurrent() {
			double voltdiff = lead_volt[0] - lead_volt[1];
			current = pip * Math.Exp(-pvpp / pvt) * (Math.Exp(voltdiff / pvt) - 1)
					+ pip * (voltdiff / pvp) * Math.Exp(1 - voltdiff / pvp) + piv
					* Math.Exp(voltdiff - pvv);
		}

		/*public override void getInfo(String[] arr) {
			arr[0] = "tunnel diode";
			arr[1] = "I = " + getCurrentText(current);
			arr[2] = "Vd = " + getVoltageText(getVoltageDiff());
			arr[3] = "P = " + getUnitText(getPower(), "W");
		}*/
	}
}