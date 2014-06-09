using System;
using System.Collections;
using System.Collections.Generic;

namespace Circuits {

	public class TunnelDiodeElm : CircuitElement {
	
		private static readonly double pvp = 0.1;
		private static readonly double pip = 4.7e-3;
		private static readonly double pvv = 0.37;
		private static readonly double pvt = 0.026;
		private static readonly double pvpp = 0.525;
		private static readonly double piv = 370e-6;

		private double lastvoltdiff;

		public TunnelDiodeElm(CirSim s) : base(s) {

		}

		public override bool nonLinear() {
			return true;
		}

		public override void reset() {
			lastvoltdiff = volts[0] = volts[1] = 0;
		}

		public double limitStep(double vnew, double vold) {
			// Prevent voltage changes of more than 1V when iterating. Wow, I
			// thought it would be much harder than this to prevent convergence problems.
			if(vnew > vold + 1)
				return vold + 1;
			
			if(vnew < vold - 1)
				return vold - 1;
			
			return vnew;
		}

		public override void stamp() {
			sim.stampNonLinear(nodes[0]);
			sim.stampNonLinear(nodes[1]);
		}

		public override void doStep() {
			double voltdiff = volts[0] - volts[1];
			if (Math.Abs(voltdiff - lastvoltdiff) > 0.01) {
				sim.converged = false;
			}
			// System.out.println(voltdiff + " " + lastvoltdiff + " " + Math.abs(voltdiff-lastvoltdiff));
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
			sim.stampConductance(nodes[0], nodes[1], geq);
			sim.stampCurrentSource(nodes[0], nodes[1], nc);
		}

		public override void calculateCurrent() {
			double voltdiff = volts[0] - volts[1];
			current = pip * Math.Exp(-pvpp / pvt) * (Math.Exp(voltdiff / pvt) - 1)
					+ pip * (voltdiff / pvp) * Math.Exp(1 - voltdiff / pvp) + piv
					* Math.Exp(voltdiff - pvv);
		}

		public override void getInfo(String[] arr) {
			arr[0] = "tunnel diode";
			arr[1] = "I = " + getCurrentText(getCurrent());
			arr[2] = "Vd = " + getVoltageText(getVoltageDiff());
			arr[3] = "P = " + getUnitText(getPower(), "W");
		}
	}
}