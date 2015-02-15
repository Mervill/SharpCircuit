using System;
using System.Collections;
using System.Collections.Generic;

namespace SharpCircuit {

	public class OpAmpElm : CircuitElement {

		public Circuit.Lead leadPos { get { return lead0; } }
		public Circuit.Lead leadNeg { get { return lead1; } }
		public Circuit.Lead leadOut { get { return new Circuit.Lead(this, 2); } }

		/// <summary>
		/// Max Output (V)
		/// </summary>
		public double maxOut { get; set; }

		/// <summary>
		/// Min Output (V)
		/// </summary>
		public double minOut { get; set; }

		private double lastvd;
		private double gain;

		public OpAmpElm() : base() {
			maxOut = 15;
			minOut = -15;
			gain = 100000;
		}

		public OpAmpElm(bool lowGain) : base() {
			gain = (lowGain) ? 1000 : 100000;
		}

		public override bool nonLinear() {
			return true;
		}

		public override double getPower() {
			return lead_volt[2] * current;
		}

		public override int getLeadCount() {
			return 3;
		}

		public override int getVoltageSourceCount() {
			return 1;
		}

		public override void getInfo(String[] arr) {
			arr[0] = "op-amp";
			arr[1] = "V+ = " + getVoltageText(lead_volt[1]);
			arr[2] = "V- = " + getVoltageText(lead_volt[0]);
			// sometimes the voltage goes slightly outside range, to make
			// convergence easier. so we hide that here.
			double vo = Math.Max(Math.Min(lead_volt[2], maxOut), minOut);
			arr[3] = "Vout = " + getVoltageText(vo);
			arr[4] = "Iout = " + getCurrentText(current);
			arr[5] = "range = " + getVoltageText(minOut) + " to " + getVoltageText(maxOut);
		}

		public override void stamp(Circuit sim) {
			int vn = sim.nodeCount + voltSource;
			sim.stampNonLinear(vn);
			sim.stampMatrix(lead_node[2], vn, 1);
		}

		public override void doStep(Circuit sim) {
			double vd = lead_volt[1] - lead_volt[0];
			if(Math.Abs(lastvd - vd) > .1) {
				sim.converged = false;
			} else if(lead_volt[2] > maxOut + .1 || lead_volt[2] < minOut - .1) {
				sim.converged = false;
			}
			double x = 0;
			int vn = sim.nodeCount + voltSource;
			double dx = 0;
			if(vd >= maxOut / gain && (lastvd >= 0 || sim.getRand(4) == 1)) {
				dx = 1e-4;
				x = maxOut - dx * maxOut / gain;
			} else if(vd <= minOut / gain && (lastvd <= 0 || sim.getRand(4) == 1)) {
				dx = 1e-4;
				x = minOut - dx * minOut / gain;
			} else {
				dx = gain;
			}

			// newton-raphson
			sim.stampMatrix(vn, lead_node[0], dx);
			sim.stampMatrix(vn, lead_node[1], -dx);
			sim.stampMatrix(vn, lead_node[2], 1);
			sim.stampRightSide(vn, x);

			lastvd = vd;
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
			return lead_volt[2] - lead_volt[1];
		}

	}
}