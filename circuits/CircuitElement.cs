using System;
using System.Collections;
using System.Collections.Generic;

namespace Circuits {

	public abstract class CircuitElement {

		public readonly static double pi = 3.14159265358979323846;

		public static double voltageRange = 5.0;
		public static double currentMult, powerMult;

		protected CirSim sim;
		protected int[] nodes;
		protected double[] volts;
		protected double current;
		
		public int voltSource;
		public ElementLead lead0;
		public ElementLead lead1;

		public CircuitElement(CirSim s) {
			lead0 = new ElementLead(this,0);
			lead1 = new ElementLead(this,1);

			allocNodes();

			sim = s;
			sim.elements.Add(this);
		}

		public int getBasicInfo(String[] arr) {
			arr[1] = "I = " + getCurrentDText(getCurrent());
			arr[2] = "Vd = " + getVoltageDText(getVoltageDiff());
			return 3;
		}

		#region Virtual's

		public virtual void doStep(){ }
		public virtual void startIteration(){ }
		public virtual void getInfo(String[] arr){ }
		public virtual void calculateCurrent(){ }
		public virtual void stamp(){ }

		public virtual void allocNodes() {
			nodes = new int[getLeadCount() + getInternalNodeCount()];
			volts = new double[getLeadCount() + getInternalNodeCount()];
		}

		public virtual void reset() {
			int i;
			for (i = 0; i != getLeadCount() + getInternalNodeCount(); i++) {
				volts[i] = 0;
			}
		}

		public virtual void setCurrent(int x, double c) {
			current = c;
		}

		public virtual double getCurrent() {
			return current;
		}

		public virtual double getLeadVoltage(int x) {
			return volts[x];
		}

		public virtual void setNodeVoltage(int n, double c) {
			volts[n] = c;
			calculateCurrent();
		}

		public virtual int getVoltageSourceCount() {
			return 0;
		}

		public virtual int getInternalNodeCount() {
			return 0;
		}

		public virtual void setNode(int lead, int node) {
			nodes[lead] = node;
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

		public virtual int getLeadCount() {
			return 2;
		}

		public virtual int getNode(int n) {
			return nodes[n];
		}

		public virtual ElementLead getLead(int n) {
			return (n == 0) ? lead0 : (n == 1) ? lead1 : null;
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

		/*public virtual bool canViewInScope() {
			return getLeadCount() <= 2;
		}*/

		public virtual bool comparePair(int x1, int x2, int y1, int y2) {
			return ((x1 == y1 && x2 == y2) || (x1 == y2 && x2 == y1));
		}

		#endregion

		public ElementLead[] newLeadArray(int n) {
			ElementLead[] a = new ElementLead[n];
			while (n > 0) {
				a[--n] = new ElementLead(this,n);
			}
			return a;
		}

		#region Attach

		// Attach [other] lead to the lead at the given index.
		public CircuitElement Attach(int lead,ElementLead other){
			getLead(lead).Connect(other);
			return other.element;
		}

		/*// Attach [other] lead to first unconnected lead on this element
		public CircuitElement AttachFirst(ElementLead other){
			for(int i = 0;i < getLeadCount();i++){
				ElementLead ld = getLead(i);
				if(!ld.IsConnected()){
					ld.Connect(other);
					break;
				}
			}
			return other.element;
		}*/

		// Attach the lead on [other] at [elm_lead] to the lead on this element at [lead].
		public CircuitElement Attach(int lead,CircuitElement other,int elm_lead){
			return(Attach(lead,other.getLead(elm_lead)));
		}

		public CircuitElement Next(CircuitElement other){
			//getLead(getLeadCount() -1).Connect(other.getLead(0));
			//return other;
			return Attach(getLeadCount() - 1,other,0);
		}

		#endregion

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
		#endregion
	}
}
