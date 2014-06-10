using System;
using System.Collections;
using System.Collections.Generic;

namespace Circuits {

	// Initializers	[X]
	// Properties	[X]
	// Leads		[X]
	// Test Basic	[_]
	// Test Prop	[_]
	public class ThermistorElm : CircuitElement {

		// stub ThermistorElm based on SparkGapElm
		// FIXME need to uncomment ThermistorElm line from CirSim.java
		// FIXME need to add ThermistorElm.java to srclist

		/// <summary>
		/// Min resistance (ohms)
		/// </summary>
		public double minresistance{ get; set; }

		/// <summary>
		/// Max resistance (ohms)
		/// </summary>
		public double maxresistance{ get; set; }

		public double slider{ get; set; }

		private double resistance;

		public ThermistorElm( CirSim s) : base(s) {
			maxresistance = 1E9;
			minresistance = 1E3;
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
			arr[0] = "Thermistor";
			getBasicInfo(arr);
			arr[3] = "R = " + getUnitText(resistance, CirSim.ohmString);
			arr[4] = "RMin = " + getUnitText(minresistance, CirSim.ohmString);
			arr[5] = "RMax = " + getUnitText(maxresistance, CirSim.ohmString);
		}

	}
}