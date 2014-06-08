using System;
using System.Collections;
using System.Collections.Generic;

namespace Circuits {

	public abstract class CircuitElm {
		
		public static double voltageRange = 5.0;
		public static double currentMult, powerMult;

		protected CirSim sim;

		public static double pi = 3.14159265358979323846;

		public int[] nodes;
		public int x, y, x2, y2, flags, voltSource;
		public Point point1;
		public Point point2;
		public double[] volts;
		public double current, curcount;
		public bool noDiagonal;

		public CircuitElm(int xx, int yy, CirSim s) {
			point1 = new Point();
			point2 = new Point();

			flags = getDefaultFlags();
			allocNodes();
			//setPoints();
			sim = s;
		}

		public virtual int getDefaultFlags() {
			return 0;
		}

		public virtual void allocNodes() {
			nodes = new int[getPostCount() + getInternalNodeCount()];
			volts = new double[getPostCount() + getInternalNodeCount()];
		}

		public virtual void reset() {
			int i;
			for (i = 0; i != getPostCount() + getInternalNodeCount(); i++) {
				volts[i] = 0;
			}
			curcount = 0;
		}

		public virtual void setCurrent(int x, double c) {
			current = c;
		}

		public virtual double getCurrent() {
			return current;
		}

		public virtual void doStep() { }
		public virtual void startIteration() { }
		public virtual void doAdjust() { }
		public virtual void setupAdjust() { }
		public virtual void getInfo(String[] arr) { }
		public virtual void calculateCurrent() { }
		public virtual void stamp() { }

		public virtual double getPostVoltage(int x) {
			return volts[x];
		}

		public virtual void setNodeVoltage(int n, double c) {
			volts[n] = c;
			calculateCurrent();
		}

		/*public virtual void setPoints() {
			dx = x2 - x;
			dy = y2 - y;
			dn = Math.Sqrt(dx * dx + dy * dy);
			dpx1 = dy / dn;
			dpy1 = -dx / dn;
			dsign = (dy == 0) ? sign(dx) : sign(dy);
			point1 = new Point(x, y);
			point2 = new Point(x2, y2);
		}

		public virtual void calcLeads(int len) {
			if (dn < len || len == 0) {
				lead1 = point1;
				lead2 = point2;
				return;
			}
			lead1 = interpPoint(point1, point2, (dn - len) / (2 * dn));
			lead2 = interpPoint(point1, point2, (dn + len) / (2 * dn));
		}*/

		public virtual int getVoltageSourceCount() {
			return 0;
		}

		public virtual int getInternalNodeCount() {
			return 0;
		}

		public virtual void setNode(int p, int n) {
			nodes[p] = n;
		}

		public virtual void setVoltageSource(int n, int v) {
			voltSource = v;
		}

		public virtual int getVoltageSource() {
			return voltSource;
		}

		public virtual double getVoltageDiff() {
			return volts[0] - volts[1];
		}

		public virtual int getPostCount() {
			return 2;
		}

		public virtual int getNode(int n) {
			return nodes[n];
		}

		public virtual Point getPost(int n) {
			return (n == 0) ? point1 : (n == 1) ? point2 : null;
		}

		public virtual double getPower() {
			return getVoltageDiff() * current;
		}

		public virtual double getScopeValue(int x) {
			return (x == 1) ? getPower() : getVoltageDiff();
		}

		public virtual String getScopeUnits(int x) {
			return (x == 1) ? "W" : "V";
		}

		public virtual bool getConnection(int n1, int n2) {
			return true;
		}

		public virtual bool hasGroundConnection(int n1) {
			return false;
		}

		public virtual bool isWire() {
			return false;
		}

		public virtual bool nonLinear() {
			return false;
		}

		public virtual bool canViewInScope() {
			return getPostCount() <= 2;
		}

		public virtual bool comparePair(int x1, int x2, int y1, int y2) {
			return ((x1 == y1 && x2 == y2) || (x1 == y2 && x2 == y1));
		}

		public Point[] newPointArray(int n) {
			Point[] a = new Point[n];
			while (n > 0) {
				a[--n] = new Point();
			}
			return a;
		}

		public int getBasicInfo(String[] arr) {
			arr[1] = "I = " + getCurrentDText(getCurrent());
			arr[2] = "Vd = " + getVoltageDText(getVoltageDiff());
			return 3;
		}

		#region Static methods

		public static String getVoltageDText(double v) {
			return getUnitText(Math.Abs(v), "V");
		}
		
		public static String getVoltageText(double v) {
			return getUnitText(v, "V");
		}
		
		public static String getUnitText(double v, String u) {
			double va = Math.Abs(v);
			if (va < 1e-14) {
				return "0 " + u;
			}
			if (va < 1e-9) {
				return v * 1e12 + " p" + u;
			}
			if (va < 1e-6) {
				return v * 1e9 + " n" + u;
			}
			if (va < 1e-3) {
				return v * 1e6 + " " + CirSim.muString + u;
			}
			if (va < 1) {
				return v * 1e3 + " m" + u;
			}
			if (va < 1e3) {
				return v + " " + u;
			}
			if (va < 1e6) {
				return v * 1e-3 + " k" + u;
			}
			if (va < 1e9) {
				return v * 1e-6 + " M" + u;
			}
			return v * 1e-9 + " G" + u;
		}
		
		public static String getShortUnitText(double v, String u) {
			double va = Math.Abs(v);
			if (va < 1e-13) {
				return null;
			}
			if (va < 1e-9) {
				return v * 1e12 + "p" + u;
			}
			if (va < 1e-6) {
				return v * 1e9 + "n" + u;
			}
			if (va < 1e-3) {
				return v * 1e6 + CirSim.muString + u;
			}
			if (va < 1) {
				return v * 1e3 + "m" + u;
			}
			if (va < 1e3) {
				return v + u;
			}
			if (va < 1e6) {
				return v * 1e-3 + "k" + u;
			}
			if (va < 1e9) {
				return v * 1e-6 + "M" + u;
			}
			return v * 1e-9 + "G" + u;
		}
		
		public static String getCurrentText(double i) {
			return getUnitText(i, "A");
		}
		
		public static String getCurrentDText(double i) {
			return getUnitText(Math.Abs(i), "A");
		}

		public static int abs(int x) {
			return x < 0 ? -x : x;
		}
		
		public static int sign(int x) {
			return (x < 0) ? -1 : (x == 0) ? 0 : 1;
		}
		
		public static int min(int a, int b) {
			return (a < b) ? a : b;
		}
		
		public static int max(int a, int b) {
			return (a > b) ? a : b;
		}
		#endregion
	}
}
