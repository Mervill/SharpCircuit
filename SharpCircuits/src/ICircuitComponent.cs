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

		void startIteration(double timeStep);
		void doStep(Circuit sim);
		void stamp(Circuit sim);

		double getPower();

		void getInfo(String[] arr);
		void reset();

		// leads
		int getLeadCount();
		double getLeadVoltage(int ndx);
		void setLeadVoltage(int ndx, double voltage);

		// current
		double getCurrent();
		void setCurrent(int x, double c);
		void calculateCurrent();

		// voltage
		double getVoltageDiff();
		int getVoltageSourceCount();
		void setVoltageSource(int n, int v);

		// connection
		bool getConnection(int n1, int n2);
		bool hasGroundConnection(int n1);

		int getInternalLeadCount();
	}

	public static class ICircuitComponentExtensions {

		public static ScopeFrame GetScopeFrame(this ICircuitComponent component, long elapsedMilliseconds) {
			return new ScopeFrame {
				time = elapsedMilliseconds,
				current = component.getCurrent(),
				voltage = component.getVoltageDiff(),
				power = component.getPower(),
			};
		}

	}

}
