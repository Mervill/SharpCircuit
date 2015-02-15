using System;
using System.Collections;
using System.Collections.Generic;

namespace SharpCircuit {

	public abstract class CircuitElement : ICircuitComponent {

		public readonly static double pi = 3.14159265358979323846;

		//protected CirSim sim;

		protected int voltSource;
		protected double current;
		protected int[] nodes;
		protected double[] volts;

		public CircuitElement() {
			allocNodes();
		}

		public int getBasicInfo(String[] arr) {
			arr[1] = "I = " + getCurrentDText(current);
			arr[2] = "Vd = " + getVoltageDText(getVoltageDiff());
			return 3;
		}

		protected void allocNodes() {
			nodes = new int[getLeadCount() + getInternalNodeCount()];
			volts = new double[getLeadCount() + getInternalNodeCount()];
		}

		public int getNode(int n) {
			if(nodes == null) allocNodes();
			return nodes[n];
		}

		public void setNode(int lead, int node) {
			if(nodes == null) allocNodes();
			nodes[lead] = node;
		}

		public double getLeadVoltage(int x) {
			return volts[x];
		}

		#region //// Virtuals ////
		public virtual void getInfo(String[] arr) { }

		public virtual void startIteration(double timeStep) { }
		public virtual void stamp(CirSim sim) { }
		public virtual void doStep(CirSim sim) { }
		public virtual void calculateCurrent() { }
		
		public virtual void reset() {
			for(int i = 0; i != getLeadCount() + getInternalNodeCount(); i++)
				volts[i] = 0;
		}

		public virtual double getCurrent() {
			return current;
		}

		public virtual void setCurrent(int x, double c) {
			current = c;
		}

		public virtual int getLeadCount() {
			return 2;
		}

		public virtual void setLeadVoltage(int n, double c) {
			volts[n] = c;
			calculateCurrent();
		}

		public virtual int getVoltageSourceCount() {
			return 0;
		}

		public virtual void setVoltageSource(int n, int v) {
			voltSource = v;
		}

		public virtual int getInternalNodeCount() {
			return 0;
		}

		public virtual double getVoltageDiff() {
			return volts[0] - volts[1];
		}

		public virtual double getPower() {
			return getVoltageDiff() * current;
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
		#endregion

		#region //// Static methods ////
		protected static bool comparePair(int x1, int x2, int y1, int y2) {
			return ((x1 == y1 && x2 == y2) || (x1 == y2 && x2 == y1));
		}

		public static String getVoltageDText(double v) {
			return getUnitText(Math.Abs(v), "V");
		}

		public static String getVoltageText(double v) {
			return getUnitText(v, "V");
		}

		public static string getCurrentText(double i) {
			return getUnitText(i, "A");
		}

		public static string getCurrentDText(double i) {
			return getUnitText(Math.Abs(i), "A");
		}

		public static string getUnitText(double v, string u) {
			double va = Math.Abs(v);
			if(va < 1e-14) return "0 " + u;
			if(va < 1e-9 ) return v * 1e12 + " p" + u;
			if(va < 1e-6 ) return v * 1e9  + " n" + u;
			if(va < 1e-3 ) return v * 1e6  + " " + CirSim.muString + u;
			if(va < 1    ) return v * 1e3  + " m" + u;
			if(va < 1e3  ) return v + " "  + u;
			if(va < 1e6  ) return v * 1e-3 + " k" + u;
			if(va < 1e9  ) return v * 1e-6 + " M" + u;
			return v * 1e-9 + " G" + u;
		}

		public static string getShortUnitText(double v, string u) {
			double va = Math.Abs(v);
			if(va < 1e-13) return null;
			if(va < 1e-9 ) return v * 1e12 + "p" + u;
			if(va < 1e-6 ) return v * 1e9  + "n" + u;
			if(va < 1e-3 ) return v * 1e6  + CirSim.muString + u;
			if(va < 1    ) return v * 1e3  + "m" + u;
			if(va < 1e3  ) return v + u;
			if(va < 1e6  ) return v * 1e-3 + "k" + u;
			if(va < 1e9  ) return v * 1e-6 + "M" + u;
			return v * 1e-9 + "G" + u;
		}
		#endregion
	}
}
