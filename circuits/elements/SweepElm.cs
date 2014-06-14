using System;
using System.Collections;
using System.Collections.Generic;

namespace Circuits {

	// Initializers	[X]
	// Properties	[X]
	// Leads		[_]
	// Test Basic	[_]
	// Test Prop	[_]
	public class SweepElm : CircuitElement {

		/// <summary>
		/// Max Frequency (Hz)
		/// </summary>
		public double maxF {
			get {
				return _maxF;
			}
			set {
				double maxfreq = 1 / (8 * sim.timeStep);
				_maxF = value;
				if (_maxF > maxfreq) {
					_maxF = maxfreq;
				}
			}
		}

		/// <summary>
		/// Min Frequency (Hz)
		/// </summary>
		public double minF {
			get {
				return _maxF;
			}
			set {
				double maxfreq = 1 / (8 * sim.timeStep);
				_minF = value;
				if (_minF > maxfreq) {
					_minF = maxfreq;
				}
			}
		}

		public bool logarithmic { get; set; }

		[System.ComponentModel.DefaultValue(true)]
		public bool bidirectional { get; set; }

		/// <summary>
		/// Sweep Time (s)
		/// </summary>
		public double sweepTime{ get; set; }

		public double frequency{ get; private set; }

		private double _maxF;
		private double _minF;
		private double _sweepTime;

		private int dir = 1;
		private double fadd;
		private double fmul;
		private double freqTime;
		private double savedTimeStep;
		private double v;
		private double maxV;

		public SweepElm(CirSim s) : base(s) {
			minF = 20;
			maxF = 4000;
			maxV = 5;
			sweepTime = 0.1;
			reset();
		}

		public override int getLeadCount() {
			return 1;
		}

		public override void stamp() {
			sim.stampVoltageSource(0, nodes[0], voltSource);
		}

		public void setParams() {
			if (frequency < minF || frequency > maxF) {
				frequency = minF;
				freqTime = 0;
				dir = 1;
			}
			if (logarithmic) {
				fadd = dir * sim.timeStep * (maxF - minF) / sweepTime;
				fmul = 1;
			} else {
				fadd = 0;
				fmul = Math.Pow(maxF / minF, dir * sim.timeStep / sweepTime);
			}
			savedTimeStep = sim.timeStep;
		}

		public override void reset() {
			frequency = minF;
			freqTime = 0;
			dir = 1;
			setParams();
		}

		public override void startIteration() {
			// has timestep been changed?
			if(sim.timeStep != savedTimeStep)
				setParams();
			
			v = Math.Sin(freqTime) * maxV;
			freqTime += frequency * 2 * pi * sim.timeStep;
			frequency = frequency * fmul + fadd;
			if (frequency >= maxF && dir == 1) {
				if (bidirectional) {
					fadd = -fadd;
					fmul = 1 / fmul;
					dir = -1;
				} else {
					frequency = minF;
				}
			}
			if (frequency <= minF && dir == -1) {
				fadd = -fadd;
				fmul = 1 / fmul;
				dir = 1;
			}
		}

		public override void doStep() {
			sim.updateVoltageSource(0, nodes[0], voltSource, v);
		}

		public override double getVoltageDiff() {
			return volts[0];
		}

		public override int getVoltageSourceCount() {
			return 1;
		}

		public override bool hasGroundConnection(int n1) {
			return true;
		}

		public override void getInfo(String[] arr) {
			arr[0] = "sweep " + (logarithmic ?  "(log)" : "(linear)");
			arr[1] = "I = " + getCurrentDText(getCurrent());
			arr[2] = "V = " + getVoltageText(volts[0]);
			arr[3] = "f = " + getUnitText(frequency, "Hz");
			arr[4] = "range = " + getUnitText(minF, "Hz") + " .. " + getUnitText(maxF, "Hz");
			arr[5] = "time = " + getUnitText(sweepTime, "s");
		}

		/*public EditInfo getEditInfo(int n) {
			if (n == 0) {
				return new EditInfo("", minF, 0, 0);
			}
			if (n == 1) {
				return new EditInfo("", maxF, 0, 0);
			}
			if (n == 2) {
				return new EditInfo("", sweepTime, 0, 0);
			}
			if (n == 3) {
				EditInfo ei = new EditInfo("", 0, -1, -1);
				ei.checkbox = new Checkbox("Logarithmic", (flags & FLAG_LOG) != 0);
				return ei;
			}
			if (n == 4) {
				return new EditInfo("Max Voltage", maxV, 0, 0);
			}
			if (n == 5) {
				EditInfo ei = new EditInfo("", 0, -1, -1);
				ei.checkbox = new Checkbox("Bidirectional",(flags & FLAG_BIDIR) != 0);
				return ei;
			}
			return null;
		}

		public void setEditValue(int n, EditInfo ei) {
			double maxfreq = 1 / (8 * sim.timeStep);

			setParams();
		}*/
	}
}