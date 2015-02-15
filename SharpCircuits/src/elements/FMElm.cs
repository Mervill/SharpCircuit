using System;
using System.Collections;
using System.Collections.Generic;

namespace SharpCircuit {

	// Contributed by Edward Calver

	public class FMElm : CircuitElement {

		public Circuit.Lead leadOut { get { return lead0; } }

		/// <summary>
		/// Carrier Frequency (Hz)
		/// </summary>
		public double carrierfreq { get; set; }

		/// <summary>
		/// The signalfreq.
		/// </summary>
		public double signalfreq { get; set; }

		/// <summary>
		/// Max Voltage
		/// </summary>
		public double maxVoltage { get; set; }

		/// <summary>
		/// Deviation (Hz)
		/// </summary>
		public double deviation { get; set; }

		private double freqTimeZero;
		private double lasttime = 0;
		private double funcx = 0;

		public FMElm() : base() {
			deviation = 200;
			maxVoltage = 5;
			carrierfreq = 800;
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

		public override void doStep(Circuit sim) {
			sim.updateVoltageSource(0, lead_node[0], voltSource, getVoltage(sim.time));
		}

		private double getVoltage(double time) {
			double deltaT = time - lasttime;
			lasttime = time;
			double signalamplitude = Math.Sin((2 * pi * (time - freqTimeZero)) * signalfreq);
			funcx += deltaT * (carrierfreq + (signalamplitude * deviation));
			double w = 2 * pi * funcx;
			return Math.Sin(w) * maxVoltage;
		}

		public override double getVoltageDiff() {
			return lead_volt[0];
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
			arr[0] = "FM Source";
			arr[1] = "I = " + getCurrentText(current);
			arr[2] = "V = " + getVoltageText(getVoltageDiff());
			arr[3] = "cf = " + getUnitText(carrierfreq, "Hz");
			arr[4] = "sf = " + getUnitText(signalfreq, "Hz");
			arr[5] = "dev =" + getUnitText(deviation, "Hz");
			arr[6] = "Vmax = " + getVoltageText(maxVoltage);
		}

	}
}