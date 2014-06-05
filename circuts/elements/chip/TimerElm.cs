using System;
using System.Collections;
using System.Collections.Generic;

namespace Circuts {

	public class TimerElm : ChipElm {
		public int FLAG_RESET = 2;
		public int N_DIS = 0;
		public int N_TRIG = 1;
		public int N_THRES = 2;
		public int N_VIN = 3;
		public int N_CTL = 4;
		public int N_OUT = 5;
		public int N_RST = 6;
		
		public TimerElm(int xx, int yy, CirSim s) : base(xx, yy, s) {

		}

		public override int getDefaultFlags() {
			return FLAG_RESET;
		}
		
		public override String getChipName() {
			return "555 Timer";
		}

		public override void setupPins() {
			sizeX = 3;
			sizeY = 5;
			pins = new Pin[7];
			pins[N_DIS] = new Pin(1, SIDE_W, "dis", this);
			pins[N_TRIG] = new Pin(3, SIDE_W, "tr", this);
			pins[N_TRIG].lineOver = true;
			pins[N_THRES] = new Pin(4, SIDE_W, "th", this);
			pins[N_VIN] = new Pin(1, SIDE_N, "Vin", this);
			pins[N_CTL] = new Pin(1, SIDE_S, "ctl", this);
			pins[N_OUT] = new Pin(2, SIDE_E, "out", this);
			pins[N_OUT].output = pins[N_OUT].state = true;
			pins[N_RST] = new Pin(1, SIDE_E, "rst", this);
		}

		public override bool nonLinear() {
			return true;
		}

		public bool hasReset() {
			return (flags & FLAG_RESET) != 0;
		}

		public override void stamp() {
			// stamp voltage divider to put ctl pin at 2/3 V
			sim.stampResistor(nodes[N_VIN], nodes[N_CTL], 5000);
			sim.stampResistor(nodes[N_CTL], 0, 10000);
			// output pin
			sim.stampVoltageSource(0, nodes[N_OUT], pins[N_OUT].voltSource);
			// discharge pin
			sim.stampNonLinear(nodes[N_DIS]);
		}

		public override void calculateCurrent() {
			// need current for V, discharge, control; output current is
			// calculated for us, and other pins have no current
			pins[N_VIN].current = (volts[N_CTL] - volts[N_VIN]) / 5000;
			pins[N_CTL].current = -volts[N_CTL] / 10000 - pins[N_VIN].current;
			pins[N_DIS].current = (!@out && !setOut) ? -volts[N_DIS] / 10 : 0;
		}

		bool setOut, @out;

		public override void startIteration() {
			@out = volts[N_OUT] > volts[N_VIN] / 2;
			setOut = false;
			// check comparators
			if (volts[N_CTL] / 2 > volts[N_TRIG]) {
				setOut = @out = true;
			}
			if (volts[N_THRES] > volts[N_CTL] || (hasReset() && volts[N_RST] < .7)) {
				@out = false;
			}
		}

		public override void doStep() {
			// if output is low, discharge pin 0. we use a small
			// resistor because it's easier, and sometimes people tie
			// the discharge pin to the trigger and threshold pins.
			// We check setOut to properly emulate the case where
			// trigger is low and threshold is high.
			if (!@out && !setOut) {
				sim.stampResistor(nodes[N_DIS], 0, 10);
			}
			// output
			sim.updateVoltageSource(0, nodes[N_OUT], pins[N_OUT].voltSource,@out ? volts[N_VIN] : 0);
		}

		public override int getPostCount() {
			return hasReset() ? 7 : 6;
		}

		public override int getVoltageSourceCount() {
			return 1;
		}

		public override int getDumpType() {
			return 165;
		}
	}
}