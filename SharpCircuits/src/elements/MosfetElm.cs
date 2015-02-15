using System;
using System.Collections;
using System.Collections.Generic;

namespace SharpCircuit {

	public class MosfetElm : CircuitElement {

		public Circuit.Lead leadBase { get { return lead0; } }
		public Circuit.Lead leadSrc { get { return lead1; } }
		public Circuit.Lead leadDrain { get { return new Circuit.Lead(this, 2); } }

		/// <summary>
		/// Threshold Voltage
		/// </summary>
		public double threshold {
			get {
				return (pnp ? -1 : 1) * _threshold;
			}
			set {
				_threshold = (pnp ? -1 : 1) * value;
			}
		}

		private double _threshold;

		private bool pnp;
		private double lastv1;
		private double lastv2;
		private double ids;
		private int mode;
		private double gm;

		public MosfetElm(bool isPNP) : base() {
			pnp = isPNP;
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
			lastv1 = lastv2 = lead_volt[0] = lead_volt[1] = lead_volt[2] = 0;
		}

		public override double getCurrent() {
			return ids;
		}

		public override double getPower() {
			return ids * (lead_volt[2] - lead_volt[1]);
		}

		public override int getLeadCount() {
			return 3;
		}

		public override void stamp(Circuit sim) {
			sim.stampNonLinear(lead_node[1]);
			sim.stampNonLinear(lead_node[2]);
		}

		public override void doStep(Circuit sim) {
			double[] vs = new double[3];
			vs[0] = lead_volt[0];
			vs[1] = lead_volt[1];
			vs[2] = lead_volt[2];
			if(vs[1] > lastv1 + .5) vs[1] = lastv1 + .5;
			if(vs[1] < lastv1 - .5) vs[1] = lastv1 - .5;
			if(vs[2] > lastv2 + .5) vs[2] = lastv2 + .5;
			if(vs[2] < lastv2 - .5) vs[2] = lastv2 - .5;
			int source = 1;
			int drain = 2;
			if((pnp ? -1 : 1) * vs[1] > (pnp ? -1 : 1) * vs[2]) {
				source = 2;
				drain = 1;
			}
			int gate = 0;
			double vgs = vs[gate] - vs[source];
			double vds = vs[drain] - vs[source];
			if(Math.Abs(lastv1 - vs[1]) > .01 || Math.Abs(lastv2 - vs[2]) > .01)
				sim.converged = false;
			lastv1 = vs[1];
			lastv2 = vs[2];
			double realvgs = vgs;
			double realvds = vds;
			vgs *= (pnp ? -1 : 1);
			vds *= (pnp ? -1 : 1);
			ids = 0;
			gm = 0;
			double Gds = 0;
			double beta = getBeta();
			if(vgs > .5 && this is JfetElm) {
				sim.stop("JFET is reverse biased!", this);
				return;
			}
			if(vgs < _threshold) {
				// should be all zero, but that causes a singular matrix,
				// so instead we treat it as a large resistor
				Gds = 1e-8;
				ids = vds * Gds;
				mode = 0;
			} else if(vds < vgs - _threshold) {
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
			double rs = -(pnp ? -1 : 1) * ids + Gds * realvds + gm * realvgs;
			sim.stampMatrix(lead_node[drain], lead_node[drain], Gds);
			sim.stampMatrix(lead_node[drain], lead_node[source], -Gds - gm);
			sim.stampMatrix(lead_node[drain], lead_node[gate], gm);
			sim.stampMatrix(lead_node[source], lead_node[drain], -Gds);
			sim.stampMatrix(lead_node[source], lead_node[source], Gds + gm);
			sim.stampMatrix(lead_node[source], lead_node[gate], -gm);
			sim.stampRightSide(lead_node[drain], rs);
			sim.stampRightSide(lead_node[source], -rs);
			if(source == 2 && (pnp ? -1 : 1) == 1 || source == 1 && (pnp ? -1 : 1) == -1)
				ids = -ids;
		}

		public void getFetInfo(String[] arr, String n) {
			arr[0] = (((pnp ? -1 : 1) == -1) ? "p-" : "n-") + n;
			arr[0] += " (Vt = " + getVoltageText((pnp ? -1 : 1) * _threshold) + ")";
			arr[1] = (((pnp ? -1 : 1) == 1) ? "Ids = " : "Isd = ") + getCurrentText(ids);
			arr[2] = "Vgs = " + getVoltageText(lead_volt[0] - lead_volt[(pnp ? -1 : 1) == -1 ? 2 : 1]);
			arr[3] = (((pnp ? -1 : 1) == 1) ? "Vds = " : "Vsd = ") + getVoltageText(lead_volt[2] - lead_volt[1]);
			arr[4] = (mode == 0) ? "off" : (mode == 1) ? "linear" : "saturation";
			arr[5] = "gm = " + getUnitText(gm, "A/V");
		}

		public override void getInfo(String[] arr) {
			getFetInfo(arr, "MOSFET");
		}

		public override double getVoltageDiff() {
			return lead_volt[2] - lead_volt[1];
		}

		public override bool getConnection(int n1, int n2) {
			return !(n1 == 0 || n2 == 0);
		}
	}
}
