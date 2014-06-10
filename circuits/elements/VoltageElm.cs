using System;
using System.Collections;
using System.Collections.Generic;

namespace Circuits {

	// Initializers	[X]
	// Properties	[X]
	// Leads		[X]
	// Test Basic	[_]
	// Test Prop	[_]
	public class VoltageElm : CircuitElement {

		public enum WaveformType {
			DC,
			AC,
			SQUARE,
			TRIANGLE,
			SAWTOOTH,
			PULSE,
			VAR
		}

		/// <summary>
		/// Waveform
		/// </summary>
		public WaveformType Waveform{ 
			get{
				return waveform;
			} 
			set {
				WaveformType ow = waveform;
				waveform = value;
				if (waveform == WaveformType.DC && ow != WaveformType.DC) {
					Bias = 0;
				}
			} 
		}
		protected WaveformType waveform;

		/// <summary>
		/// The max voltage (AC) or flat voltage (DC)
		/// </summary>
		public double MaxVoltage{ get; set; }

		/// <summary>
		/// DC Offset (V)
		/// </summary>
		public double Bias{ get; set; }

		/// <summary>
		/// Frequency (Hz)
		/// </summary>
		public double Frequency{ 
			get{
				return frequency;
			} 
			set{
				double oldfreq = frequency;
				frequency = value;
				double maxfreq = 1 / (8 * sim.timeStep);
				if (frequency > maxfreq) {
					frequency = maxfreq;
				}
				freqTimeZero = sim.time - oldfreq * (sim.time - freqTimeZero) / frequency;
			} 
		}
		protected double frequency;
		protected double freqTimeZero;

		/// <summary>
		/// Phase Offset (degrees)
		/// </summary>
		public double PhaseShift{ 
			get{
				return phaseShift * 180 / pi;
			} 
			set{
				phaseShift = value * pi / 180;
			} 
		}
		protected double phaseShift;

		/// <summary>
		/// Duty Cycle
		/// </summary>
		public double DutyCycle{ 
			get{
				return dutyCycle * 100;
			} 
			set{
				dutyCycle = value * 0.01;
			} 
		}
		protected double dutyCycle;

		public VoltageElm(CirSim s,WaveformType wf) : base(s) {
			waveform = wf;
			MaxVoltage = 5;
			frequency = 40;
			dutyCycle = 0.5;
			reset();
		}

		public override void reset() {
			freqTimeZero = 0;
		}

		public double triangleFunc(double x) {
			if (x < pi) {
				return x * (2 / pi) - 1;
			}
			return 1 - (x - pi) * (2 / pi);
		}

		public override void stamp() {
			if (waveform == WaveformType.DC) {
				sim.stampVoltageSource(nodes[0], nodes[1], voltSource, getVoltage());
			} else {
				sim.stampVoltageSource(nodes[0], nodes[1], voltSource);
			}
		}

		public override void doStep() {
			if (waveform != WaveformType.DC) {
				sim.updateVoltageSource(nodes[0], nodes[1], voltSource, getVoltage());
			}
		}

		public virtual double getVoltage() {
			double w = 2 * pi * (sim.time - freqTimeZero) * frequency + phaseShift;
			switch (waveform) {
			case WaveformType.DC:
				return MaxVoltage + Bias;
			case WaveformType.AC:
				return Math.Sin(w) * MaxVoltage + Bias;
			case WaveformType.SQUARE:
				return Bias + ((w % (2 * pi) > (2 * pi * dutyCycle)) ? -MaxVoltage : MaxVoltage);
			case WaveformType.TRIANGLE:
				return Bias + triangleFunc(w % (2 * pi)) * MaxVoltage;
			case WaveformType.SAWTOOTH:
				return Bias + (w % (2 * pi)) * (MaxVoltage / pi) - MaxVoltage;
			case WaveformType.PULSE:
				return ((w % (2 * pi)) < 1) ? MaxVoltage + Bias : Bias;
			default:
				return 0;
			}
		}

		public override int getVoltageSourceCount() {
			return 1;
		}

		public override double getPower() {
			return -getVoltageDiff() * current;
		}

		public override double getVoltageDiff() {
			return volts[1] - volts[0];
		}

		public override void getInfo(String[] arr) {
			switch (waveform) {
			case WaveformType.DC:
			case WaveformType.VAR:
				arr[0] = "voltage source";
				break;
			case WaveformType.AC:
				arr[0] = "A/C source";
				break;
			case WaveformType.SQUARE:
				arr[0] = "square wave gen";
				break;
			case WaveformType.PULSE:
				arr[0] = "pulse gen";
				break;
			case WaveformType.SAWTOOTH:
				arr[0] = "sawtooth gen";
				break;
			case WaveformType.TRIANGLE:
				arr[0] = "triangle gen";
				break;
			}
			arr[1] = "I = " + getCurrentText(getCurrent());
			arr[2] = ((this is RailElm) ? "V = " : "Vd = ") + getVoltageText(getVoltageDiff());
			if (waveform != WaveformType.DC && waveform != WaveformType.VAR) {
				arr[3] = "f = " + getUnitText(frequency, "Hz");
				arr[4] = "Vmax = " + getVoltageText(MaxVoltage);
				int i = 5;
				if (Bias != 0) {
					arr[i++] = "Voff = " + getVoltageText(Bias);
				} else if (frequency > 500) {
					arr[i++] = "wavelength = "
							+ getUnitText(2.9979e8 / frequency, "m");
				}
				arr[i++] = "P = " + getUnitText(getPower(), "W");
			}
		}

	}
}