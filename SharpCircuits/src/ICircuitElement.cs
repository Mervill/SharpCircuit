using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SharpCircuit {

	public class ScopeFrame {
		public double time { get; set; }
		public double current { get; set; }
		public double voltage { get; set; }
	}

	public interface ICircuitElement {

		void beginStep(Circuit sim);
		void step(Circuit sim);
		void stamp(Circuit sim);
		
		void reset();

		int getLeadNode(int lead_ndx);
		void setLeadNode(int lead_ndx, int node_ndx);

		// lead count
		int getLeadCount();
		int getInternalLeadCount();

		// lead voltage
		double getLeadVoltage(int leadX);
		void setLeadVoltage(int leadX, double vValue);

		// current
		double getCurrent();
		void setCurrent(int voltSourceNdx, double cValue);

		// voltage
		double getVoltageDelta();
		int getVoltageSourceCount();
		void setVoltageSource(int leadX, int voltSourceNdx);

		// connection
		bool leadsAreConnected(int leadX, int leadY);
		bool leadIsGround(int leadX);

		// state
		bool isWire();
		bool nonLinear();

	}

	public static class ICircuitComponentExtensions {

		public static ScopeFrame GetScopeFrame(this ICircuitElement elem, double time) {
			return new ScopeFrame {
				time = time,
				current = elem.getCurrent(),
				voltage = elem.getVoltageDelta(),
			};
		}

		public static string GetCurrentString(this ICircuitElement elem) {
			return SIUnits.Current(elem.getCurrent());
		}

		public static string GetVoltageString(this ICircuitElement elem) {
			return SIUnits.Voltage(elem.getVoltageDelta());
		}

	}

}
