// stub implementation of TriacElm, based on SCRElm
// FIXME need to add TriacElm to srclist
// FIXME need to uncomment TriacElm line from CirSim.java

using System;
using System.Collections;
using System.Collections.Generic;

namespace Circuits {

	// Silicon-Controlled Rectifier
	// 3 nodes, 1 internal node
	// 0 = anode, 1 = cathode, 2 = gate
	// 0, 3 = variable resistor
	// 3, 2 = diode
	// 2, 1 = 50 ohm resistor

	public class TriacElm : CircuitElement {
		public int anode = 0;
		public int cnode = 1;
		public int gnode = 2;
		public int inode = 3;
		public Diode diode;

		public TriacElm( CirSim s) : base(s) {
			setDefaults();
			setup();
		}

		public void setDefaults() {
			cresistance = 50;
			holdingI = .0082;
			triggerI = .01;
		}

		public void setup() {
			diode = new Diode(sim);
			diode.setup(.8, 0);
		}

		public override bool nonLinear() {
			return true;
		}

		public override void reset() {
			volts[anode] = volts[cnode] = volts[gnode] = 0;
			diode.reset();
			lastvag = lastvac = 0;
		}

		public double ia, ic, ig;
		public double lastvac, lastvag;
		public double cresistance, triggerI, holdingI;

		public ElementLead gate;

//		public override void setPoints() {
//			base.setPoints();
//			int dir = 0;
//			if (abs(dx) > abs(dy)) {
//				dir = -sign(dx) * sign(dy);
//				point2.y = point1.y;
//			} else {
//				dir = sign(dy) * sign(dx);
//				point2.x = point1.x;
//			}
//			if (dir == 0) {
//				dir = 1;
//			}
//			calcLeads(16);
//			cathode = newPointArray(2);
//			Point[] pa = newPointArray(2);
//			interpPoint2(lead1, lead2, pa[0], pa[1], 0, hs);
//			interpPoint2(lead1, lead2, cathode[0], cathode[1], 1, hs);
//
//			gate = newPointArray(2);
//			double leadlen = (dn - 16) / 2;
//			int gatelen = sim.gridSize;
//			gatelen += (int)leadlen % sim.gridSize;
//			if (leadlen < gatelen) {
//				x2 = x;
//				y2 = y;
//				return;
//			}
//			interpPoint(lead2, point2, gate[0], gatelen / leadlen, gatelen * dir);
//			interpPoint(lead2, point2, gate[1], gatelen / leadlen, sim.gridSize * 2 * dir);
//		}

		/*public override void draw(Graphics g) {
			setBbox(point1, point2, hs);
			adjustBbox(gate[0], gate[1]);

			double v1 = volts[anode];
			double v2 = volts[cnode];

			draw2Leads(g);

			// draw arrow thingy
			setPowerColor(g, true);
			setVoltageColor(g, v1);
			g.fillPolygon(poly);

			// draw thing arrow is pointing to
			setVoltageColor(g, v2);
			drawThickLine(g, cathode[0], cathode[1]);

			drawThickLine(g, lead2, gate[0]);
			drawThickLine(g, gate[0], gate[1]);

			curcount_a = updateDotCount(ia, curcount_a);
			curcount_c = updateDotCount(ic, curcount_c);
			curcount_g = updateDotCount(ig, curcount_g);
			if (sim.dragElm != this) {
				drawDots(g, point1, lead2, curcount_a);
				drawDots(g, point2, lead2, curcount_c);
				drawDots(g, gate[1], gate[0], curcount_g);
				drawDots(g, gate[0], lead2, curcount_g + distance(gate[1], gate[0]));
			}
			drawPosts(g);
		}*/

		public override ElementLead getLead(int n) {
			return (n == 0) ? point0 : (n == 1) ? point1 : gate;
		}

		public override int getLeadCount() {
			return 3;
		}

		public override int getInternalNodeCount() {
			return 1;
		}

		public override double getPower() {
			return (volts[anode] - volts[gnode]) * ia + (volts[cnode] - volts[gnode]) * ic;
		}

		public double aresistance;

		public override void stamp() {
			sim.stampNonLinear(nodes[anode]);
			sim.stampNonLinear(nodes[cnode]);
			sim.stampNonLinear(nodes[gnode]);
			sim.stampNonLinear(nodes[inode]);
			sim.stampResistor(nodes[gnode], nodes[cnode], cresistance);
			diode.stamp(nodes[inode], nodes[gnode]);
		}

		public override void doStep() {
			double vac = volts[anode] - volts[cnode]; // typically negative
			double vag = volts[anode] - volts[gnode]; // typically positive
			if (Math.Abs(vac - lastvac) > .01 || Math.Abs(vag - lastvag) > .01) {
				sim.converged = false;
			}
			lastvac = vac;
			lastvag = vag;
			diode.doStep(volts[inode] - volts[gnode]);
			double icmult = 1 / triggerI;
			double iamult = 1 / holdingI - icmult;
			// System.out.println(icmult + " " + iamult);
			aresistance = (-icmult * ic + ia * iamult > 1) ? .0105 : 10e5;
			// System.out.println(vac + " " + vag + " " + sim.converged + " " + ic +
			// " " + ia + " " + aresistance + " " + volts[inode] + " " +
			// volts[gnode] + " " + volts[anode]);
			sim.stampResistor(nodes[anode], nodes[inode], aresistance);
		}

		public override void getInfo(String[] arr) {
			arr[0] = "SCR";
			double vac = volts[anode] - volts[cnode];
			double vag = volts[anode] - volts[gnode];
			double vgc = volts[gnode] - volts[cnode];
			arr[1] = "Ia = " + getCurrentText(ia);
			arr[2] = "Ig = " + getCurrentText(ig);
			arr[3] = "Vac = " + getVoltageText(vac);
			arr[4] = "Vag = " + getVoltageText(vag);
			arr[5] = "Vgc = " + getVoltageText(vgc);
		}

		public override void calculateCurrent() {
			ic = (volts[cnode] - volts[gnode]) / cresistance;
			ia = (volts[anode] - volts[inode]) / aresistance;
			ig = -ic - ia;
		}

		/*public override EditInfo getEditInfo(int n) {
			// ohmString doesn't work here on linux
			if (n == 0) {
				return new EditInfo("Trigger Current (A)", triggerI, 0, 0);
			}
			if (n == 1) {
				return new EditInfo("Holding Current (A)", holdingI, 0, 0);
			}
			if (n == 2) {
				return new EditInfo("Gate-Cathode Resistance (ohms)", cresistance,
						0, 0);
			}
			return null;
		}

		public override void setEditValue(int n, EditInfo ei) {
			if (n == 0 && ei.value > 0) {
				triggerI = ei.value;
			}
			if (n == 1 && ei.value > 0) {
				holdingI = ei.value;
			}
			if (n == 2 && ei.value > 0) {
				cresistance = ei.value;
			}
		}*/
	}
}