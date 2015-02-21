using System;
using System.Collections;
using System.Collections.Generic;

namespace SharpCircuit {

	public class SweepElm : CircuitElement {

		//public ElementLead leadOut { get { return lead0; }}

		/// <summary>
		/// Max Frequency (Hz)
		/// </summary>
		public double maxF {
			get {
				return _maxF;
			}
			set {
				_maxF = value;
				double maxfreq = 1 / (8 * sim.timeStep);
				if(_maxF > maxfreq) {
					_maxF = maxfreq;
					setParams();
				}
			}
		}

		/// <summary>
		/// Min Frequency (Hz)
		/// </summary>
		public double minF {
			get {
				return _minF;
			}
			set {
				_minF = value;
				double maxfreq = 1 / (8 * sim.timeStep);
				if(_minF > maxfreq) {
					_minF = maxfreq;
					setParams();
				}
			}
		}

		public bool logarithmic { get; set; }

		public bool bidirectional { get; set; }

		/// <summary>
		/// Sweep Time (s)
		/// </summary>
		public double sweepTime { get; set; }

		public double frequency { get; private set; }

		private double _maxF;
		private double _minF;

		private int dir = 1;
		private double fadd;
		private double fmul;
		private double freqTime;
		private double savedTimeStep;
		private double v;
		private double maxV;

		public SweepElm() {
			minF = 20;
			maxF = 4000;
			maxV = 5;
			sweepTime = 0.1;
			bidirectional = true;
			reset();
		}

		public override int getLeadCount() {
			return 1;
		}

		public override void stamp(Circuit sim) {
			sim.stampVoltageSource(0, lead_node[0], voltSource);
		}

		public void setParams(double timeStep) {
			if(frequency < minF || frequency > maxF) {
				frequency = minF;
				freqTime = 0;
				dir = 1;
			}
			if(logarithmic) {
				fadd = dir * timeStep * (maxF - minF) / sweepTime;
				fmul = 1;
			} else {
				fadd = 0;
				fmul = Math.Pow(maxF / minF, dir * timeStep / sweepTime);
			}
			savedTimeStep = timeStep;
		}

		public override void reset() {
			frequency = minF;
			freqTime = 0;
			dir = 1;
			setParams();
		}

		public override void startIteration(double timeStep) {
			// has timestep been changed?
			if(timeStep != savedTimeStep)
				setParams(timeStep);

			v = Math.Sin(freqTime) * maxV;
			freqTime += frequency * 2 * pi * timeStep;
			frequency = frequency * fmul + fadd;
			if(frequency >= maxF && dir == 1) {
				if(bidirectional) {
					fadd = -fadd;
					fmul = 1 / fmul;
					dir = -1;
				} else {
					frequency = minF;
				}
			}

			if(frequency <= minF && dir == -1) {
				fadd = -fadd;
				fmul = 1 / fmul;
				dir = 1;
			}
		}

		public override void doStep(Circuit sim) {
			sim.updateVoltageSource(0, lead_node[0], voltSource, v);
		}

		public override double getVoltageDelta() {
			return lead_volt[0];
		}

		public override int getVoltageSourceCount() {
			return 1;
		}

		public override bool hasGroundConnection(int n1) {
			return true;
		}

		/*public override void getInfo(String[] arr) {
			arr[0] = "sweep " + (logarithmic ? "(log)" : "(linear)");
			arr[1] = "I = " + getCurrentDText(current);
			arr[2] = "V = " + getVoltageText(volts[0]);
			arr[3] = "f = " + getUnitText(frequency, "Hz");
			arr[4] = "range = " + getUnitText(minF, "Hz") + " .. " + getUnitText(maxF, "Hz");
			arr[5] = "time = " + getUnitText(sweepTime, "s");
		}*/

	}
}