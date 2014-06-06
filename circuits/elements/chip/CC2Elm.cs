using System;
using System.Collections;
using System.Collections.Generic;

namespace Circuits {

	public class CC2Elm : ChipElm {
		public double gain;

		public CC2Elm(int xx, int yy, CirSim s) : base(xx, yy, s) {
			gain = 1;
		}

		public CC2Elm(int xx, int yy, int g, CirSim s) : base(xx, yy, s) {
			gain = g;
		}

		public override String getChipName() {
			return "CC2";
		}

		public override void setupPins() {
			sizeX = 2;
			sizeY = 3;
			pins = new Pin[3];
			pins[0] = new Pin(0, SIDE_W, "X", this);
			pins[0].output = true;
			pins[1] = new Pin(2, SIDE_W, "Y", this);
			pins[2] = new Pin(1, SIDE_E, "Z", this);
		}

		public override void getInfo(String[] arr) {
			arr[0] = (gain == 1) ? "CCII+" : "CCII-";
			arr[1] = "X,Y = " + getVoltageText(volts[0]);
			arr[2] = "Z = " + getVoltageText(volts[2]);
			arr[3] = "I = " + getCurrentText(pins[0].current);
		}

		// boolean nonLinear() { return true; }
		public override void stamp() {
			// X voltage = Y voltage
			sim.stampVoltageSource(0, nodes[0], pins[0].voltSource);
			sim.stampVCVS(0, nodes[1], 1, pins[0].voltSource);
			// Z current = gain * X current
			sim.stampCCCS(0, nodes[2], pins[0].voltSource, gain);
		}

		/*public override void draw(Graphics g) {
			pins[2].current = pins[0].current * gain;
			drawChip(g);
		}*/

		public override int getPostCount() {
			return 3;
		}

		public override int getVoltageSourceCount() {
			return 1;
		}

	}

	class CC2NegElm : CC2Elm {
		
		public CC2NegElm(int xx, int yy, CirSim s) : base(xx, yy, s) {
			
		}
		
	}
}