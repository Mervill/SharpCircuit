using System;
using System.Collections;
using System.Collections.Generic;

namespace Circuits {

	// Initializers	[X]
	// Properties	[X]
	// Leads		[X]
	// Test Basic	[_]
	// Test Prop	[_]
	public class TriodeElm : CircuitElement {

		public ElementLead leadPlate 	{ get { return leads[0]; }}
		public ElementLead leadGrid 	{ get { return leads[1]; }}
		public ElementLead leadCath 	{ get { return leads[2]; }}

		public double currentp, currentg, currentc;
		
		private double mu, kg1;
		private double gridCurrentR = 6000;
		private double lastv0, lastv1, lastv2;

		public TriodeElm( CirSim s) : base(s) {
			mu = 93;
			kg1 = 680;
		}

		public override bool nonLinear() {
			return true;
		}

		public override void reset() {
			volts[0] = volts[1] = volts[2] = 0;
		}

		public override int getLeadCount() {
			return 3;
		}

		public override double getPower() {
			return (volts[0] - volts[2]) * current;
		}

		public override void doStep() {
			double[] vs = new double[3];
			vs[0] = volts[0];
			vs[1] = volts[1];
			vs[2] = volts[2];
			if (vs[1] > lastv1 + 0.5) {
				vs[1] = lastv1 + 0.5;
			}
			if (vs[1] < lastv1 - 0.5) {
				vs[1] = lastv1 - 0.5;
			}
			if (vs[2] > lastv2 + 0.5) {
				vs[2] = lastv2 + 0.5;
			}
			if (vs[2] < lastv2 - 0.5) {
				vs[2] = lastv2 - 0.5;
			}
			int grid = 1;
			int cath = 2;
			int plate = 0;
			double vgk = vs[grid] - vs[cath];
			double vpk = vs[plate] - vs[cath];
			if (Math.Abs(lastv0 - vs[0]) > 0.01 || Math.Abs(lastv1 - vs[1]) > 0.01 || Math.Abs(lastv2 - vs[2]) > 0.01) {
				sim.converged = false;
			}
			lastv0 = vs[0];
			lastv1 = vs[1];
			lastv2 = vs[2];
			double ids = 0;
			double gm = 0;
			double Gds = 0;
			double ival = vgk + vpk / mu;
			currentg = 0;
			if (vgk > .01) {
				sim.stampResistor(nodes[grid], nodes[cath], gridCurrentR);
				currentg = vgk / gridCurrentR;
			}
			if (ival < 0) {
				// should be all zero, but that causes a singular matrix,
				// so instead we treat it as a large resistor
				Gds = 1E-8;
				ids = vpk * Gds;
			} else {
				ids = Math.Pow(ival, 1.5) / kg1;
				double q = 1.5 * Math.Sqrt(ival) / kg1;
				// gm = dids/dgk;
				// Gds = dids/dpk;
				Gds = q;
				gm = q / mu;
			}
			currentp = ids;
			currentc = ids + currentg;
			double rs = -ids + Gds * vpk + gm * vgk;
			sim.stampMatrix(nodes[plate], nodes[plate], Gds);
			sim.stampMatrix(nodes[plate], nodes[cath], -Gds - gm);
			sim.stampMatrix(nodes[plate], nodes[grid], gm);

			sim.stampMatrix(nodes[cath], nodes[plate], -Gds);
			sim.stampMatrix(nodes[cath], nodes[cath], Gds + gm);
			sim.stampMatrix(nodes[cath], nodes[grid], -gm);

			sim.stampRightSide(nodes[plate], rs);
			sim.stampRightSide(nodes[cath], -rs);
		}

		public override void stamp() {
			sim.stampNonLinear(nodes[0]);
			sim.stampNonLinear(nodes[1]);
			sim.stampNonLinear(nodes[2]);
		}

		public override void getInfo(String[] arr) {
			arr[0] = "triode";
			double vbc = volts[0] - volts[1];
			double vbe = volts[0] - volts[2];
			double vce = volts[1] - volts[2];
			arr[1] = "Vbe = " + getVoltageText(vbe);
			arr[2] = "Vbc = " + getVoltageText(vbc);
			arr[3] = "Vce = " + getVoltageText(vce);
		}

		// grid not connected to other terminals
		public override bool getConnection(int n1, int n2) {
			return !(n1 == 1 || n2 == 1);
		}
	}
}