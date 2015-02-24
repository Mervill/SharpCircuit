using System;
using System.Collections;
using System.Collections.Generic;

namespace SharpCircuit {

	public class SevenSegElm : Chip {
		
		public SevenSegElm() : base() {

		}

		public override String getChipName() {
			return "7-segment driver/display";
		}

		public override void setupPins() {
			pins = new Pin[7];
			pins[0] = new Pin("a");
			pins[1] = new Pin("b");
			pins[2] = new Pin("c");
			pins[3] = new Pin("d");
			pins[4] = new Pin("e");
			pins[5] = new Pin("f");
			pins[6] = new Pin("g");
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

		public override int getLeadCount() {
			return 7;
		}

		public override int getVoltageSourceCount() {
			return 0;
		}

	}
}