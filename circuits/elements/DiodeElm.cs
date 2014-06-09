using System;
using System.Collections;
using System.Collections.Generic;

namespace Circuits {

	public class DiodeElm : CircuitElement {

		protected Diode diode;

		/// <summary>
		/// Fwd Voltage @ 1A
		/// </summary>
		public double forwardDrop{ get; set; }

		protected double defaultdrop = 0.805904783;

		/// <summary>
		/// Zener Voltage @ 5mA
		/// </summary>
		protected double zvoltage{ get; set; }

		public DiodeElm(CirSim s) : base(s) {
			diode = new Diode(sim);
			forwardDrop = defaultdrop;
			zvoltage = 0;
			setup();
		}

		public override bool nonLinear() {
			return true;
		}

		public virtual void setup() {
			diode.setup(forwardDrop, zvoltage);
		}

		public override void reset() {
			diode.reset();
			volts[0] = volts[1] = 0;
		}

		public override void stamp() {
			diode.stamp(nodes[0], nodes[1]);
		}

		public override void doStep() {
			diode.doStep(volts[0] - volts[1]);
		}

		public override void calculateCurrent() {
			current = diode.calculateCurrent(volts[0] - volts[1]);
		}

		public override void getInfo(String[] arr) {
			arr[0] = "diode";
			arr[1] = "I = " + getCurrentText(getCurrent());
			arr[2] = "Vd = " + getVoltageText(getVoltageDiff());
			arr[3] = "P = " + getUnitText(getPower(), "W");
			arr[4] = "Vf = " + getVoltageText(forwardDrop);
		}

	}
}