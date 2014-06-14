using System;
using System.Collections;
using System.Collections.Generic;

namespace Circuits {

	// Initializers	[X]
	// Properties	[X]
	// Leads		[X]
	// Test Basic	[_]
	// Test Prop	[_]
	public class CapacitorElm : CircuitElement {

		/// <summary>
		/// Capacitance (F)
		/// </summary>
		public double capacitance{ 
			get {
				return _capacitance;
			}
			set {
				if(value > 0)
					_capacitance = value;
			}
		}

		[System.ComponentModel.DefaultValue(true)]
		public bool isTrapezoidal { get; set; }

		private double _capacitance = 1E-5;

		private double compResistance;
		private double voltdiff;
		private double curSourceValue;

		public CapacitorElm(CirSim s) : base (s) {

		}

		public override void setLeadVoltage(int n, double c) {
			base.setLeadVoltage(n, c);
			voltdiff = volts[0] - volts[1];
		}

		public override void reset() {
			current = 0;
			// Put small charge on caps when reset to start oscillators
			voltdiff = 1E-3;
		}

		public override void stamp() {
			// Capacitor companion model using trapezoidal approximation
			// (Norton equivalent) consists of a current source in
			// parallel with a resistor. Trapezoidal is more accurate
			// than backward euler but can cause oscillatory behavior
			// if RC is small relative to the timestep.
			if (isTrapezoidal) {
				compResistance = sim.timeStep / (2 * capacitance);
			} else {
				compResistance = sim.timeStep / capacitance;
			}
			sim.stampResistor(nodes[0], nodes[1], compResistance);
			sim.stampRightSide(nodes[0]);
			sim.stampRightSide(nodes[1]);
		}

		public override void startIteration() {
			if (isTrapezoidal) {
				curSourceValue = -voltdiff / compResistance - current;
			} else {
				curSourceValue = -voltdiff / compResistance;
				// System.out.println("cap " + compResistance + " " + curSourceValue + " " + current + " " + voltdiff);
			}
		}

		public override void calculateCurrent() {
			double voltdiff = volts[0] - volts[1];
			// We check compResistance because this might get called
			// before stamp(), which sets compResistance, causing
			// infinite current
			if (compResistance > 0) {
				current = voltdiff / compResistance + curSourceValue;
			}
		}

		public override void doStep() {
			sim.stampCurrentSource(nodes[0], nodes[1], curSourceValue);
		}

		public override void getInfo(String[] arr) {
			arr[0] = "capacitor";
			getBasicInfo(arr);
			arr[3] = "C = " + getUnitText(capacitance, "F");
			arr[4] = "P = " + getUnitText(getPower(), "W");
			// double v = getVoltageDiff();
			// arr[4] = "U = " + getUnitText(.5*capacitance*v*v, "J");
		}

	}
}