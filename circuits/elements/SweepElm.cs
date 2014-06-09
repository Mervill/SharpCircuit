using System;
using System.Collections;
using System.Collections.Generic;

namespace Circuits {

	public class SweepElm : CircuitElement {

		public double maxV, maxF, minF, sweepTime, frequency;
		public int FLAG_LOG = 1;
		public int FLAG_BIDIR = 2;
		public double fadd, fmul, freqTime, savedTimeStep;
		public int dir = 1;
		public double v;

		public SweepElm(CirSim s) : base(s) {
			minF = 20;
			maxF = 4000;
			maxV = 5;
			sweepTime = .1;
			flags = FLAG_BIDIR;
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
			if ((flags & FLAG_LOG) == 0) {
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
			if (sim.timeStep != savedTimeStep) {
				setParams();
			}
			v = Math.Sin(freqTime) * maxV;
			freqTime += frequency * 2 * pi * sim.timeStep;
			frequency = frequency * fmul + fadd;
			if (frequency >= maxF && dir == 1) {
				if ((flags & FLAG_BIDIR) != 0) {
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
			arr[0] = "sweep " + (((flags & FLAG_LOG) == 0) ? "(linear)" : "(log)");
			arr[1] = "I = " + getCurrentDText(getCurrent());
			arr[2] = "V = " + getVoltageText(volts[0]);
			arr[3] = "f = " + getUnitText(frequency, "Hz");
			arr[4] = "range = " + getUnitText(minF, "Hz") + " .. "
					+ getUnitText(maxF, "Hz");
			arr[5] = "time = " + getUnitText(sweepTime, "s");
		}

		/*public EditInfo getEditInfo(int n) {
			if (n == 0) {
				return new EditInfo("Min Frequency (Hz)", minF, 0, 0);
			}
			if (n == 1) {
				return new EditInfo("Max Frequency (Hz)", maxF, 0, 0);
			}
			if (n == 2) {
				return new EditInfo("Sweep Time (s)", sweepTime, 0, 0);
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
				ei.checkbox = new Checkbox("Bidirectional",
						(flags & FLAG_BIDIR) != 0);
				return ei;
			}
			return null;
		}

		public void setEditValue(int n, EditInfo ei) {
			double maxfreq = 1 / (8 * sim.timeStep);
			if (n == 0) {
				minF = ei.value;
				if (minF > maxfreq) {
					minF = maxfreq;
				}
			}
			if (n == 1) {
				maxF = ei.value;
				if (maxF > maxfreq) {
					maxF = maxfreq;
				}
			}
			if (n == 2) {
				sweepTime = ei.value;
			}
			if (n == 3) {
				flags &= ~FLAG_LOG;
				if (ei.checkbox.getState()) {
					flags |= FLAG_LOG;
				}
			}
			if (n == 4) {
				maxV = ei.value;
			}
			if (n == 5) {
				flags &= ~FLAG_BIDIR;
				if (ei.checkbox.getState()) {
					flags |= FLAG_BIDIR;
				}
			}
			setParams();
		}*/
	}
}