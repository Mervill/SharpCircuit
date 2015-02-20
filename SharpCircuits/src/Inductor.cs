#if DEPRECIATED
using System;
using System.Collections;
using System.Collections.Generic;

namespace SharpCircuit {

	public class Inductor {

		public bool isTrapezoidal { get; set; }

		public int[] nodes;

		double current;
		double inductance = 1;
		double compResistance;
		double curSourceValue;

		public Inductor() {
			nodes = new int[2];
		}

		public void setup(double ic, double cr, bool trapezoid) {
			inductance = ic;
			current = cr;
			isTrapezoidal = trapezoid;
		}

		public void reset() {
			current = 0;
		}

		public void stamp(Circuit sim, double deltaTime, int n0, int n1) {
			// inductor companion model using trapezoidal or backward euler
			// approximations (Norton equivalent) consists of a current
			// source in parallel with a resistor. Trapezoidal is more
			// accurate than backward euler but can cause oscillatory behavior.
			// The oscillation is a real problem in circuits with switches.
			nodes[0] = n0;
			nodes[1] = n1;
			if(isTrapezoidal) {
				compResistance = 2 * inductance / deltaTime;
			} else {
				compResistance = inductance / deltaTime; // backward euler
			}
			sim.stampResistor(nodes[0], nodes[1], compResistance);
			sim.stampRightSide(nodes[0]);
			sim.stampRightSide(nodes[1]);
		}

		public void startIteration(double voltdiff) {
			if(isTrapezoidal) {
				curSourceValue = voltdiff / compResistance + current;
			} else {
				curSourceValue = current; // backward euler
			}
		}

		public double calculateCurrent(double voltdiff) {
			// we check compResistance because this might get called
			// before stamp(CirSim sim), which sets compResistance, causing
			// infinite current
			if(compResistance > 0)
				current = voltdiff / compResistance + curSourceValue;
			return current;
		}

		public void doStep(Circuit sim, double voltdiff) {
			sim.stampCurrentSource(nodes[0], nodes[1], curSourceValue);
		}
	}
}
#endif