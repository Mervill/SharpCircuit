using System;
using System.Collections;
using System.Collections.Generic;

namespace SharpCircuit {

	public class TransistorElm : CircuitElement {

		public Circuit.Lead leadBase { get { return lead0; } }
		public Circuit.Lead leadCollector { get { return lead1; } }
		public Circuit.Lead leadEmitter { get { return new Circuit.Lead(this, 2); } }

		/// <summary>
		/// Beta/hFE
		/// </summary>
		public double Beta {
			get {
				return beta;
			}
			set {
				beta = value;
				setup();
			}
		}

		private double beta;

		private static readonly double leakage = 1E-13; // 1e-6;
		private static readonly double vt = 0.025;
		private static readonly double vdcoef = 1 / vt;
		private static readonly double rgain = 0.5;

		private double pnp;

		private double fgain;
		private double gmin;

		private double ic, ie, ib;
		private double vcrit;
		private double lastvbc, lastvbe;

		public TransistorElm() : base() {
			pnp = 1;
			beta = 100;
			setup();
		}

		public TransistorElm(bool pnpflag) : base() {
			pnp = (pnpflag) ? -1 : 1;
			beta = 100;
			setup();
		}

		public void setup() {
			vcrit = vt * Math.Log(vt / (Math.Sqrt(2) * leakage));
			fgain = beta / (beta + 1);
		}

		public override bool nonLinear() {
			return true;
		}

		public override void reset() {
			lead_volt[0] = lead_volt[1] = lead_volt[2] = 0;
			lastvbc = lastvbe = 0;
		}

		public override int getLeadCount() {
			return 3;
		}

		public override double getPower() {
			return (lead_volt[0] - lead_volt[2]) * ib + (lead_volt[1] - lead_volt[2]) * ic;
		}

		public double limitStep(Circuit sim, double vnew, double vold) {
			double arg;
			if(vnew > vcrit && Math.Abs(vnew - vold) > (vt + vt)) {
				if(vold > 0) {
					arg = 1 + (vnew - vold) / vt;
					if(arg > 0) {
						vnew = vold + vt * Math.Log(arg);
					} else {
						vnew = vcrit;
					}
				} else {
					vnew = vt * Math.Log(vnew / vt);
				}
				sim.converged = false;
				// System.out.println(vnew + " " + oo + " " + vold);
			}
			return (vnew);
		}

		public override void stamp(Circuit sim) {
			sim.stampNonLinear(lead_node[0]);
			sim.stampNonLinear(lead_node[1]);
			sim.stampNonLinear(lead_node[2]);
		}

		public override void step(Circuit sim) {
			double vbc = lead_volt[0] - lead_volt[1]; // typically negative
			double vbe = lead_volt[0] - lead_volt[2]; // typically positive
			if(Math.Abs(vbc - lastvbc) > 0.01 || // .01
					Math.Abs(vbe - lastvbe) > 0.01) {
				sim.converged = false;
			}
			gmin = 0;
			if(sim.subIterations > 100) {
				// if we have trouble converging, put a conductance in parallel with
				// all P-N junctions. Gradually increase the conductance value for each iteration.
				gmin = Math.Exp(-9 * Math.Log(10) * (1 - sim.subIterations / 3000.0));
				if(gmin > .1) {
					gmin = .1;
				}
			}
			// System.out.print("T " + vbc + " " + vbe + "\n");
			vbc = pnp * limitStep(sim, pnp * vbc, pnp * lastvbc);
			vbe = pnp * limitStep(sim, pnp * vbe, pnp * lastvbe);
			lastvbc = vbc;
			lastvbe = vbe;
			double pcoef = vdcoef * pnp;
			double expbc = Math.Exp(vbc * pcoef);
			/*
			 * if (expbc > 1e13 || Double.isInfinite(expbc)) expbc = 1e13;
			 */
			double expbe = Math.Exp(vbe * pcoef);
			if(expbe < 1) {
				expbe = 1;
			}
			/*
			 * if (expbe > 1e13 || Double.isInfinite(expbe)) expbe = 1e13;
			 */
			ie = pnp * leakage * (-(expbe - 1) + rgain * (expbc - 1));
			ic = pnp * leakage * (fgain * (expbe - 1) - (expbc - 1));
			ib = -(ie + ic);
			// System.out.println("gain " + ic/ib);
			// System.out.print("T " + vbc + " " + vbe + " " + ie + " " + ic +
			// "\n");
			double gee = -leakage * vdcoef * expbe;
			double gec = rgain * leakage * vdcoef * expbc;
			double gce = -gee * fgain;
			double gcc = -gec * (1 / rgain);

			/*
			 * System.out.print("gee = " + gee + "\n"); System.out.print("gec = " +
			 * gec + "\n"); System.out.print("gce = " + gce + "\n");
			 * System.out.print("gcc = " + gcc + "\n");
			 * System.out.print("gce+gcc = " + (gce+gcc) + "\n");
			 * System.out.print("gee+gec = " + (gee+gec) + "\n");
			 */

			// stamps from page 302 of Pillage. Node 0 is the base,
			// node 1 the collector, node 2 the emitter. Also stamp
			// minimum conductance (gmin) between b,e and b,c
			sim.stampMatrix(lead_node[0], lead_node[0], -gee - gec - gce - gcc + gmin * 2);
			sim.stampMatrix(lead_node[0], lead_node[1], gec + gcc - gmin);
			sim.stampMatrix(lead_node[0], lead_node[2], gee + gce - gmin);
			sim.stampMatrix(lead_node[1], lead_node[0], gce + gcc - gmin);
			sim.stampMatrix(lead_node[1], lead_node[1], -gcc + gmin);
			sim.stampMatrix(lead_node[1], lead_node[2], -gce);
			sim.stampMatrix(lead_node[2], lead_node[0], gee + gec - gmin);
			sim.stampMatrix(lead_node[2], lead_node[1], -gec);
			sim.stampMatrix(lead_node[2], lead_node[2], -gee + gmin);

			// we are solving for v(k+1), not delta v, so we use formula
			// 10.5.13, multiplying J by v(k)
			sim.stampRightSide(lead_node[0], -ib - (gec + gcc) * vbc - (gee + gce) * vbe);
			sim.stampRightSide(lead_node[1], -ic + gce * vbe + gcc * vbc);
			sim.stampRightSide(lead_node[2], -ie + gee * vbe + gec * vbc);
		}

		public override void getInfo(String[] arr) {
			arr[0] = "transistor (" + ((pnp == -1) ? "PNP)" : "NPN)") + " beta=" + beta;
			double vbc = lead_volt[0] - lead_volt[1];
			double vbe = lead_volt[0] - lead_volt[2];
			double vce = lead_volt[1] - lead_volt[2];
			if(vbc * pnp > .2) {
				arr[1] = vbe * pnp > .2 ? "saturation" : "reverse active";
			} else {
				arr[1] = vbe * pnp > .2 ? "fwd active" : "cutoff";
			}
			arr[2] = "Ic = " + getCurrentText(ic);
			arr[3] = "Ib = " + getCurrentText(ib);
			arr[4] = "Vbe = " + getVoltageText(vbe);
			arr[5] = "Vbc = " + getVoltageText(vbc);
			arr[6] = "Vce = " + getVoltageText(vce);
		}

		/*public override double getScopeValue(int x) {
			switch (x) {
			case 1:
				return ib;
			case 2:
				return ic;
			case 3:
				return ie;
			case 4:
				return volts[0] - volts[2];
			case 5:
				return volts[0] - volts[1];
			case 6:
				return volts[1] - volts[2];
			}
			return 0;
		}*/

		/*public override String getScopeUnits(int x) {
			switch (x) {
			case 1:
			case 2:
			case 3:
				return "A";
			default:
				return "V";
			}
		}*/

		/*public override bool canViewInScope() {
			return true;
		}*/
	}
}