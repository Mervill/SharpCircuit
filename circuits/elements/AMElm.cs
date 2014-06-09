using System;
using System.Collections;
using System.Collections.Generic;

namespace Circuits {

	// Contributed by Edward Calver.
	public class AMElm : CircuitElement {

		public double carrierfreq, signalfreq, maxVoltage, freqTimeZero;

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

		/*public EditInfo getEditInfo(int n) {
			if (n == 0) {
				return new EditInfo("Max Voltage", maxVoltage, -20, 20);
			}
			if (n == 1) {
				return new EditInfo("Carrier Frequency (Hz)", carrierfreq, 4, 500);
			}
			if (n == 2) {
				return new EditInfo("Signal Frequency (Hz)", signalfreq, 4, 500);
			}

			return null;
		}

		public void setEditValue(int n, EditInfo ei) {
			if (n == 0) {
				maxVoltage = ei.value;
			}
			if (n == 1) {
				carrierfreq = ei.value;
			}
			if (n == 2) {
				signalfreq = ei.value;
			}
		}*/
	}
}