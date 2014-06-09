using System;
using System.Collections;
using System.Collections.Generic;

namespace Circuits {

	public class SparkGapElm : CircuitElement {

		public double resistance, onresistance, offresistance, breakdown, holdcurrent;
		public bool state;

		public SparkGapElm( CirSim s) : base(s) {
			offresistance = 1e9;
			onresistance = 1e3;
			breakdown = 1e3;
			holdcurrent = 0.001;
			state = false;
		}

		public override bool nonLinear() {
			return true;
		}

		public override void calculateCurrent() {
			double vd = volts[0] - volts[1];
			current = vd / resistance;
		}

		public override void reset() {
			base.reset();
			state = false;
		}

		public override void startIteration() {
			if (Math.Abs(current) < holdcurrent) {
				state = false;
			}
			double vd = volts[0] - volts[1];
			if (Math.Abs(vd) > breakdown) {
				state = true;
			}
		}

		public override void doStep() {
			resistance = (state) ? onresistance : offresistance;
			sim.stampResistor(nodes[0], nodes[1], resistance);
		}

		public override void stamp() {
			sim.stampNonLinear(nodes[0]);
			sim.stampNonLinear(nodes[1]);
		}

		public override void getInfo(String[] arr) {
			arr[0] = "spark gap";
			getBasicInfo(arr);
			arr[3] = state ? "on" : "off";
			arr[4] = "Ron = " + getUnitText(onresistance, CirSim.ohmString);
			arr[5] = "Roff = " + getUnitText(offresistance, CirSim.ohmString);
			arr[6] = "Vbreakdown = " + getUnitText(breakdown, "V");
		}

		/*public EditInfo getEditInfo(int n) {
			// ohmString doesn't work here on linux
			if (n == 0) {
				return new EditInfo("On resistance (ohms)", onresistance, 0, 0);
			}
			if (n == 1) {
				return new EditInfo("Off resistance (ohms)", offresistance, 0, 0);
			}
			if (n == 2) {
				return new EditInfo("Breakdown voltage", breakdown, 0, 0);
			}
			if (n == 3) {
				return new EditInfo("Holding current (A)", holdcurrent, 0, 0);
			}
			return null;
		}

		public void setEditValue(int n, EditInfo ei) {
			if (ei.value > 0 && n == 0) {
				onresistance = ei.value;
			}
			if (ei.value > 0 && n == 1) {
				offresistance = ei.value;
			}
			if (ei.value > 0 && n == 2) {
				breakdown = ei.value;
			}
			if (ei.value > 0 && n == 3) {
				holdcurrent = ei.value;
			}
		}*/
	}
}