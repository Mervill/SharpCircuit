using System;
using System.Collections;
using System.Collections.Generic;

namespace Circuits {

	public class TransLineElm : CircuitElement {

		/// <summary>
		/// Delay (s)
		/// </summary>
		public double Delay{ get; set; }

		/// <summary>
		/// Impedance (ohms)
		/// </summary>
		public double Impedance{ get; set; }

		private double[] voltageL, voltageR;
		private int lenSteps, ptr;
		private int voltSource1, voltSource2;

		public double current1, current2;

		public ElementLead[] leads;

		public TransLineElm(CirSim s) : base(s) {
			Delay = 1000 * sim.timeStep;
			Impedance = 75;
			leads = new ElementLead[] { 
				new ElementLead(this,0), 
				new ElementLead(this,1), 
				new ElementLead(this,2), 
				new ElementLead(this,3) 
			};
			reset();
		}

		public override int getLeadCount() {
			return 4;
		}

		public override int getInternalNodeCount() {
			return 2;
		}

		public override void reset() {
			if(sim.timeStep == 0)
				return;
			
			lenSteps = (int) (Delay / sim.timeStep);
			//System.out.println(lenSteps + " steps");
			if(lenSteps > 100000){
				voltageL = voltageR = null;
			}else{
				voltageL = new double[lenSteps];
				voltageR = new double[lenSteps];
			}
			ptr = 0;
			base.reset();
		}

		public override void setVoltageSource(int n, int v) {
			if(n == 0){
				voltSource1 = v;
			}else{
				voltSource2 = v;
			}
		}

		public override void setCurrent(int v, double c) {
			if(v == voltSource1){
				current1 = c;
			}else{
				current2 = c;
			}
		}

		public override void stamp() {
			sim.stampVoltageSource(nodes[4], nodes[0], voltSource1);
			sim.stampVoltageSource(nodes[5], nodes[1], voltSource2);
			sim.stampResistor(nodes[2], nodes[4], Impedance);
			sim.stampResistor(nodes[3], nodes[5], Impedance);
		}

		public override void startIteration() {
			// calculate voltages, currents sent over wire
			if (voltageL == null) {
				sim.stop("Transmission line delay too large!", this);
				return;
			}
			voltageL[ptr] = volts[2] - volts[0] + volts[2] - volts[4];
			voltageR[ptr] = volts[3] - volts[1] + volts[3] - volts[5];
			// System.out.println(volts[2] + " " + volts[0] + " " +
			// (volts[2]-volts[0]) + " " + (imped*current1) + " " + voltageL[ptr]);
			/*
			 * System.out.println("sending fwd  " + currentL[ptr] + " " + current1);
			 * System.out.println("sending back " + currentR[ptr] + " " + current2);
			 */
			// System.out.println("sending back " + voltageR[ptr]);
			ptr = (ptr + 1) % lenSteps;
		}

		public override void doStep() {
			if (voltageL == null) {
				sim.stop("Transmission line delay too large!", this);
				return;
			}

			sim.updateVoltageSource(nodes[4], nodes[0], voltSource1, -voltageR[ptr]);
			sim.updateVoltageSource(nodes[5], nodes[1], voltSource2, -voltageL[ptr]);

			if(Math.Abs(volts[0]) > 1e-5 || Math.Abs(volts[1]) > 1e-5){
				sim.stop("Need to ground transmission line!", this);
				return;
			}
		}

		public override ElementLead getLead(int n) {
			return leads[n];
		}

		// double getVoltageDiff() { return volts[0]; }
		public override int getVoltageSourceCount() {
			return 2;
		}

		public override bool hasGroundConnection(int n1) {
			return false;
		}

		public override bool getConnection(int n1, int n2) {
			return false;
			// if (comparePair(n1, n2, 0, 1)) return true; if (comparePair(n1, n2, 2, 3)) return true; return false;
		}

		public override void getInfo(String[] arr) {
			arr[0] = "transmission line";
			arr[1] = getUnitText(Impedance, CirSim.ohmString);
			arr[2] = "length = " + getUnitText(2.9979e8 * Delay, "m");
			arr[3] = "delay = " + getUnitText(Delay, "s");
		}

	}
}