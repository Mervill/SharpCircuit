using System;
using System.Collections;
using System.Collections.Generic;

namespace SharpCircuit {

	// Stub implementation of DiacElm, based on SparkGapElm
	// FIXME need to add DiacElm.java to srclist
	// FIXME need to uncomment DiacElm line from CirSim.java

	public class DiacElm : CircuitElement {

		public Circuit.Lead leadIn { get { return lead0; } }
		public Circuit.Lead leadOut { get { return lead1; } }

		/// <summary>
		/// On resistance (ohms)
		/// </summary>
		public double onResistance { get; set; }

		/// <summary>
		/// Off resistance (ohms)
		/// </summary>
		public double offResistance { get; set; }

		/// <summary>
		/// Breakdown voltage (volts)
		/// </summary>
		public double breakdown { get; set; }

		/// <summary>
		/// Hold current (amps)
		/// </summary>
		public double holdCurrent { get; set; }

		private bool state;

		public DiacElm() : base() {
			// FIXME need to adjust defaults to make sense for diac
			offResistance = 1E9;
			onResistance = 1E3;
			breakdown = 1E3;
			holdCurrent = 0.001;
			state = false;
		}

		public override bool nonLinear() {
			return true;
		}

		public override void calculateCurrent() {
			double vd = lead_volt[0] - lead_volt[1];
			if(state) {
				current = vd / onResistance;
			} else {
				current = vd / offResistance;
			}
		}

		public override void startIteration(double timeStep) {
			double vd = lead_volt[0] - lead_volt[1];
			if(Math.Abs(current) < holdCurrent) {
				state = false;
			}
			if(Math.Abs(vd) > breakdown) {
				state = true;
				// System.out.print(this + " res current set to " + current + "\n");
			}
		}

		public override void doStep(Circuit sim) {
			if(state) {
				sim.stampResistor(lead_node[0], lead_node[1], onResistance);
			} else {
				sim.stampResistor(lead_node[0], lead_node[1], offResistance);
			}
		}

		public override void stamp(Circuit sim) {
			sim.stampNonLinear(lead_node[0]);
			sim.stampNonLinear(lead_node[1]);
		}

		public override void getInfo(String[] arr) {
			// FIXME
			arr[0] = "spark gap";
			getBasicInfo(arr);
			arr[3] = state ? "on" : "off";
			arr[4] = "Ron = " + getUnitText(onResistance, Circuit.ohmString);
			arr[5] = "Roff = " + getUnitText(offResistance, Circuit.ohmString);
			arr[6] = "Vbrkdn = " + getUnitText(breakdown, "V");
			arr[7] = "Ihold = " + getUnitText(holdCurrent, "A");
		}

	}
}