using System;
using System.Collections;
using System.Collections.Generic;

namespace SharpCircuit {

	public class ThermistorElm : CircuitElement {

		// stub ThermistorElm based on SparkGapElm
		// FIXME need to uncomment ThermistorElm line from CirSim.java
		// FIXME need to add ThermistorElm.java to srclist

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

		public double slider { get; set; }

		private double resistance;

		public ThermistorElm() : base() {
			maxResistance = 1E9;
			minResistance = 1E3;
		}

		public override bool nonLinear() {
			return true;
		}

		public override void calculateCurrent() {
			double vd = lead_volt[0] - lead_volt[1];
			current = vd / resistance;
		}

		public override void beginStep(Circuit sim) {
			// FIXME set resistance as appropriate, using slider.getValue()
			resistance = minResistance;
		}

		public override void step(Circuit sim) {
			sim.stampResistor(lead_node[0], lead_node[1], resistance);
		}

		public override void stamp(Circuit sim) {
			sim.stampNonLinear(lead_node[0]);
			sim.stampNonLinear(lead_node[1]);
		}

		/*public override void getInfo(String[] arr) {
			arr[0] = "Thermistor";
			getBasicInfo(arr);
			arr[3] = "R = " + getUnitText(resistance, Circuit.ohmString);
			arr[4] = "RMin = " + getUnitText(minResistance, Circuit.ohmString);
			arr[5] = "RMax = " + getUnitText(maxResistance, Circuit.ohmString);
		}*/

	}
}