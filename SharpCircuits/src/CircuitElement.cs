using System;
using System.Collections;
using System.Collections.Generic;

namespace SharpCircuit {

	public abstract class CircuitElement : ICircuitElement {

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

		/*public int getBasicInfo(String[] arr) {
			arr[1] = "I = " + getCurrentDText(current);
			arr[2] = "Vd = " + getVoltageDText(getVoltageDiff());
			return 3;
		}*/

		protected void allocLeads() {
			lead_node = new int[getLeadCount() + getInternalLeadCount()];
			lead_volt = new double[getLeadCount() + getInternalLeadCount()];
		}

		public virtual void calculateCurrent() { }
		//public virtual double getPower() { return getVoltageDiff() * current; }
		//public virtual void getInfo(String[] arr) { }

		#region //// Interface ////

		public int getLeadNode(int lead_ndx) {
			if(lead_node == null) allocLeads();
			return lead_node[lead_ndx];
		}

		public void setLeadNode(int lead_ndx, int node_ndx) {
			if(lead_node == null) allocLeads();
			lead_node[lead_ndx] = node_ndx;
		}

		public virtual void beginStep(Circuit sim) { }
		public virtual void step(Circuit sim) { }
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
		
		public virtual void setCurrent(int voltSourceNdx, double c) { current = c; }
		#endregion

		#region //// Voltage ////
		public virtual double getVoltageDelta() { return lead_volt[0] - lead_volt[1]; }
		
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
		#endregion
	}
}
