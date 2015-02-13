using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SharpCircuit {

	public class ScopeFrame {
		public long time { get; set; }
		public double current { get; set; }
		public double voltage { get; set; }
		public double power { get; set; }
	}

	public interface ICircuitComponent {

		bool isWire();
		bool nonLinear();

		void startIteration();
		void doStep();
		void stamp();

		double power(); // { get; }

		void getInfo(String[] arr);
		void reset();

		// leads
		int getLeadCount(); // { get; }
		double getLeadVoltage(int x);
		void setLeadVoltage(int n, double c);

		// current
		double current { get; }
		void calculateCurrent();
		void setCurrent(int x, double c);

		// voltage
		double getVoltageDiff();
		int getVoltageSourceCount(); // { get; }
		void setVoltageSource(int n, int v);

		// connection
		bool getConnection(int n1, int n2);
		bool hasGroundConnection(int n1);

		int getInternalNodeCount();
	}

	public static class ICircuitComponentExtensions {

		public static ScopeFrame GetScopeFrame(this ICircuitComponent component, long elapsedMilliseconds) {
			return new ScopeFrame {
				time = elapsedMilliseconds,
				current = component.current,
				voltage = component.getVoltageDiff(),
				power = component.power(),
			};
		}

	}

}
