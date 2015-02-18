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

	public interface ICircuitComponent {

		void beginStep(Circuit sim);
		void step(Circuit sim);
		void stamp(Circuit sim);
		
		void reset();

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
		double getVoltageDiff();
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

		public static ScopeFrame GetScopeFrame(this ICircuitComponent component, double time) {
			return new ScopeFrame {
				time = time,
				current = component.getCurrent(),
				voltage = component.getVoltageDiff(),
			};
		}

	}

}
