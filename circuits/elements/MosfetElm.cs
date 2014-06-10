using System;
using System.Collections;
using System.Collections.Generic;

namespace Circuits {

	// Initializers	[X]
	// Properties	[X]
	// Leads		[_]
	// Test Basic	[_]
	// Test Prop	[_]
	public class MosfetElm : CircuitElement {

		public static readonly int FLAG_PNP = 1;

		private int pnp;
		private double lastv1;
		private double lastv2;
		private double ids;
		private int mode;
		private double gm;

		/// <summary>
		/// Threshold Voltage
		/// </summary>
		public double threshold{ 
			get {
				return pnp * _threshold;
			} 
			set {
				_threshold = pnp * value;
			} 
		}
		private double _threshold;

		public ElementLead src;
		public ElementLead drn;

		public MosfetElm(CirSim s,bool pnpflag) : base(s) {
			src = new ElementLead(this,1);
			drn = new ElementLead(this,2);
			pnp = (pnpflag) ? -1 : 1;
			flags = (pnpflag) ? FLAG_PNP : 0;
			_threshold = getDefaultThreshold();
		}

		public virtual double getDefaultThreshold() {
			return 1.5;
		}

		public virtual double getBeta() {
			return 0.02;
		}

		public override bool nonLinear() {
			return true;
		}

		public override void reset() {
			lastv1 = lastv2 = volts[0] = volts[1] = volts[2] = 0;
		}
		
		public override ElementLead getLead(int n) {
			return (n == 0) ? lead0 : (n == 1) ? src : drn;
		}

		public override double getCurrent() {
			return ids;
		}

		public override double getPower() {
			return ids * (volts[2] - volts[1]);
		}

		public override int getLeadCount() {
			return 3;
		}

		public override void stamp() {
			sim.stampNonLinear(nodes[1]);
			sim.stampNonLinear(nodes[2]);
		}

		public override void doStep() {
			double[] vs = new double[3];
			vs[0] = volts[0];
			vs[1] = volts[1];
			vs[2] = volts[2];
			if (vs[1] > lastv1 + .5) {
				vs[1] = lastv1 + .5;
			}
			if (vs[1] < lastv1 - .5) {
				vs[1] = lastv1 - .5;
			}
			if (vs[2] > lastv2 + .5) {
				vs[2] = lastv2 + .5;
			}
			if (vs[2] < lastv2 - .5) {
				vs[2] = lastv2 - .5;
			}
			int source = 1;
			int drain = 2;
			if (pnp * vs[1] > pnp * vs[2]) {
				source = 2;
				drain = 1;
			}
			int gate = 0;
			double vgs = vs[gate] - vs[source];
			double vds = vs[drain] - vs[source];
			if (Math.Abs(lastv1 - vs[1]) > .01 || Math.Abs(lastv2 - vs[2]) > .01) {
				sim.converged = false;
			}
			lastv1 = vs[1];
			lastv2 = vs[2];
			double realvgs = vgs;
			double realvds = vds;
			vgs *= pnp;
			vds *= pnp;
			ids = 0;
			gm = 0;
			double Gds = 0;
			double beta = getBeta();
			if (vgs > .5 && this is JfetElm) {
				sim.stop("JFET is reverse biased!", this);
				return;
			}
			if (vgs < _threshold) {
				// should be all zero, but that causes a singular matrix,
				// so instead we treat it as a large resistor
				Gds = 1e-8;
				ids = vds * Gds;
				mode = 0;
			} else if (vds < vgs - _threshold) {
				// linear
				ids = beta * ((vgs - _threshold) * vds - vds * vds * .5);
				gm = beta * vds;
				Gds = beta * (vgs - vds - _threshold);
				mode = 1;
			} else {
				// saturation; Gds = 0
				gm = beta * (vgs - _threshold);
				// use very small Gds to avoid nonconvergence
				Gds = 1e-8;
				ids = 0.5 * beta * (vgs - _threshold) * (vgs - _threshold) + (vds - (vgs - _threshold)) * Gds;
				mode = 2;
			}
			double rs = -pnp * ids + Gds * realvds + gm * realvgs;
			// System.out.println("M " + vds + " " + vgs + " " + ids + " " + gm +
			// " "+ Gds + " " + volts[0] + " " + volts[1] + " " + volts[2] + " " +
			// source + " " + rs + " " + this);
			sim.stampMatrix(nodes[drain], nodes[drain], Gds);
			sim.stampMatrix(nodes[drain], nodes[source], -Gds - gm);
			sim.stampMatrix(nodes[drain], nodes[gate], gm);

			sim.stampMatrix(nodes[source], nodes[drain], -Gds);
			sim.stampMatrix(nodes[source], nodes[source], Gds + gm);
			sim.stampMatrix(nodes[source], nodes[gate], -gm);

			sim.stampRightSide(nodes[drain], rs);
			sim.stampRightSide(nodes[source], -rs);
			if (source == 2 && pnp == 1 || source == 1 && pnp == -1) {
				ids = -ids;
			}
		}

		public void getFetInfo(String[] arr, String n) {
			arr[0] = ((pnp == -1) ? "p-" : "n-") + n;
			arr[0] += " (Vt = " + getVoltageText(pnp * _threshold) + ")";
			arr[1] = ((pnp == 1) ? "Ids = " : "Isd = ") + getCurrentText(ids);
			arr[2] = "Vgs = " + getVoltageText(volts[0] - volts[pnp == -1 ? 2 : 1]);
			arr[3] = ((pnp == 1) ? "Vds = " : "Vsd = ") + getVoltageText(volts[2] - volts[1]);
			arr[4] = (mode == 0) ? "off" : (mode == 1) ? "linear" : "saturation";
			arr[5] = "gm = " + getUnitText(gm, "A/V");
		}

		public override void getInfo(String[] arr) {
			getFetInfo(arr, "MOSFET");
		}

		/*public override bool canViewInScope() {
			return true;
		}*/

		public override double getVoltageDiff() {
			return volts[2] - volts[1];
		}

		public override bool getConnection(int n1, int n2) {
			return !(n1 == 0 || n2 == 0);
		}
	}
}
