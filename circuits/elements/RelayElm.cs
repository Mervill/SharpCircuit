using System;
using System.Collections;
using System.Collections.Generic;

namespace Circuits {

	// 0 = switch
	// 1 = switch end 1
	// 2 = switch end 2
	// ...
	// 3n   = coil
	// 3n+1 = coil
	// 3n+2 = end of coil resistor

	// Initializers	[X]
	// Properties	[X]
	// Leads		[_]
	// Test Basic	[_]
	// Test Prop	[_]
	public class RelayElm : CircuitElement {

		private Inductor ind;

		/// <summary>
		/// On Resistance (ohms)
		/// </summary>
		public double inductance {
			get {
				return _inductance;
			}
			set {
				_inductance = value;
				ind.setup(_inductance,coilCurrent,Inductor.FLAG_BACK_EULER);
			}
		}

		/// <summary>
		/// On Resistance (ohms)
		/// </summary>
		public double r_on {
			get {
				return _r_on;
			}
			set {
				if(value > 0)
					_r_on = value;
			}
		}

		/// <summary>
		/// Off Resistance (ohms)
		/// </summary>
		public double r_off {
			get {
				return _r_off;
			}
			set {
				if(value > 0)
					_r_off = value;
			}
		}

		/// <summary>
		/// On Current (A)
		/// </summary>
		public double onCurrent {
			get {
				return _onCurrent;
			}
			set {
				if(value > 0)
					_onCurrent = value;
			}
		}

		/// <summary>
		/// Number of Poles
		/// </summary>
		public int poleCount {
			get {
				return _poleCount;
			}
			set {
				if(value >= 1){
					_poleCount = value;
					setupPoles();
				}
			}
		}

		/// <summary>
		/// Coil Resistance (ohms)
		/// </summary>
		public double coilR{ get; set; }

		private double _inductance;
		private double _r_on;
		private double _r_off;
		private double _onCurrent;
		private int _poleCount;

		public ElementLead[] coilPosts;
		private ElementLead[][] swposts;

		private double coilCurrent, coilCurCount;
		private double[] switchCurrent, switchCurCount;
		private double d_position;
		private int i_position;
		private int nSwitch0 = 0;
		private int nSwitch1 = 1;
		private int nSwitch2 = 2;
		private int nCoil1, nCoil2, nCoil3;

		public RelayElm(CirSim s) : base(s) {

			ind = new Inductor(sim);
			inductance = 0.2;
			ind.setup(inductance, 0, Inductor.FLAG_BACK_EULER);
			onCurrent = 0.02;
			r_on = 0.05;
			r_off = 1e6;
			coilR = 20;
			coilCurrent = coilCurCount = 0;
			poleCount = 1;
			setupPoles();
		}

		public void setupPoles() {
			nCoil1 = 3 * poleCount;
			nCoil2 = nCoil1 + 1;
			nCoil3 = nCoil1 + 2;
			if (switchCurrent == null || switchCurrent.Length != poleCount) {
				switchCurrent = new double[poleCount];
				switchCurCount = new double[poleCount];
			}
		}

		public override ElementLead getLead(int n) {
			if (n < 3 * poleCount)
				return swposts[n / 3][n % 3];
			
			return coilPosts[n - 3 * poleCount];
		}

		public override int getLeadCount() {
			return 2 + poleCount * 3;
		}

		public override int getInternalNodeCount() {
			return 1;
		}

		public override void reset() {
			base.reset();
			ind.reset();
			coilCurrent = coilCurCount = 0;

			for(int i = 0; i != poleCount; i++)
				switchCurrent[i] = switchCurCount[i] = 0;

		}

		public override void stamp() {
			// inductor from coil post 1 to internal node
			ind.stamp(nodes[nCoil1], nodes[nCoil3]);
			// resistor from internal node to coil post 2
			sim.stampResistor(nodes[nCoil3], nodes[nCoil2], coilR);

			for(int i = 0; i != poleCount * 3; i++)
				sim.stampNonLinear(nodes[nSwitch0 + i]);
			
		}

		public override void startIteration() {
			ind.startIteration(volts[nCoil1] - volts[nCoil3]);

			// magic value to balance operate speed with reset speed
			// semi-realistically
			double magic = 1.3;
			double pmult = Math.Sqrt(magic + 1);
			double p = coilCurrent * pmult / onCurrent;
			d_position = Math.Abs(p * p) - 1.3;
			if (d_position < 0) {
				d_position = 0;
			}
			if (d_position > 1) {
				d_position = 1;
			}
			if (d_position < 0.1) {
				i_position = 0;
			} else if (d_position > .9) {
				i_position = 1;
			} else {
				i_position = 2;
				// System.out.println("ind " + this + " " + current + " " + voltdiff);
			}
		}

		public override bool nonLinear() {
			return true;
		}

		public override void doStep() {
			double voltdiff = volts[nCoil1] - volts[nCoil3];
			ind.doStep(voltdiff);
			int p;
			for (p = 0; p != poleCount * 3; p += 3) {
				sim.stampResistor(nodes[nSwitch0 + p], nodes[nSwitch1 + p],i_position == 0 ? r_on : r_off);
				sim.stampResistor(nodes[nSwitch0 + p], nodes[nSwitch2 + p],i_position == 1 ? r_on : r_off);
			}
		}

		public override void calculateCurrent() {
			double voltdiff = volts[nCoil1] - volts[nCoil3];
			coilCurrent = ind.calculateCurrent(voltdiff);

			// actually this isn't correct, since there is a small amount
			// of current through the switch when off
			int p;
			for (p = 0; p != poleCount; p++) {
				if (i_position == 2) {
					switchCurrent[p] = 0;
				} else {
					switchCurrent[p] = (volts[nSwitch0 + p * 3] - volts[nSwitch1 + p * 3 + i_position]) / r_on;
				}
			}
		}

		public override void getInfo(String[] arr) {
			arr[0] = i_position == 0 ? "relay (off)" : i_position == 1 ? "relay (on)" : "relay";
			int i;
			int ln = 1;
			for (i = 0; i != poleCount; i++) {
				arr[ln++] = "I" + (i + 1) + " = " + getCurrentDText(switchCurrent[i]);
			}
			arr[ln++] = "coil I = " + getCurrentDText(coilCurrent);
			arr[ln++] = "coil Vd = " + getVoltageDText(volts[nCoil1] - volts[nCoil2]);
		}

		public override bool getConnection(int n1, int n2) {
			return (n1 / 3 == n2 / 3);
		}

	}
}