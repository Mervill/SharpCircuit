using System;
using System.Collections;
using System.Collections.Generic;

namespace Circuits {

	// Initializers	[X]
	// Properties	[X]
	// Leads		[X]
	// Test Basic	[_]
	// Test Prop	[_]
	public class PhotoResistorElm : CircuitElement {

		// Stub PhotoResistorElm based on SparkGapElm.
		// FIXME need to uncomment PhotoResistorElm line from CirSim.java
		// FIXME need to add PhotoResistorElm.java to srclist

		/// <summary>
		/// Min resistance (ohms)
		/// </summary>
		public double minresistance{ get; set; }

		/// <summary>
		/// Max resistance (ohms)
		/// </summary>
		public double maxresistance{ get; set; }

		private double resistance;

		public PhotoResistorElm( CirSim s) : base(s) {
			maxresistance = 1e9;
			minresistance = 1e3;
		}

		public override bool nonLinear() {
			return true;
		}

		public override void calculateCurrent() {
			double vd = volts[0] - volts[1];
			current = vd / resistance;
		}

		public override void startIteration() {
			// FIXME set resistance as appropriate, using slider.getValue()
			resistance = minresistance;
			// System.out.print(this + " res current set to " + current + "\n");
		}

		public override void doStep() {
			sim.stampResistor(nodes[0], nodes[1], resistance);
		}

		public override void stamp() {
			sim.stampNonLinear(nodes[0]);
			sim.stampNonLinear(nodes[1]);
		}

		public override void getInfo(String[] arr) {
			// FIXME
			arr[0] = "spark gap";
			getBasicInfo(arr);
			arr[3] = "R = " + getUnitText(resistance, CirSim.ohmString);
			arr[4] = "Ron = " + getUnitText(minresistance, CirSim.ohmString);
			arr[5] = "Roff = " + getUnitText(maxresistance, CirSim.ohmString);
		}

	}
}