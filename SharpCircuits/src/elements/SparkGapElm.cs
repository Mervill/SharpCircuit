using System;
using System.Collections;
using System.Collections.Generic;

namespace SharpCircuit {

	public class SparkGapElm : CircuitElement {

		//public ElementLead leadIn 	{ get { return lead0; }}
		//public ElementLead leadOut 	{ get { return lead1; }}

		/// <summary>
		/// On resistance (ohms)
		/// </summary>
		public double onresistance { get; set; }

		/// <summary>
		/// Off resistance (ohms)
		/// </summary>
		public double offresistance { get; set; }

		/// <summary>
		/// Breakdown voltage
		/// </summary>
		public double breakdown { get; set; }

		/// <summary>
		/// Holding current (A)
		/// </summary>
		public double holdcurrent { get; set; }

		private double resistance;
		private bool state;

		public SparkGapElm() {
			offresistance = 1E9;
			onresistance = 1E3;
			breakdown = 1E3;
			holdcurrent = 0.001;
			state = false;
		}

		public override bool nonLinear() {
			return true;
		}

		public override void calculateCurrent() {
			double vd = volts[0] - volts[1];
			current = vd / resistance;
		}

		public override void reset() {
			base.reset();
			state = false;
		}

		public override void startIteration(double timeStep) {
			if (Math.Abs(current) < holdcurrent) {
				state = false;
			}
			double vd = volts[0] - volts[1];
			if (Math.Abs(vd) > breakdown) {
				state = true;
			}
		}

		public override void doStep(CirSim sim) {
			resistance = (state) ? onresistance : offresistance;
			sim.stampResistor(nodes[0], nodes[1], resistance);
		}

		public override void stamp(CirSim sim) {
			sim.stampNonLinear(nodes[0]);
			sim.stampNonLinear(nodes[1]);
		}

		public override void getInfo(String[] arr) {
			arr[0] = "spark gap";
			getBasicInfo(arr);
			arr[3] = state ? "on" : "off";
			arr[4] = "Ron = " + getUnitText(onresistance, CirSim.ohmString);
			arr[5] = "Roff = " + getUnitText(offresistance, CirSim.ohmString);
			arr[6] = "Vbreakdown = " + getUnitText(breakdown, "V");
		}

	}
}