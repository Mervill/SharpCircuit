using System;
using System.Collections;
using System.Collections.Generic;

namespace Circuits {

	public class TunnelDiodeElm : CircuitElm {
	
		public TunnelDiodeElm(int xx, int yy,CirSim s) : base(xx, yy,s) {
			setup();
		}

		public override bool nonLinear() {
			return true;
		}

		public void setup() { }

//		public override void setPoints() {
//			base.setPoints();
//			calcLeads(16);
//			cathode = newPointArray(4);
//			Point[] pa = newPointArray(2);
//			interpPoint2(lead1, lead2, pa[0], pa[1], 0, hs);
//			interpPoint2(lead1, lead2, cathode[0], cathode[1], 1, hs);
//			interpPoint2(lead1, lead2, cathode[2], cathode[3], .8, hs);
//			//poly = createPolygon(pa[0], pa[1], lead2);
//		}

		/*public override void draw(Graphics g) {
			setBbox(point1, point2, hs);

			double v1 = volts[0];
			double v2 = volts[1];

			draw2Leads(g);

			// draw arrow thingy
			setPowerColor(g, true);
			setVoltageColor(g, v1);
			g.fillPolygon(poly);

			// draw thing arrow is pointing to
			setVoltageColor(g, v2);
			drawThickLine(g, cathode[0], cathode[1]);
			drawThickLine(g, cathode[2], cathode[0]);
			drawThickLine(g, cathode[3], cathode[1]);

			doDots(g);
			drawPosts(g);
		}*/

		public override void reset() {
			lastvoltdiff = volts[0] = volts[1] = curcount = 0;
		}

		public double lastvoltdiff;

		public double limitStep(double vnew, double vold) {
			// Prevent voltage changes of more than 1V when iterating. Wow, I
			// thought it would be
			// much harder than this to prevent convergence problems.
			if (vnew > vold + 1) {
				return vold + 1;
			}
			if (vnew < vold - 1) {
				return vold - 1;
			}
			return vnew;
		}

		public override void stamp() {
			sim.stampNonLinear(nodes[0]);
			sim.stampNonLinear(nodes[1]);
		}

		public static double pvp = 0.1;
		public static double pip = 4.7e-3;
		public static double pvv = 0.37;
		public static double pvt = 0.026;
		public static double pvpp = 0.525;
		public static double piv = 370e-6;

		public override void doStep() {
			double voltdiff = volts[0] - volts[1];
			if (Math.Abs(voltdiff - lastvoltdiff) > 0.01) {
				sim.converged = false;
			}
			// System.out.println(voltdiff + " " + lastvoltdiff + " " +
			// Math.abs(voltdiff-lastvoltdiff));
			voltdiff = limitStep(voltdiff, lastvoltdiff);
			lastvoltdiff = voltdiff;

			double i = pip * Math.Exp(-pvpp / pvt) * (Math.Exp(voltdiff / pvt) - 1)
					+ pip * (voltdiff / pvp) * Math.Exp(1 - voltdiff / pvp) + piv
					* Math.Exp(voltdiff - pvv);

			double geq = pip * Math.Exp(-pvpp / pvt) * Math.Exp(voltdiff / pvt)
					/ pvt + pip * Math.Exp(1 - voltdiff / pvp) / pvp
					- Math.Exp(1 - voltdiff / pvp) * pip * voltdiff / (pvp * pvp)
					+ Math.Exp(voltdiff - pvv) * piv;
			double nc = i - geq * voltdiff;
			sim.stampConductance(nodes[0], nodes[1], geq);
			sim.stampCurrentSource(nodes[0], nodes[1], nc);
		}

		public override void calculateCurrent() {
			double voltdiff = volts[0] - volts[1];
			current = pip * Math.Exp(-pvpp / pvt) * (Math.Exp(voltdiff / pvt) - 1)
					+ pip * (voltdiff / pvp) * Math.Exp(1 - voltdiff / pvp) + piv
					* Math.Exp(voltdiff - pvv);
		}

		public override void getInfo(String[] arr) {
			arr[0] = "tunnel diode";
			arr[1] = "I = " + getCurrentText(getCurrent());
			arr[2] = "Vd = " + getVoltageText(getVoltageDiff());
			arr[3] = "P = " + getUnitText(getPower(), "W");
		}
	}
}