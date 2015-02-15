using System;
using System.Collections;
using System.Collections.Generic;

namespace SharpCircuit {

	public class PhotoResistorElm : CircuitElement {

		// Stub PhotoResistorElm based on SparkGapElm.
		// FIXME need to uncomment PhotoResistorElm line from CirSim.java
		// FIXME need to add PhotoResistorElm.java to srclist

		public Circuit.Lead leadIn { get { return lead0; } }
		public Circuit.Lead leadOut { get { return lead1; } }

		/// <summary>
		/// Min resistance (ohms)
		/// </summary>
		public double minResistance { get; set; }

		/// <summary>
		/// Max resistance (ohms)
		/// </summary>
		public double maxResistance { get; set; }

		private double resistance;

		public PhotoResistorElm() : base() {
			maxResistance = 1e9;
			minResistance = 1e3;
		}

		public override bool nonLinear() {
			return true;
		}

		public override void calculateCurrent() {
			double vd = lead_volt[0] - lead_volt[1];
			current = vd / resistance;
		}

		public override void startIteration(double timeStep) {
			// FIXME set resistance as appropriate, using slider.getValue()
			resistance = minResistance;
			// System.out.print(this + " res current set to " + current + "\n");
		}

		public override void doStep(Circuit sim) {
			sim.stampResistor(lead_node[0], lead_node[1], resistance);
		}

		public override void stamp(Circuit sim) {
			sim.stampNonLinear(lead_node[0]);
			sim.stampNonLinear(lead_node[1]);
		}

		public override void getInfo(String[] arr) {
			// FIXME
			arr[0] = "spark gap";
			getBasicInfo(arr);
			arr[3] = "R = " + getUnitText(resistance, Circuit.ohmString);
			arr[4] = "Ron = " + getUnitText(minResistance, Circuit.ohmString);
			arr[5] = "Roff = " + getUnitText(maxResistance, Circuit.ohmString);
		}

	}
}