using System;
using System.Collections;
using System.Collections.Generic;

namespace Circuits {

	public class TransistorElm : CircuitElement {

		private static readonly double leakage = 1E-13; // 1e-6;
		private static readonly double vt = 0.025;
		private static readonly double vdcoef = 1 / vt;
		private static readonly double rgain = 0.5;

		private int pnp;

		/// <summary>
		/// Beta/hFE
		/// </summary>
		public double Beta {
			get{
				return beta;
			}
			set{
				beta = value;
				setup();
			}
		}
		private double beta;

		private double fgain;
		private double gmin;

		private double ic, ie, ib;
		private double vcrit;
		private double lastvbc, lastvbe;

		public int FLAG_FLIP = 1;

		public ElementLead coll;
		public ElementLead emit;

		public TransistorElm(CirSim s,bool pnpflag) : base(s) {
			coll = new ElementLead(this,1);
			emit = new ElementLead(this,2);

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
			volts[0] = volts[1] = volts[2] = 0;
			lastvbc = lastvbe = 0;
		}

		public override ElementLead getLead(int n) {
			return (n == 0) ? lead0 : (n == 1) ? coll : emit;
		}

		public override int getLeadCount() {
			return 3;
		}

		public override double getPower() {
			return (volts[0] - volts[2]) * ib + (volts[1] - volts[2]) * ic;
		}

		public double limitStep(double vnew, double vold) {
			double arg;
			if (vnew > vcrit && Math.Abs(vnew - vold) > (vt + vt)) {
				if (vold > 0) {
					arg = 1 + (vnew - vold) / vt;
					if (arg > 0) {
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

		public override void stamp() {
			sim.stampNonLinear(nodes[0]);
			sim.stampNonLinear(nodes[1]);
			sim.stampNonLinear(nodes[2]);
		}

		public override void doStep() {
			double vbc = volts[0] - volts[1]; // typically negative
			double vbe = volts[0] - volts[2]; // typically positive
			if (Math.Abs(vbc - lastvbc) > 0.01 || // .01
					Math.Abs(vbe - lastvbe) > 0.01) {
				sim.converged = false;
			}
			gmin = 0;
			if (sim.subIterations > 100) {
				// if we have trouble converging, put a conductance in parallel with
				// all P-N junctions.
				// Gradually increase the conductance value for each iteration.
				gmin = Math.Exp(-9 * Math.Log(10) * (1 - sim.subIterations / 3000.0));
				if (gmin > .1) {
					gmin = .1;
				}
			}
			// System.out.print("T " + vbc + " " + vbe + "\n");
			vbc = pnp * limitStep(pnp * vbc, pnp * lastvbc);
			vbe = pnp * limitStep(pnp * vbe, pnp * lastvbe);
			lastvbc = vbc;
			lastvbe = vbe;
			double pcoef = vdcoef * pnp;
			double expbc = Math.Exp(vbc * pcoef);
			/*
			 * if (expbc > 1e13 || Double.isInfinite(expbc)) expbc = 1e13;
			 */
			double expbe = Math.Exp(vbe * pcoef);
			if (expbe < 1) {
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
			sim.stampMatrix(nodes[0], nodes[0], -gee - gec - gce - gcc + gmin * 2);
			sim.stampMatrix(nodes[0], nodes[1], gec + gcc - gmin);
			sim.stampMatrix(nodes[0], nodes[2], gee + gce - gmin);
			sim.stampMatrix(nodes[1], nodes[0], gce + gcc - gmin);
			sim.stampMatrix(nodes[1], nodes[1], -gcc + gmin);
			sim.stampMatrix(nodes[1], nodes[2], -gce);
			sim.stampMatrix(nodes[2], nodes[0], gee + gec - gmin);
			sim.stampMatrix(nodes[2], nodes[1], -gec);
			sim.stampMatrix(nodes[2], nodes[2], -gee + gmin);

			// we are solving for v(k+1), not delta v, so we use formula
			// 10.5.13, multiplying J by v(k)
			sim.stampRightSide(nodes[0], -ib - (gec + gcc) * vbc - (gee + gce)
					* vbe);
			sim.stampRightSide(nodes[1], -ic + gce * vbe + gcc * vbc);
			sim.stampRightSide(nodes[2], -ie + gee * vbe + gec * vbc);
		}

		public override void getInfo(String[] arr) {
			arr[0] = "transistor (" + ((pnp == -1) ? "PNP)" : "NPN)") + " beta=" + beta;
			double vbc = volts[0] - volts[1];
			double vbe = volts[0] - volts[2];
			double vce = volts[1] - volts[2];
			if (vbc * pnp > .2) {
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

		public override double getScopeValue(int x) {
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
		}

		public override String getScopeUnits(int x) {
			switch (x) {
			case 1:
			case 2:
			case 3:
				return "A";
			default:
				return "V";
			}
		}

		/*public override bool canViewInScope() {
			return true;
		}*/
	}
}