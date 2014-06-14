using System;
using System.Collections;
using System.Collections.Generic;

namespace Circuits {

	// Initializers	[X]
	// Properties	[X]
	// Leads		[X]
	// Test Basic	[_]
	// Test Prop	[_]
	public class MemristorElm : CircuitElement {

		/// <summary>
		/// Max Resistance (ohms)
		/// </summary>
		public double r_on{ get; set; }

		/// <summary>
		/// Min Resistance (ohms)
		/// </summary>
		public double r_off{ get; set; }

		/// <summary>
		/// Width of Doped Region (nm)
		/// </summary>
		public double dopeWidth {
			get {
				return _dopeWidth * 1E9;
			}
			set {
				_dopeWidth = value * 1E-9;
			}
		}

		/// <summary>
		/// Total Width (nm)
		/// </summary>
		public double totalWidth{
			get {
				return _totalWidth * 1E9;
			}
			set {
				_totalWidth = value * 1E-9;
			}
		}

		/// <summary>
		/// Mobility (um^2/(s*V))
		/// </summary>
		public double mobility{
			get {
				return _mobility * 1E12;
			}
			set {
				_mobility = value * 1E12;
			}
		}

		private double _dopeWidth;
		private double _totalWidth;
		private double _mobility;

		private double resistance;

		public MemristorElm( CirSim s) : base(s) {
			r_on = 100;
			r_off = 160 * r_on;
			dopeWidth = 0;
			totalWidth = 10E-9; // meters
			mobility = 1E-10; // m^2/sV
			resistance = 100;
		}

		public override bool nonLinear() {
			return true;
		}

		public override void calculateCurrent() {
			current = (volts[0] - volts[1]) / resistance;
		}

		public override void reset() {
			dopeWidth = 0;
		}

		public override void startIteration() {
			double wd = dopeWidth / totalWidth;
			dopeWidth += sim.timeStep * mobility * r_on * current / totalWidth;
			if (dopeWidth < 0) {
				dopeWidth = 0;
			}
			if (dopeWidth > totalWidth) {
				dopeWidth = totalWidth;
			}
			resistance = r_on * wd + r_off * (1 - wd);
		}

		public override void stamp() {
			sim.stampNonLinear(nodes[0]);
			sim.stampNonLinear(nodes[1]);
		}

		public override void doStep() {
			sim.stampResistor(nodes[0], nodes[1], resistance);
		}

		public override void getInfo(String[] arr) {
			arr[0] = "memristor";
			getBasicInfo(arr);
			arr[3] = "R = " + getUnitText(resistance, CirSim.ohmString);
			arr[4] = "P = " + getUnitText(getPower(), "W");
		}

		public override double getScopeValue(int x) {
			return (x == 2) ? resistance : (x == 1) ? getPower() : getVoltageDiff();
		}

		public override String getScopeUnits(int x) {
			return (x == 2) ? CirSim.ohmString : (x == 1) ? "W" : "V";
		}

	}
}