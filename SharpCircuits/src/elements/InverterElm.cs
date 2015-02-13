using System;
using System.Collections;
using System.Collections.Generic;

namespace SharpCircuit {
	
	public class InverterElm : CircuitElement {

		//public ElementLead leadIn 	{ get { return lead0; }}
		//public ElementLead leadOut 	{ get { return lead1; }}

		/// <summary>
		/// Slew Rate (V/ns)
		/// </summary>
		public double slewRate { get; set; }

		public InverterElm() {
			slewRate = 0.5;
		}

		public override int getVoltageSourceCount() {
			return 1;
		}

		public override void stamp() {
			sim.stampVoltageSource(0, nodes[1], voltSource);
		}

		public override void doStep() {
			double v0 = volts[1];
			double @out = volts[0] > 2.5 ? 0 : 5;
			double maxStep = slewRate * sim.timeStep * 1e9;
			@out = Math.Max(Math.Min(v0 + maxStep, @out), v0 - maxStep);
			sim.updateVoltageSource(0, nodes[1], voltSource, @out);
		}

		public override double getVoltageDiff() {
			return volts[0];
		}

		public override void getInfo(String[] arr) {
			arr[0] = "inverter";
			arr[1] = "Vi = " + getVoltageText(volts[0]);
			arr[2] = "Vo = " + getVoltageText(volts[1]);
		}

		// There is no current path through the inverter input, but there
		// is an indirect path through the output to ground.
		public override bool getConnection(int n1, int n2) {
			return false;
		}

		public override bool hasGroundConnection(int n1) {
			return (n1 == 1);
		}

	}
}