using System;
using System.Collections;
using System.Collections.Generic;

namespace SharpCircuit {

	public class TransLineElm : CircuitElement {

		/// <summary>
		/// Delay (s)
		/// </summary>
		public double delay { get; set; }

		/// <summary>
		/// Impedance (ohms)
		/// </summary>
		public double impedance { get; set; }

		private double[] voltageL, voltageR;
		private int lenSteps, ptr;
		private int voltSource1, voltSource2;
		private double current1, current2;

		public TransLineElm() {
			delay = 1000 * sim.timeStep;
			impedance = 75;
			reset();
		}

		public override int getLeadCount() {
			return 4;
		}
		
		public override int getInternalLeadCount() {
			return 2;
		}

		public override void reset() {
			if(sim.timeStep == 0)
				return;

			lenSteps = (int)(delay / sim.timeStep);
			//System.out.println(lenSteps + " steps");
			if(lenSteps > 100000) {
				voltageL = voltageR = null;
			} else {
				voltageL = new double[lenSteps];
				voltageR = new double[lenSteps];
			}
			ptr = 0;
			base.reset();
		}

		public override void setVoltageSource(int n, int v) {
			if(n == 0) {
				voltSource1 = v;
			} else {
				voltSource2 = v;
			}
		}

		public override void setCurrent(int v, double c) {
			if(v == voltSource1) {
				current1 = c;
			} else {
				current2 = c;
			}
		}

		public override void stamp(Circuit sim) {
			sim.stampVoltageSource(lead_node[4], lead_node[0], voltSource1);
			sim.stampVoltageSource(lead_node[5], lead_node[1], voltSource2);
			sim.stampResistor(lead_node[2], lead_node[4], impedance);
			sim.stampResistor(lead_node[3], lead_node[5], impedance);
		}
		
		public override void beginStep(Circuit sim) {
			// calculate voltages, currents sent over wire
			if(voltageL == null) {
				sim.panic("Transmission line delay too large!", this);
				return;
			}
			voltageL[ptr] = lead_volt[2] - lead_volt[0] + lead_volt[2] - lead_volt[4];
			voltageR[ptr] = lead_volt[3] - lead_volt[1] + lead_volt[3] - lead_volt[5];
			// System.out.println(volts[2] + " " + volts[0] + " " +
			// (volts[2]-volts[0]) + " " + (imped*current1) + " " + voltageL[ptr]);
			/*
			 * System.out.println("sending fwd  " + currentL[ptr] + " " + current1);
			 * System.out.println("sending back " + currentR[ptr] + " " + current2);
			 */
			// System.out.println("sending back " + voltageR[ptr]);
			ptr = (ptr + 1) % lenSteps;
		}

		public override void step(Circuit sim) {
			if(voltageL == null) {
				sim.panic("Transmission line delay too large!", this);
				return;
			}

			sim.updateVoltageSource(lead_node[4], lead_node[0], voltSource1, -voltageR[ptr]);
			sim.updateVoltageSource(lead_node[5], lead_node[1], voltSource2, -voltageL[ptr]);

			if(Math.Abs(lead_volt[0]) > 1e-5 || Math.Abs(lead_volt[1]) > 1e-5) {
				sim.panic("Need to ground transmission line!", this);
				return;
			}
		}

		// double getVoltageDiff() { return volts[0]; }
		public override int getVoltageSourceCount() {
			return 2;
		}

		public override bool leadIsGround(int n1) {
			return false;
		}

		public override bool leadsAreConnected(int n1, int n2) {
			return false;
			// if (comparePair(n1, n2, 0, 1)) return true; if (comparePair(n1, n2, 2, 3)) return true; return false;
		}

		/*public override void getInfo(String[] arr) {
			arr[0] = "transmission line";
			arr[1] = getUnitText(impedance, CirSim.ohmString);
			arr[2] = "length = " + getUnitText(2.9979e8 * delay, "m");
			arr[3] = "delay = " + getUnitText(delay, "s");
		}*/

	}
}