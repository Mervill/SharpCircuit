using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

namespace Circuits {

	public class LEDMatrixElm : ChipElm {

		[DefaultValue(false)]
		public bool negateRows { get; set; }

		[DefaultValue(false)]
		public bool negateColumns { get; set; }

		[DefaultValue(1.0)]
		public double colorR { get; set; }

		[DefaultValue(0.0)]
		public double colorG { get; set; }

		[DefaultValue(0.0)]
		public double colorB { get; set; }

		public LEDMatrixElm(CirSim s) : base(s) {

		}

		public override String getChipName() {
			return "LED Matrix";
		}

		public override void setupPins() {
			pins = new Pin[16];
			pins[0] = new Pin("R0");
			pins[1] = new Pin("R1");
			pins[2] = new Pin("R2");
			pins[3] = new Pin("R3");
			pins[4] = new Pin("R4");
			pins[5] = new Pin("R5");
			pins[6] = new Pin("R6");
			pins[7] = new Pin("R7");

			pins[8] = new Pin("C0");
			pins[9] = new Pin("C1");
			pins[10] = new Pin("C2");
			pins[11] = new Pin("C3");
			pins[12] = new Pin("C4");
			pins[13] = new Pin("C5");
			pins[14] = new Pin("C6");
			pins[15] = new Pin("C7");
		}

		/*public override void draw(Graphics g) {
			drawChip(g);
			Color color = new Color((int) (colorR * 255), (int) (colorG * 255),
					(int) (colorB * 255));
			for (int col = 0; col < size; col++) {
				for (int row = 0; row < size; row++) {
					int centreX = x + 2 * (col + 1) * cspc;
					int centreY = y + 2 * row * cspc;
					int radius = cspc / 2;
					if ((negateRows ^ pins[row].value)
							&& (negateColumns ^ pins[col + 8].value)) {
						g.setColor(color);
						g.fillOval(centreX - radius, centreY - radius, radius * 2,
								radius * 2);
					}
					g.setColor(Color.gray);
					radius = (3 * cspc) / 4;
					drawThickCircle(g, centreX, centreY, radius);
				}
			}
		}*/

		public override int getLeadCount() {
			return 16;
		}

		public override int getVoltageSourceCount() {
			return 0;
		}

	}
}