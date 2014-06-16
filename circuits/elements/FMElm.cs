using System;
using System.Collections;
using System.Collections.Generic;

namespace Circuits {

	// Contributed by Edward Calver

	// Initializers	[X]
	// Properties	[X]
	// Leads		[_]
	// Test Basic	[_]
	// Test Prop	[_]
	public class FMElm : CircuitElement {

		/// <summary>
		/// Carrier Frequency (Hz)
		/// </summary>
		public double carrierfreq{ get; set; }

		/// <summary>
		/// The signalfreq.
		/// </summary>
		public double signalfreq{ get; set; }

		/// <summary>
		/// Max Voltage
		/// </summary>
		public double maxVoltage{ get; set; }

		/// <summary>
		/// Deviation (Hz)
		/// </summary>
		public double deviation{ get; set; }

		private double freqTimeZero;
		private double lasttime = 0;
		private double funcx = 0;

		public FMElm(CirSim s) : base (s) {
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

		public override void stamp() {
			sim.stampVoltageSource(0, nodes[0], voltSource);
		}

		public override void doStep() {
			sim.updateVoltageSource(0, nodes[0], voltSource, getVoltage());
		}

		private double getVoltage() {
			double deltaT = sim.time - lasttime;
			lasttime = sim.time;
			double signalamplitude = Math.Sin((2 * pi * (sim.time - freqTimeZero)) * signalfreq);
			funcx += deltaT * (carrierfreq + (signalamplitude * deviation));
			double w = 2 * pi * funcx;
			return Math.Sin(w) * maxVoltage;
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
			arr[0] = "FM Source";
			arr[1] = "I = " + getCurrentText(getCurrent());
			arr[2] = "V = " + getVoltageText(getVoltageDiff());
			arr[3] = "cf = " + getUnitText(carrierfreq, "Hz");
			arr[4] = "sf = " + getUnitText(signalfreq, "Hz");
			arr[5] = "dev =" + getUnitText(deviation, "Hz");
			arr[6] = "Vmax = " + getVoltageText(maxVoltage);
		}
		
	}
}