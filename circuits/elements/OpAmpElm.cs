using System;
using System.Collections;
using System.Collections.Generic;

namespace Circuits {

	// Initializers	[X]
	// Properties	[X]
	// Leads		[X]
	// Test Basic	[_]
	// Test Prop	[_]
	public class OpAmpElm : CircuitElement {

		public ElementLead leadOut { get { return leads[0]; }}
		public ElementLead leadIn1 { get { return leads[1]; }}
		public ElementLead leadIn2 { get { return leads[2]; }}

		/// <summary>
		/// Max Output (V)
		/// </summary>
		public double maxOut{ get; set; }

		/// <summary>
		/// Min Output (V)
		/// </summary>
		public double minOut{ get; set; }

		private double lastvd;
		private double gain;

		public OpAmpElm(CirSim s) : base(s) {
			maxOut = 15;
			minOut = -15;
			gain = 100000;
		}

		public OpAmpElm(CirSim s,bool lowGain) : this(s) {
			gain = (lowGain) ? 1000 : 100000;
		}

		public override bool nonLinear() {
			return true;
		}

		public override double getPower() {
			return volts[2] * current;
		}

		public override int getLeadCount() {
			return 3;
		}

		public override ElementLead getLead(int n) {
			return (n == 0) ? leadIn1 : (n == 1) ? leadIn2 : lead1;
		}

		public override int getVoltageSourceCount() {
			return 1;
		}

		public override void getInfo(String[] arr) {
			arr[0] = "op-amp";
			arr[1] = "V+ = " + getVoltageText(volts[1]);
			arr[2] = "V- = " + getVoltageText(volts[0]);
			// sometimes the voltage goes slightly outside range, to make
			// convergence easier. so we hide that here.
			double vo = Math.Max(Math.Min(volts[2], maxOut), minOut);
			arr[3] = "Vout = " + getVoltageText(vo);
			arr[4] = "Iout = " + getCurrentText(getCurrent());
			arr[5] = "range = " + getVoltageText(minOut) + " to "
					+ getVoltageText(maxOut);
		}

		public override void stamp() {
			int vn = sim.nodeList.Count + voltSource;
			sim.stampNonLinear(vn);
			sim.stampMatrix(nodes[2], vn, 1);
		}

		public override void doStep() {
			double vd = volts[1] - volts[0];
			if (Math.Abs(lastvd - vd) > .1) {
				sim.converged = false;
			} else if (volts[2] > maxOut + .1 || volts[2] < minOut - .1) {
				sim.converged = false;
			}
			double x = 0;
			int vn = sim.nodeList.Count + voltSource;
			double dx = 0;
			if (vd >= maxOut / gain && (lastvd >= 0 || sim.getrand(4) == 1)) {
				dx = 1e-4;
				x = maxOut - dx * maxOut / gain;
			} else if (vd <= minOut / gain && (lastvd <= 0 || sim.getrand(4) == 1)) {
				dx = 1e-4;
				x = minOut - dx * minOut / gain;
			} else {
				dx = gain;
				// System.out.println("opamp " + vd + " " + volts[2] + " " + dx +
				// " " +
				// x + " " + lastvd + " " + sim.converged);
			}

			// newton-raphson
			sim.stampMatrix(vn, nodes[0], dx);
			sim.stampMatrix(vn, nodes[1], -dx);
			sim.stampMatrix(vn, nodes[2], 1);
			sim.stampRightSide(vn, x);

			lastvd = vd;
			/*
			 * if (sim.converged) System.out.println((volts[1]-volts[0]) + " " +
			 * volts[2] + " " + initvd);
			 */
		}

		// there is no current path through the op-amp inputs, but there
		// is an indirect path through the output to ground.
		public override bool getConnection(int n1, int n2) {
			return false;
		}

		public override bool hasGroundConnection(int n1) {
			return (n1 == 2);
		}

		public override double getVoltageDiff() {
			return volts[2] - volts[1];
		}

	}
}