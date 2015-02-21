using System;
using System.Collections;
using System.Collections.Generic;

namespace SharpCircuit {

	public class TimerElm : ChipElm {

		public bool hasResetPin {
			get {
				return _hasReset;
			}
			set {
				_hasReset = value;
				setupPins();
				allocLeads();
			}
		}

		public int N_DIS = 0;
		public int N_TRIG = 1;
		public int N_THRES = 2;
		public int N_VIN = 3;
		public int N_CTL = 4;
		public int N_OUT = 5;
		public int N_RST = 6;

		private bool _hasReset = true;

		private bool setOut, @out;

		public TimerElm() : base() {

		}

		public override String getChipName() {
			return "555 Timer";
		}

		public override void setupPins() {
			pins = new Pin[7];
			pins[N_DIS] = new Pin("dis");
			pins[N_TRIG] = new Pin("tr");
			pins[N_TRIG].lineOver = true;
			pins[N_THRES] = new Pin("th");
			pins[N_VIN] = new Pin("Vin");
			pins[N_CTL] = new Pin("ctl");
			pins[N_OUT] = new Pin("out");
			pins[N_OUT].output = true;
			pins[N_RST] = new Pin("rst");
		}

		public override bool nonLinear() { return true; }

		public override void stamp(Circuit sim) {
			// stamp voltage divider to put ctl pin at 2/3 V
			sim.stampResistor(lead_node[N_VIN], lead_node[N_CTL], 5000);
			sim.stampResistor(lead_node[N_CTL], 0, 10000);
			// output pin
			sim.stampVoltageSource(0, lead_node[N_OUT], pins[N_OUT].voltSource);
			// discharge pin
			sim.stampNonLinear(lead_node[N_DIS]);
		}

		public override void calculateCurrent() {
			// need current for V, discharge, control; output current is
			// calculated for us, and other pins have no current
			pins[N_VIN].current = (lead_volt[N_CTL] - lead_volt[N_VIN]) / 5000;
			pins[N_CTL].current = -lead_volt[N_CTL] / 10000 - pins[N_VIN].current;
			pins[N_DIS].current = (!@out && !setOut) ? -lead_volt[N_DIS] / 10 : 0;
		}

		public override void beginStep(Circuit sim) {
			@out = lead_volt[N_OUT] > lead_volt[N_VIN] / 2;
			setOut = false;
			// check comparators
			if(lead_volt[N_CTL] / 2 > lead_volt[N_TRIG])
				setOut = @out = true;
			if(lead_volt[N_THRES] > lead_volt[N_CTL] || (hasResetPin && lead_volt[N_RST] < .7))
				@out = false;
		}

		public override void step(Circuit sim) {
			// if output is low, discharge pin 0. we use a small
			// resistor because it's easier, and sometimes people tie
			// the discharge pin to the trigger and threshold pins.
			// We check setOut to properly emulate the case where
			// trigger is low and threshold is high.
			if(!@out && !setOut)
				sim.stampResistor(lead_node[N_DIS], 0, 10);
			// output
			sim.updateVoltageSource(0, lead_node[N_OUT], pins[N_OUT].voltSource, @out ? lead_volt[N_VIN] : 0);
		}

		public override int getLeadCount() {
			return hasResetPin ? 7 : 6;
		}

		public override int getVoltageSourceCount() {
			return 1;
		}

	}
}