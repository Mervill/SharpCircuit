using System;
using System.Collections;
using System.Collections.Generic;

namespace Circuits {

	public class LampElm : CircuitElement {

		public static readonly double roomTemp = 300;

		public double resistance;
		public double temp, nom_pow, nom_v, warmTime, coolTime;

		public LampElm(CirSim s) : base(s) {
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
			current = (volts[0] - volts[1]) / resistance;
			// System.out.print(this + " res current set to " + current + "\n");
		}

		public override void stamp() {
			sim.stampNonLinear(nodes[0]);
			sim.stampNonLinear(nodes[1]);
		}

		public override bool nonLinear() {
			return true;
		}

		public override void startIteration() {
			// based on http://www.intusoft.com/nlpdf/nl11.pdf
			double nom_r = nom_v * nom_v / nom_pow;
			// this formula doesn't work for values over 5390
			double tp = (temp > 5390) ? 5390 : temp;
			resistance = nom_r * (1.26104 - 4.90662 * Math.Sqrt(17.1839 / tp - 0.00318794) - 7.8569 / (tp - 187.56));
			double cap = 1.57e-4 * nom_pow;
			double capw = cap * warmTime / .4;
			double capc = cap * coolTime / .4;
			// System.out.println(nom_r + " " + (resistance/nom_r));
			temp += getPower() * sim.timeStep / capw;
			double cr = 2600 / nom_pow;
			temp -= sim.timeStep * (temp - roomTemp) / (capc * cr);
			// System.out.println(capw + " " + capc + " " + temp + " " +resistance);
		}

		public override void doStep() {
			sim.stampResistor(nodes[0], nodes[1], resistance);
		}

		public override void getInfo(String[] arr) {
			arr[0] = "lamp";
			getBasicInfo(arr);
			arr[3] = "R = " + getUnitText(resistance, CirSim.ohmString);
			arr[4] = "P = " + getUnitText(getPower(), "W");
			arr[5] = "T = " + ((int) temp) + " K";
		}

		/*public EditInfo getEditInfo(int n) {
			// ohmString doesn't work here on linux
			if (n == 0) {
				return new EditInfo("Nominal Power", nom_pow, 0, 0);
			}
			if (n == 1) {
				return new EditInfo("Nominal Voltage", nom_v, 0, 0);
			}
			if (n == 2) {
				return new EditInfo("Warmup Time (s)", warmTime, 0, 0);
			}
			if (n == 3) {
				return new EditInfo("Cooldown Time (s)", coolTime, 0, 0);
			}
			return null;
		}

		public void setEditValue(int n, EditInfo ei) {
			if (n == 0 && ei.value > 0) {
				nom_pow = ei.value;
			}
			if (n == 1 && ei.value > 0) {
				nom_v = ei.value;
			}
			if (n == 2 && ei.value > 0) {
				warmTime = ei.value;
			}
			if (n == 3 && ei.value > 0) {
				coolTime = ei.value;
			}
		}*/
	}
}