using System;
using System.Collections;
using System.Collections.Generic;

namespace Circuits {

	public class SevenSegDecoderElm : ChipElm {

		public static bool[,] symbols = {
				{ true, true, true, true, true, true, false },// 0
				{ false, true, true, false, false, false, false },// 1
				{ true, true, false, true, true, false, true },// 2
				{ true, true, true, true, false, false, true },// 3
				{ false, true, true, false, false, true, true },// 4
				{ true, false, true, true, false, true, true },// 5
				{ true, false, true, true, true, true, true },// 6
				{ true, true, true, false, false, false, false },// 7
				{ true, true, true, true, true, true, true },// 8
				{ true, true, true, false, false, true, true },// 9
				{ true, true, true, false, true, true, true },// A
				{ false, false, true, true, true, true, true },// B
				{ true, false, false, true, true, true, false },// C
				{ false, true, true, true, true, false, true },// D
				{ true, false, false, true, true, true, true },// E
				{ true, false, false, false, true, true, true },// F
		};
		
		public SevenSegDecoderElm(CirSim s) : base(s) {

		}

		public bool hasReset() {
			return false;
		}
		
		public override String getChipName() {
			return "Seven Segment LED Decoder";
		}

		public override void setupPins() {
			pins = new Pin[getLeadCount()];

			pins[7] = new Pin("I3");
			pins[8] = new Pin("I2");
			pins[9] = new Pin("I1");
			pins[10] = new Pin("I0");

			pins[0] = new Pin("a");
			pins[0].output = true;
			pins[1] = new Pin("b");
			pins[1].output = true;
			pins[2] = new Pin("c");
			pins[2].output = true;
			pins[3] = new Pin("d");
			pins[3].output = true;
			pins[4] = new Pin("e");
			pins[4].output = true;
			pins[5] = new Pin("f");
			pins[5].output = true;
			pins[6] = new Pin("g");
			pins[6].output = true;
		}

		public override int getLeadCount() {
			return 11;
		}

		public override int getVoltageSourceCount() {
			return 7;
		}

		public override void execute() {
			int input = 0;
			if (pins[7].value) {
				input += 8;
			}
			if (pins[8].value) {
				input += 4;
			}
			if (pins[9].value) {
				input += 2;
			}
			if (pins[10].value) {
				input += 1;
			}

			for (int i = 0; i < 7; i++) {
				pins[i].value = symbols[input,i];
			}
		}

	}
}