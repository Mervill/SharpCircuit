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
		
		public SevenSegDecoderElm(int xx, int yy, CirSim s) : base(xx, yy, s) {

		}

		public bool hasReset() {
			return false;
		}
		
		public override String getChipName() {
			return "Seven Segment LED Decoder";
		}

		public override void setupPins() {
			sizeX = 3;
			sizeY = 7;
			pins = new Pin[getPostCount()];

			pins[7] = new Pin(0, SIDE_W, "I3", this);
			pins[8] = new Pin(1, SIDE_W, "I2", this);
			pins[9] = new Pin(2, SIDE_W, "I1", this);
			pins[10] = new Pin(3, SIDE_W, "I0", this);

			pins[0] = new Pin(0, SIDE_E, "a", this);
			pins[0].output = true;
			pins[1] = new Pin(1, SIDE_E, "b", this);
			pins[1].output = true;
			pins[2] = new Pin(2, SIDE_E, "c", this);
			pins[2].output = true;
			pins[3] = new Pin(3, SIDE_E, "d", this);
			pins[3].output = true;
			pins[4] = new Pin(4, SIDE_E, "e", this);
			pins[4].output = true;
			pins[5] = new Pin(5, SIDE_E, "f", this);
			pins[5].output = true;
			pins[6] = new Pin(6, SIDE_E, "g", this);
			pins[6].output = true;
		}

		public override int getPostCount() {
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