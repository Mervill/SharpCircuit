using System;
using System.Collections;
using System.Collections.Generic;

namespace SharpCircuit {

	// Silicon-Controlled Rectifier
	// 3 nodes, 1 internal node
	// 0 = anode, 1 = cathode, 2 = gate
	// 0, 3 = variable resistor
	// 3, 2 = diode
	// 2, 1 = 50 ohm resistor

	public class SCRElm : CircuitElement {

		public Circuit.Lead leadIn { get { return lead0; } }
		public Circuit.Lead leadOut { get { return lead1; } }
		public Circuit.Lead leadGate { get { return new Circuit.Lead(this, 2); } }

		/// <summary>
		/// Gate-Cathode Resistance (ohms)
		/// </summary>
		public double cresistance { get; set; }

		/// <summary>
		/// Trigger Current (A)
		/// </summary>
		public double triggerI { get; set; }

		/// <summary>
		/// Holding Current (A)
		/// </summary>
		public double holdingI { get; set; }

		private static readonly int anode = 0;
		private static readonly int cnode = 1;
		private static readonly int gnode = 2;
		private static readonly int inode = 3;

		private Diode diode;
		private double ia, ic, ig;
		private double lastvac;
		private double lastvag;

		public SCRElm() : base() {
			diode = new Diode();
			diode.setup(0.8, 0);
			cresistance = 50;
			holdingI = 0.0082;
			triggerI = 0.01;
		}

		public override bool nonLinear() { return true; }

		public override void reset() {
			lead_volt[anode] = lead_volt[cnode] = lead_volt[gnode] = 0;
			diode.reset();
			lastvag = lastvac = 0;
		}

		public override int getLeadCount() {
			return 3;
		}

		public override int getInternalLeadCount() {
			return 1;
		}

		/*public override double getPower() {
			return (lead_volt[anode] - lead_volt[gnode]) * ia + (lead_volt[cnode] - lead_volt[gnode]) * ic;
		}*/

		public double aresistance;

		public override void stamp(Circuit sim) {
			sim.stampNonLinear(lead_node[anode]);
			sim.stampNonLinear(lead_node[cnode]);
			sim.stampNonLinear(lead_node[gnode]);
			sim.stampNonLinear(lead_node[inode]);
			sim.stampResistor(lead_node[gnode], lead_node[cnode], cresistance);
			diode.stamp(sim, lead_node[inode], lead_node[gnode]);
		}

		public override void step(Circuit sim) {
			double vac = lead_volt[anode] - lead_volt[cnode]; // typically negative
			double vag = lead_volt[anode] - lead_volt[gnode]; // typically positive
			if(Math.Abs(vac - lastvac) > .01 || Math.Abs(vag - lastvag) > .01)
				sim.converged = false;
			lastvac = vac;
			lastvag = vag;
			diode.doStep(sim, lead_volt[inode] - lead_volt[gnode]);
			double icmult = 1 / triggerI;
			double iamult = 1 / holdingI - icmult;
			aresistance = (-icmult * ic + ia * iamult > 1) ? .0105 : 10e5;
			sim.stampResistor(lead_node[anode], lead_node[inode], aresistance);
		}

		/*public override void getInfo(String[] arr) {
			arr[0] = "SCR";
			double vac = lead_volt[anode] - lead_volt[cnode];
			double vag = lead_volt[anode] - lead_volt[gnode];
			double vgc = lead_volt[gnode] - lead_volt[cnode];
			arr[1] = "Ia = " + getCurrentText(ia);
			arr[2] = "Ig = " + getCurrentText(ig);
			arr[3] = "Vac = " + getVoltageText(vac);
			arr[4] = "Vag = " + getVoltageText(vag);
			arr[5] = "Vgc = " + getVoltageText(vgc);
		}*/

		public override void calculateCurrent() {
			ic = (lead_volt[cnode] - lead_volt[gnode]) / cresistance;
			ia = (lead_volt[anode] - lead_volt[inode]) / aresistance;
			ig = -ic - ia;
		}

	}
}