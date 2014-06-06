using System;
using System.Collections;
using System.Collections.Generic;

namespace Circuits {

	public class SparkGapElm : CircuitElm {
		public double resistance, onresistance, offresistance, breakdown, holdcurrent;
		public bool state;

		public SparkGapElm(int xx, int yy, CirSim s) : base(xx, yy, s) {
			offresistance = 1e9;
			onresistance = 1e3;
			breakdown = 1e3;
			holdcurrent = 0.001;
			state = false;
		}

		public override bool nonLinear() {
			return true;
		}

		//public Polygon arrow1, arrow2;

//		public override void setPoints() {
//			base.setPoints();
//			int dist = 16;
//			int alen = 8;
//			calcLeads(dist + alen);
//			//Point p1 = interpPoint(point1, point2, (dn - alen) / (2 * dn));
//			//arrow1 = calcArrow(point1, p1, alen, alen);
//			//p1 = interpPoint(point1, point2, (dn + alen) / (2 * dn));
//			//arrow2 = calcArrow(point2, p1, alen, alen);
//		}

		/*public override void draw(Graphics g) {
			setBbox(point1, point2, 8);
			draw2Leads(g);
			setPowerColor(g, true);
			setVoltageColor(g, volts[0]);
			g.fillPolygon(arrow1);
			setVoltageColor(g, volts[1]);
			g.fillPolygon(arrow2);
			if (state) {
				doDots(g);
			}
			drawPosts(g);
		}*/

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