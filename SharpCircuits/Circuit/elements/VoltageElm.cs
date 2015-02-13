using System;
using System.Collections;
using System.Collections.Generic;

namespace SharpCircuit {

	public class VoltageElm : CircuitElement {

		public enum WaveType {
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
		public WaveType waveform {
			get {
				return _waveform;
			}
			set {
				WaveType ow = _waveform;
				_waveform = value;
				if(_waveform == WaveType.DC && ow != WaveType.DC) {
					bias = 0;
				}
			}
		}

		/// <summary>
		/// Frequency (Hz)
		/// </summary>
		public double frequency {
			get {
				return _frequency;
			}
			set {
				double oldfreq = _frequency;
				_frequency = value;
				double maxfreq = 1 / (8 * sim.timeStep);
				if(_frequency > maxfreq) {
					_frequency = maxfreq;
				}
				freqTimeZero = sim.time - oldfreq * (sim.time - freqTimeZero) / _frequency;
			}
		}

		/// <summary>
		/// Phase Offset (degrees)
		/// </summary>
		public double phaseShift {
			get {
				return _phaseShift * 180 / pi;
			}
			set {
				_phaseShift = value * pi / 180;
			}
		}

		/// <summary>
		/// Duty Cycle
		/// </summary>
		public double dutyCycle {
			get {
				return _dutyCycle * 10;
			}
			set {
				_dutyCycle = value * 0.01;
			}
		}

		/// <summary>
		/// The max voltage (AC) or flat voltage (DC)
		/// </summary>
		public double maxVoltage { get; set; }

		/// <summary>
		/// DC Offset (V)
		/// </summary>
		public double bias { get; set; }

		private WaveType _waveform;
		private double _frequency = 40;
		private double _phaseShift;
		private double _dutyCycle = 0.5;

		protected double freqTimeZero;

		public VoltageElm(WaveType wf) {
			waveform = wf;
			maxVoltage = 5;
			reset();
		}

		public override void reset() {
			freqTimeZero = 0;
		}

		public double triangleFunc(double x) {
			if(x < pi) {
				return x * (2 / pi) - 1;
			}
			return 1 - (x - pi) * (2 / pi);
		}

		public override void stamp() {
			if(waveform == WaveType.DC) {
				sim.stampVoltageSource(nodes[0], nodes[1], voltSource, getVoltage());
			} else {
				sim.stampVoltageSource(nodes[0], nodes[1], voltSource);
			}
		}

		public override void doStep() {
			if(waveform != WaveType.DC) {
				sim.updateVoltageSource(nodes[0], nodes[1], voltSource, getVoltage());
			}
		}

		public virtual double getVoltage() {
			double w = 2 * pi * (sim.time - freqTimeZero) * frequency + _phaseShift;
			switch(waveform) {
				case WaveType.DC:
					return maxVoltage + bias;
				case WaveType.AC:
					return Math.Sin(w) * maxVoltage + bias;
				case WaveType.SQUARE:
					return bias + ((w % (2 * pi) > (2 * pi * _dutyCycle)) ? -maxVoltage : maxVoltage);
				case WaveType.TRIANGLE:
					return bias + triangleFunc(w % (2 * pi)) * maxVoltage;
				case WaveType.SAWTOOTH:
					return bias + (w % (2 * pi)) * (maxVoltage / pi) - maxVoltage;
				case WaveType.PULSE:
					return ((w % (2 * pi)) < 1) ? maxVoltage + bias : bias;
				default:
					return 0;
			}
		}

		public override int getVoltageSourceCount() {
			return 1;
		}

		public override double power() {
			return -getVoltageDiff() * current;
		}

		public override double getVoltageDiff() {
			return volts[1] - volts[0];
		}

		public override void getInfo(String[] arr) {
			switch(waveform) {
				case WaveType.DC:
				case WaveType.VAR:
					arr[0] = "voltage source";
					break;
				case WaveType.AC:
					arr[0] = "A/C source";
					break;
				case WaveType.SQUARE:
					arr[0] = "square wave gen";
					break;
				case WaveType.PULSE:
					arr[0] = "pulse gen";
					break;
				case WaveType.SAWTOOTH:
					arr[0] = "sawtooth gen";
					break;
				case WaveType.TRIANGLE:
					arr[0] = "triangle gen";
					break;
			}
			arr[1] = "I = " + getCurrentText(current);
			arr[2] = ((this is RailElm) ? "V = " : "Vd = ") + getVoltageText(getVoltageDiff());
			if(waveform != WaveType.DC && waveform != WaveType.VAR) {
				arr[3] = "f = " + getUnitText(frequency, "Hz");
				arr[4] = "Vmax = " + getVoltageText(maxVoltage);
				int i = 5;
				if(bias != 0) {
					arr[i++] = "Voff = " + getVoltageText(bias);
				} else if(frequency > 500) {
					arr[i++] = "wavelength = "
							+ getUnitText(2.9979e8 / frequency, "m");
				}
				arr[i++] = "P = " + getUnitText(power(), "W");
			}
		}

	}
}