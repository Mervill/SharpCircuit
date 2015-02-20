using System;
using System.Collections;
using System.Collections.Generic;

namespace SharpCircuit {

	// Contributed by Edward Calver.

	public class AMElm : CircuitElement {

		public Circuit.Lead leadOut { get { return lead0; } }

		/// <summary>
		/// Carrier Frequency (Hz)
		/// </summary>
		public double carrierfreq { get; set; }

		/// <summary>
		/// Signal Frequency (Hz)
		/// </summary>
		public double signalfreq { get; set; }

		/// <summary>
		/// Max Voltage
		/// </summary>
		public double maxVoltage { get; set; }

		private double freqTimeZero;

		public AMElm() : base() {
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

		public override void stamp(Circuit sim) {
			sim.stampVoltageSource(0, lead_node[0], voltSource);
		}

		public override void step(Circuit sim) {
			sim.updateVoltageSource(0, lead_node[0], voltSource, getVoltage(sim.time));
		}

		public double getVoltage(double time) {
			double w = 2 * pi * (time - freqTimeZero);
			return ((Math.Sin(w * signalfreq) + 1) / 2) * Math.Sin(w * carrierfreq) * maxVoltage;
		}

		public override double getVoltageDelta() {
			return lead_volt[0];
		}

		public override bool leadIsGround(int n1) {
			return true;
		}

		public override int getVoltageSourceCount() {
			return 1;
		}

		/*public override double getPower() {
			return -getVoltageDiff() * current;
		}*/

		/*public override void getInfo(String[] arr) {
			arr[0] = "AM Source";
			arr[1] = "I = " + getCurrentText(current);
			arr[2] = "V = " + getVoltageText(getVoltageDiff());
			arr[3] = "cf = " + getUnitText(carrierfreq, "Hz");
			arr[4] = "sf = " + getUnitText(signalfreq, "Hz");
			arr[5] = "Vmax = " + getVoltageText(maxVoltage);
		}*/
	}
}