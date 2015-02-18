using System;
using System.Collections;
using System.Collections.Generic;

namespace SharpCircuit {

	public abstract class CircuitElement : ICircuitComponent {

		public readonly static double pi = 3.14159265358979323846;

		protected Circuit.Lead lead0 { get { return new Circuit.Lead(this, 0); } }
		protected Circuit.Lead lead1 { get { return new Circuit.Lead(this, 1); } }

		protected int voltSource;
		protected double current;
		protected int[]    lead_node;
		protected double[] lead_volt;

		public CircuitElement() {
			allocLeads();
		}

		public int getBasicInfo(String[] arr) {
			arr[1] = "I = " + getCurrentDText(current);
			arr[2] = "Vd = " + getVoltageDText(getVoltageDiff());
			return 3;
		}

		protected void allocLeads() {
			lead_node = new int[getLeadCount() + getInternalLeadCount()];
			lead_volt = new double[getLeadCount() + getInternalLeadCount()];
		}

		public int getLeadNode(int ndx) {
			if(lead_node == null) allocLeads();
			return lead_node[ndx];
		}

		public void setLeadNode(int ndx, int node_ndx) {
			if(lead_node == null) allocLeads();
			lead_node[ndx] = node_ndx;
		}

		public virtual void calculateCurrent() { }
		public virtual double getPower() { return getVoltageDiff() * current; }
		public virtual void getInfo(String[] arr) { }

		#region //// Interface ////

		public virtual void startIteration(double timeStep) { }
		public virtual void doStep(Circuit sim) { }
		public virtual void stamp(Circuit sim) { }

		public virtual void reset() {
			for(int i = 0; i != getLeadCount() + getInternalLeadCount(); i++)
				lead_volt[i] = 0;
		}

		#region //// Lead count ////
		public virtual int getLeadCount() { return 2; }
		public virtual int getInternalLeadCount() { return 0; }
		#endregion

		#region //// Lead voltage ////
		public virtual double getLeadVoltage(int leadX) {
			return lead_volt[leadX];
		}

		public virtual void setLeadVoltage(int leadX, double vValue) {
			lead_volt[leadX] = vValue;
			calculateCurrent();
		}
		#endregion

		#region //// Current ////
		public virtual double getCurrent() { return current; }
		
		public virtual void setCurrent(int voltSourceNdx, double cValue) { current = cValue; }
		#endregion

		#region //// Voltage ////
		public virtual double getVoltageDiff() { return lead_volt[0] - lead_volt[1]; }
		
		public virtual int getVoltageSourceCount() { return 0; }
		
		public virtual void setVoltageSource(int leadX, int voltSourceNdx) { voltSource = voltSourceNdx; }
		#endregion

		#region //// Connection ////
		public virtual bool leadsAreConnected(int leadX, int leadY) { return true; }
		
		public virtual bool leadIsGround(int leadX) { return false; }
		#endregion

		#region //// State ////
		public virtual bool isWire() { return false; }
		
		public virtual bool nonLinear() { return false; }
		#endregion

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

/*Debug.Log(CircuitElement.getUnitText(0.00000000000001, "V")); // 0.01pV
  Debug.Log(CircuitElement.getUnitText(0.000000000001, "V")); // 1pV
  Debug.Log(CircuitElement.getUnitText(0.000000001, "V")); // 1nP
  Debug.Log(CircuitElement.getUnitText(0.000001, "V")); // 1uV
  Debug.Log(CircuitElement.getUnitText(0.001, "V")); // 1mV
  Debug.Log(CircuitElement.getUnitText(1, "V")); // 1V
  Debug.Log(CircuitElement.getUnitText(1000, "V")); // 1KV
  Debug.Log(CircuitElement.getUnitText(1000000, "V")); // 1MV
  Debug.Log(CircuitElement.getUnitText(1000000000, "V")); // 1GV
  Debug.Log(CircuitElement.getUnitText(1000000000000, "V")); // 1TV
  Debug.Log(CircuitElement.getUnitText(1000000000000000, "V")); // 1000TV*/

		public static string getUnitText(double v, string u) {
			double va = Math.Abs(v);
			if(va < 1E-14) return "0" + u;
			if(va < 1E-9) return v * 1E12 + "p" + u; // pico
			if(va < 1E-6) return v * 1E9  + "n" + u; // nano
			if(va < 1E-3) return v * 1E6  + "u" + u; // micro
			if(va < 1   ) return v * 1E3  + "m" + u; // milli
			if(va < 1E3 ) return v + u;
			if(va < 1E6 ) return v * 1E-3 + "K" + u; // kilo
			if(va < 1E9 ) return v * 1E-6 + "M" + u; // mega
			if(va < 1E12) return v * 1E-9 + "G" + u; // giga
			return v * 1E-12 + "T" + u;              // tera
		}
		#endregion
	}
}
