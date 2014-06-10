using System;
using System.Collections;
using System.Collections.Generic;

namespace Circuits {

	// Initializers	[X]
	// Properties	[X]
	// Leads		[X]
	// Test Basic	[_]
	// Test Prop	[_]
	public class DiodeElm : CircuitElement {

		protected Diode diode;

		/// <summary>
		/// Fwd Voltage @ 1A
		/// </summary>
		public double forwardDrop{ 
			get{
				return _forwardDrop;
			}
			set{
				forwardDrop = value;
				setup();
			}
		}
		protected double _forwardDrop;

		protected double defaultdrop = 0.805904783;

		/// <summary>
		/// Zener Voltage @ 5mA
		/// </summary>
		public double zvoltage{ 
			get{
				return _zvoltage;
			}
			set{
				_zvoltage = value;
				setup();
			}
		}
		protected double _zvoltage;

		public DiodeElm(CirSim s) : base(s) {
			diode = new Diode(sim);
			_forwardDrop = defaultdrop;
			_zvoltage = 0;
			setup();
		}

		public override bool nonLinear() {
			return true;
		}

		public virtual void setup() {
			diode.setup(_forwardDrop, _zvoltage);
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
			arr[4] = "Vf = " + getVoltageText(_forwardDrop);
		}

	}
}