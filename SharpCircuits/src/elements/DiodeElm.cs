using System;
using System.Collections;
using System.Collections.Generic;

namespace SharpCircuit {

	public class DiodeElm : CircuitElement {

		public Circuit.Lead leadIn { get { return lead0; } }
		public Circuit.Lead leadOut { get { return lead1; } }

		protected Diode diode;

		/// <summary>
		/// Fwd Voltage @ 1A
		/// </summary>
		public double forwardDrop {
			get {
				return _forwardDrop;
			}
			set {
				_forwardDrop = value;
				setup();
			}
		}

		/// <summary>
		/// Zener Voltage @ 5mA
		/// </summary>
		public double zvoltage {
			get {
				return _zvoltage;
			}
			set {
				_zvoltage = value;
				setup();
			}
		}

		protected double _forwardDrop;
		protected double _zvoltage;

		protected double defaultdrop = 0.805904783;

		public DiodeElm() : base() {
			diode = new Diode();
			forwardDrop = defaultdrop;
			zvoltage = 0;
			setup();
		}

		public override bool nonLinear() { return true; }

		public virtual void setup() {
			diode.setup(forwardDrop, zvoltage);
		}

		public override void reset() {
			diode.reset();
			lead_volt[0] = lead_volt[1] = 0;
		}

		public override void stamp(Circuit sim) {
			diode.stamp(sim, lead_node[0], lead_node[1]);
		}

		public override void step(Circuit sim) {
			diode.doStep(sim, lead_volt[0] - lead_volt[1]);
		}

		public override void calculateCurrent() {
			current = diode.calculateCurrent(lead_volt[0] - lead_volt[1]);
		}

		/*public override void getInfo(String[] arr) {
			arr[0] = "diode";
			arr[1] = "I = " + getCurrentText(current);
			arr[2] = "Vd = " + getVoltageText(getVoltageDiff());
			arr[3] = "P = " + getUnitText(getPower(), "W");
			arr[4] = "Vf = " + getVoltageText(forwardDrop);
		}*/

	}
}