using System;
using System.Collections;
using System.Collections.Generic;

namespace Circuits {

	public class SevenSegElm : ChipElm {
		
		public SevenSegElm(int xx, int yy, CirSim s) : base(xx, yy, s) {

		}

		public override String getChipName() {
			return "7-segment driver/display";
		}

		//public Color darkred;

		public override void setupPins() {
			//darkred = new Color(30, 0, 0);
			sizeX = 4;
			sizeY = 4;
			pins = new Pin[7];
			pins[0] = new Pin(0, SIDE_W, "a", this);
			pins[1] = new Pin(1, SIDE_W, "b", this);
			pins[2] = new Pin(2, SIDE_W, "c", this);
			pins[3] = new Pin(3, SIDE_W, "d", this);
			pins[4] = new Pin(1, SIDE_S, "e", this);
			pins[5] = new Pin(2, SIDE_S, "f", this);
			pins[6] = new Pin(3, SIDE_S, "g", this);
		}

		/*void draw(Graphics g) {
			drawChip(g);
			g.setColor(Color.red);
			int xl = x + cspc * 5;
			int yl = y + cspc;
			setColor(g, 0);
			drawThickLine(g, xl, yl, xl + cspc, yl);
			setColor(g, 1);
			drawThickLine(g, xl + cspc, yl, xl + cspc, yl + cspc);
			setColor(g, 2);
			drawThickLine(g, xl + cspc, yl + cspc, xl + cspc, yl + cspc2);
			setColor(g, 3);
			drawThickLine(g, xl, yl + cspc2, xl + cspc, yl + cspc2);
			setColor(g, 4);
			drawThickLine(g, xl, yl + cspc, xl, yl + cspc2);
			setColor(g, 5);
			drawThickLine(g, xl, yl, xl, yl + cspc);
			setColor(g, 6);
			drawThickLine(g, xl, yl + cspc, xl + cspc, yl + cspc);
		}

		void setColor(Graphics g, int p) {
			g.setColor(pins[p].value ? Color.red : sim.printableCheckItem
					.getState() ? Color.white : darkred);
		}*/

		public override int getPostCount() {
			return 7;
		}

		public override int getVoltageSourceCount() {
			return 0;
		}

	}
}