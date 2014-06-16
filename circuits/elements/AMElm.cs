using System;
using System.Collections;
using System.Collections.Generic;

namespace Circuits {

	// Contributed by Edward Calver.

	// Initializers	[X]
	// Properties	[X]
	// Leads		[X]
	// Test Basic	[_]
	// Test Prop	[_]
	public class AMElm : CircuitElement {

		public ElementLead leadOut { get { return leads[0]; }}

		/// <summary>
		/// Carrier Frequency (Hz)
		/// </summary>
		public double carrierfreq{ get; set; }

		/// <summary>
		/// Signal Frequency (Hz)
		/// </summary>
		public double signalfreq{ get; set; }

		/// <summary>
		/// Max Voltage
		/// </summary>
		public double maxVoltage{ get; set; }

		private double freqTimeZero;

		public AMElm(CirSim s) : base (s) {
			maxVoltage = 5;
			carrierfreq = 1000;
			signalfreq = 40;
			reset();
		}

		public override void reset() {
			freqTimeZero = 0;
		}

		public override int getLeadCount() {
			return 1;
		}

		public override void stamp() {
			sim.stampVoltageSource(0, nodes[0], voltSource);
		}

		public override void doStep() {
			sim.updateVoltageSource(0, nodes[0], voltSource, getVoltage());
		}

		public double getVoltage() {
			double w = 2 * pi * (sim.time - freqTimeZero);
			return ((Math.Sin(w * signalfreq) + 1) / 2) * Math.Sin(w * carrierfreq) * maxVoltage;
		}

		public override double getVoltageDiff() {
			return volts[0];
		}

		public override bool hasGroundConnection(int n1) {
			return true;
		}

		public override int getVoltageSourceCount() {
			return 1;
		}

		public override double getPower() {
			return -getVoltageDiff() * current;
		}

		public override void getInfo(String[] arr) {
			arr[0] = "AM Source";
			arr[1] = "I = " + getCurrentText(getCurrent());
			arr[2] = "V = " + getVoltageText(getVoltageDiff());
			arr[3] = "cf = " + getUnitText(carrierfreq, "Hz");
			arr[4] = "sf = " + getUnitText(signalfreq, "Hz");
			arr[5] = "Vmax = " + getVoltageText(maxVoltage);
		}
	}
}