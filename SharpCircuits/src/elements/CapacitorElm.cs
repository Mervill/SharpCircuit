using System;
using System.Collections;
using System.Collections.Generic;

namespace SharpCircuit {

	public class CapacitorElm : CircuitElement {

		public Circuit.Lead leadIn { get { return lead0; } }
		public Circuit.Lead leadOut { get { return lead1; } }

		/// <summary>
		/// Capacitance (F)
		/// </summary>
		public double capacitance {
			get {
				return _capacitance;
			}
			set {
				if(value > 0)
					_capacitance = value;
			}
		}

		public bool isTrapezoidal { get; set; }

		private double _capacitance = 1E-5;

		private double compResistance;
		private double voltdiff;
		private double curSourceValue;

		public CapacitorElm() : base() {
			isTrapezoidal = true;
		}

		public CapacitorElm(double c) : base() {
			capacitance = c;
			isTrapezoidal = true;
		}

		public override void setLeadVoltage(int leadX, double vValue) {
			base.setLeadVoltage(leadX, vValue);
			voltdiff = lead_volt[0] - lead_volt[1];
		}

		public override void reset() {
			current = 0;
			// Put small charge on caps when reset to start oscillators
			voltdiff = 1E-3;
		}

		public override void stamp(Circuit sim) {
			// Capacitor companion model using trapezoidal approximation
			// (Norton equivalent) consists of a current source in
			// parallel with a resistor. Trapezoidal is more accurate
			// than backward euler but can cause oscillatory behavior
			// if RC is small relative to the timestep.
			if(isTrapezoidal) {
				compResistance = sim.timeStep / (2 * capacitance);
			} else {
				compResistance = sim.timeStep / capacitance;
			}
			sim.stampResistor(lead_node[0], lead_node[1], compResistance);
			sim.stampRightSide(lead_node[0]);
			sim.stampRightSide(lead_node[1]);
		}

		public override void beginStep(Circuit sim) {
			if(isTrapezoidal) {
				curSourceValue = -voltdiff / compResistance - current;
			} else {
				curSourceValue = -voltdiff / compResistance;
				// System.out.println("cap " + compResistance + " " + curSourceValue + " " + current + " " + voltdiff);
			}
		}

		public override void calculateCurrent() {
			double voltdiff = lead_volt[0] - lead_volt[1];
			// We check compResistance because this might get called
			// before stamp(CirSim sim), which sets compResistance,
			// causing infinite current
			if(compResistance > 0)
				current = voltdiff / compResistance + curSourceValue;
		}

		public override void step(Circuit sim) {
			sim.stampCurrentSource(lead_node[0], lead_node[1], curSourceValue);
		}

		/*public override void getInfo(String[] arr) {
			arr[0] = "capacitor";
			getBasicInfo(arr);
			arr[3] = "C = " + getUnitText(capacitance, "F");
			arr[4] = "P = " + getUnitText(getPower(), "W");
			// double v = getVoltageDiff();
			// arr[4] = "U = " + getUnitText(.5*capacitance*v*v, "J");
		}*/

		/*Get the capacitance. For the special case of a parallel plate capacitor this is given by the 
		formula C=epsilon0*A/d Farads, where epsilon0 is the permittivity of free space or a vacuum with 
		a value of 8.854e-12, A is the area across the face of the conductors and d is the distance 
		between the plates.*/
	}
}