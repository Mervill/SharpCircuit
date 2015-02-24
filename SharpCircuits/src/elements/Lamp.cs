using System;
using System.Collections;
using System.Collections.Generic;

namespace SharpCircuit {

	public class Lamp : CircuitElement {

		public static readonly double roomTemp = 300;

		public Circuit.Lead leadIn { get { return lead0; } }
		public Circuit.Lead leadOut { get { return lead1; } }

		/// <summary>
		/// Tempature
		/// </summary>
		public double temp { get; private set; }

		/// <summary>
		/// Nominal Power
		/// </summary>
		public double nom_pow { get; set; }

		/// <summary>
		/// Nominal Voltage
		/// </summary>
		public double nom_v { get; set; }

		/// <summary>
		/// Warmup Time (s)
		/// </summary>
		public double warmTime { get; set; }

		/// <summary>
		/// Cooldown Time (s)
		/// </summary>
		public double coolTime { get; set; }

		private double resistance;

		public Lamp() : base() {
			temp = roomTemp;
			nom_pow = 100;
			nom_v = 120;
			warmTime = 0.4;
			coolTime = 0.4;
		}

		public override void reset() {
			base.reset();
			temp = roomTemp;
		}

		public override void calculateCurrent() {
			current = (lead_volt[0] - lead_volt[1]) / resistance;
			// System.out.print(this + " res current set to " + current + "\n");
		}

		public override void stamp(Circuit sim) {
			sim.stampNonLinear(lead_node[0]);
			sim.stampNonLinear(lead_node[1]);
		}

		public override bool nonLinear() { return true; }

		public override void beginStep(Circuit sim) {
			// based on http://www.intusoft.com/nlpdf/nl11.pdf
			double nom_r = nom_v * nom_v / nom_pow;
			// this formula doesn't work for values over 5390
			double tp = (temp > 5390) ? 5390 : temp;
			resistance = nom_r * (1.26104 - 4.90662 * Math.Sqrt(17.1839 / tp - 0.00318794) - 7.8569 / (tp - 187.56));
			double cap = 1.57e-4 * nom_pow;
			double capw = cap * warmTime / .4;
			double capc = cap * coolTime / .4;
			// System.out.println(nom_r + " " + (resistance/nom_r));
			double voltageDiff = lead_volt[0] - lead_volt[1];
			double power = voltageDiff * current;
			temp += power * sim.timeStep / capw;
			double cr = 2600 / nom_pow;
			temp -= sim.timeStep * (temp - roomTemp) / (capc * cr);
			// System.out.println(capw + " " + capc + " " + temp + " " +resistance);
		}

		public override void step(Circuit sim) {
			sim.stampResistor(lead_node[0], lead_node[1], resistance);
		}

		/*public override void getInfo(String[] arr) {
			arr[0] = "lamp";
			getBasicInfo(arr);
			arr[3] = "R = " + getUnitText(resistance, Circuit.ohmString);
			arr[4] = "P = " + getUnitText(getPower(), "W");
			arr[5] = "T = " + ((int)temp) + " K";
		}*/

	}
}