using System;
using System.Collections;
using System.Collections.Generic;

namespace SharpCircuit {

	// Contributed by Edward Calver

	public class SeqGenElm : ChipElm {

		public bool Bit0Set {
			get {
				return (data & 1) != 0;
			}
			set {
				if(value) {
					data |= 1;
				} else {
					data &= ~1;
				}
			}
		}

		public bool Bit1set {
			get {
				return (data & 2) != 0;
			}
			set {
				if(value) {
					data |= 2;
				} else {
					data &= ~2;
				}
			}
		}

		public bool Bit2Set {
			get {
				return (data & 4) != 0;
			}
			set {
				if(value) {
					data |= 4;
				} else {
					data &= ~4;
				}
			}
		}

		public bool Bit3Set {
			get {
				return (data & 8) != 0;
			}
			set {
				if(value) {
					data |= 8;
				} else {
					data &= ~8;
				}
			}
		}

		public bool Bit4Set {
			get {
				return (data & 16) != 0;
			}
			set {
				if(value) {
					data |= 16;
				} else {
					data &= ~16;
				}
			}
		}

		public bool Bit5Set {
			get {
				return (data & 32) != 0;
			}
			set {
				if(value) {
					data |= 32;
				} else {
					data &= ~32;
				}
			}
		}

		public bool Bit6Set {
			get {
				return (data & 64) != 0;
			}
			set {
				if(value) {
					data |= 64;
				} else {
					data &= ~64;
				}
			}
		}

		public bool Bit7Set {
			get {
				return (data & 128) != 0;
			}
			set {
				if(value) {
					data |= 128;
				} else {
					data &= ~128;
				}
			}
		}

		public bool OneShot {
			get {
				return oneshot;
			}
			set {
				oneshot = value;
				if(oneshot) {
					position = 8;
				} else {
					position = 0;
				}
			}
		}

		private short data = 0;
		private byte position = 0;
		private bool oneshot = false;
		private double lastchangetime = 0;
		private bool clockstate = false;

		public SeqGenElm() : base() {

		}

		public bool hasReset() {
			return false;
		}

		public override String getChipName() {
			return "Sequence generator";
		}

		public override void setupPins() {
			pins = new Pin[getLeadCount()];

			pins[0] = new Pin("");
			pins[0].clock = true;
			pins[1] = new Pin("Q");
			pins[1].output = true;
		}

		public override int getLeadCount() {
			return 2;
		}

		public override int getVoltageSourceCount() {
			return 1;
		}

		void GetNextBit() {
			if(((data >> position) & 1) != 0) {
				pins[1].value = true;
			} else {
				pins[1].value = false;
			}
			position++;
		}

		public override void execute(CirSim sim) {
			if(oneshot) {
				if(sim.time - lastchangetime > 0.005) {
					if(position <= 8)
						GetNextBit();
					lastchangetime = sim.time;
				}
			}
			if(pins[0].value && !clockstate) {
				clockstate = true;
				if(oneshot) {
					position = 0;
				} else {
					GetNextBit();
					if(position >= 8)
						position = 0;
				}
			}
			if(!pins[0].value)
				clockstate = false;
		}

	}
}