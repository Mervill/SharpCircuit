using System;
using System.Collections;
using System.Collections.Generic;

namespace Circuits {

	public abstract class CircuitElm {
		
		public static double voltageRange = 5;
		public static double currentMult, powerMult;
		public static Point ps1, ps2;
		public static CirSim sim;

		public static double pi = 3.14159265358979323846;

		public int[] nodes;
		public int x, y, x2, y2, flags, voltSource;
		public int dx, dy, dsign;
		public double dn, dpx1, dpy1;
		public Point point1, point2, lead1, lead2;
		public double[] volts;
		public double current, curcount;
		public bool noDiagonal;
		public bool selected;

		public CircuitElm(int xx, int yy, CirSim s) {
			x = x2 = xx;
			y = y2 = yy;
			flags = getDefaultFlags();
			allocNodes();
			setPoints();
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
		//public virtual void delete() { }
		public virtual void startIteration() { }
		public virtual void doAdjust() { }
		public virtual void setupAdjust() { }
		public virtual void getInfo(String[] arr) { }
		public virtual void calculateCurrent() { }

		public virtual double getPostVoltage(int x) {
			return volts[x];
		}

		public virtual void setNodeVoltage(int n, double c) {
			volts[n] = c;
			calculateCurrent();
		}

		public virtual void setPoints() {
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
		}

		public virtual void move(int dx, int dy) {
			x += dx;
			y += dy;
			x2 += dx;
			y2 += dy;
			setPoints();
		}

		// determine if moving this element by (dx,dy) will put it on top of another element
		public bool allowMove(int dx, int dy) {
			int nx = x + dx;
			int ny = y + dy;
			int nx2 = x2 + dx;
			int ny2 = y2 + dy;
			int i;
			for (i = 0; i != sim.elmList.Count; i++) {
				CircuitElm ce = sim.getElm(i);
				if (ce.x == nx && ce.y == ny && ce.x2 == nx2 && ce.y2 == ny2) {
					return false;
				}
				if (ce.x == nx2 && ce.y == ny2 && ce.x2 == nx && ce.y2 == ny) {
					return false;
				}
			}
			return true;
		}

		public void movePoint(int n, int dx, int dy) {
			if (n == 0) {
				x += dx;
				y += dy;
			} else {
				x2 += dx;
				y2 += dy;
			}
			setPoints();
		}

		public virtual void stamp() { }

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
		protected static Point interpPoint(Point a, Point b, double f) {
			Point p = new Point();
			interpPoint(a, b, p, f);
			return p;
		}
		
		protected static void interpPoint(Point a, Point b, Point c, double f) {
			// double q = (a.x*(1-f)+b.x*f+.48); System.out.println(q + " " + (int) q);
			c.x = (int) Math.Floor(a.x * (1 - f) + b.x * f + .48);
			c.y = (int) Math.Floor(a.y * (1 - f) + b.y * f + .48);
		}
		
		protected static void interpPoint(Point a, Point b, Point c, double f, double g) {
			int gx = b.y - a.y;
			int gy = a.x - b.x;
			g /= Math.Sqrt(gx * gx + gy * gy);
			c.x = (int) Math.Floor(a.x * (1 - f) + b.x * f + g * gx + .48);
			c.y = (int) Math.Floor(a.y * (1 - f) + b.y * f + g * gy + .48);
		}
		
		protected static Point interpPoint(Point a, Point b, double f, double g) {
			Point p = new Point();
			interpPoint(a, b, p, f, g);
			return p;
		}
		
		protected static void interpPoint2(Point a, Point b, Point c, Point d, double f, double g) {
			int gx = b.y - a.y;
			int gy = a.x - b.x;
			g /= Math.Sqrt(gx * gx + gy * gy);
			c.x = (int) Math.Floor(a.x * (1 - f) + b.x * f + g * gx + .48);
			c.y = (int) Math.Floor(a.y * (1 - f) + b.y * f + g * gy + .48);
			d.x = (int) Math.Floor(a.x * (1 - f) + b.x * f - g * gx + .48);
			d.y = (int) Math.Floor(a.y * (1 - f) + b.y * f - g * gy + .48);
		}

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
		
		public static double distance(Point p1, Point p2) {
			double x = p1.x - p2.x;
			double y = p1.y - p2.y;
			return Math.Sqrt(x * x + y * y);
		}
		#endregion
	}
}
