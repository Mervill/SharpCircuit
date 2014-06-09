using System;
using System.Collections;
using System.Collections.Generic;

namespace Circuits {

	// stub implementation of TriacElm, based on SCRElm
	// FIXME need to add TriacElm to srclist
	// FIXME need to uncomment TriacElm line from CirSim.java

	// Silicon-Controlled Rectifier
	// 3 nodes, 1 internal node
	// 0 = anode, 1 = cathode, 2 = gate
	// 0, 3 = variable resistor
	// 3, 2 = diode
	// 2, 1 = 50 ohm resistor

	public class TriacElm : CircuitElement {

		private static readonly int anode = 0;
		private static readonly int cnode = 1;
		private static readonly int gnode = 2;
		private static readonly int inode = 3;

		public Diode diode{ get; private set; }

		private double ia, ic, ig;
		private double lastvac, lastvag;

		/// <summary>
		/// Gate-Cathode Resistance (ohms)
		/// </summary>
		public double CResistance{ get; set; }

		/// <summary>
		/// Trigger Current (A)
		/// </summary>
		public double TriggerI{ get; set; }

		/// <summary>
		/// Holding Current (A)
		/// </summary>
		public double HoldingI{ get; set; }
		
		public ElementLead gate;

		public TriacElm(CirSim s) : base(s) {
			gate = new ElementLead(this,2);
			CResistance = 50;
			HoldingI = 0.0082;
			TriggerI = 0.01;
			diode = new Diode(sim);
			diode.setup(.8, 0);
		}

		public override bool nonLinear() {
			return true;
		}

		public override void reset() {
			volts[anode] = volts[cnode] = volts[gnode] = 0;
			diode.reset();
			lastvag = lastvac = 0;
		}

		public override ElementLead getLead(int n) {
			return (n == 0) ? lead0 : (n == 1) ? lead1 : gate;
		}

		public override int getLeadCount() {
			return 3;
		}

		public override int getInternalNodeCount() {
			return 1;
		}

		public override double getPower() {
			return (volts[anode] - volts[gnode]) * ia + (volts[cnode] - volts[gnode]) * ic;
		}

		public double aresistance;

		public override void stamp() {
			sim.stampNonLinear(nodes[anode]);
			sim.stampNonLinear(nodes[cnode]);
			sim.stampNonLinear(nodes[gnode]);
			sim.stampNonLinear(nodes[inode]);
			sim.stampResistor(nodes[gnode], nodes[cnode], CResistance);
			diode.stamp(nodes[inode], nodes[gnode]);
		}

		public override void doStep() {
			double vac = volts[anode] - volts[cnode]; // typically negative
			double vag = volts[anode] - volts[gnode]; // typically positive
			if (Math.Abs(vac - lastvac) > .01 || Math.Abs(vag - lastvag) > .01) {
				sim.converged = false;
			}
			lastvac = vac;
			lastvag = vag;
			diode.doStep(volts[inode] - volts[gnode]);
			double icmult = 1 / TriggerI;
			double iamult = 1 / HoldingI - icmult;
			// System.out.println(icmult + " " + iamult);
			aresistance = (-icmult * ic + ia * iamult > 1) ? .0105 : 10e5;
			// System.out.println(vac + " " + vag + " " + sim.converged + " " + ic +
			// " " + ia + " " + aresistance + " " + volts[inode] + " " +
			// volts[gnode] + " " + volts[anode]);
			sim.stampResistor(nodes[anode], nodes[inode], aresistance);
		}

		public override void getInfo(String[] arr) {
			arr[0] = "SCR";
			double vac = volts[anode] - volts[cnode];
			double vag = volts[anode] - volts[gnode];
			double vgc = volts[gnode] - volts[cnode];
			arr[1] = "Ia = " + getCurrentText(ia);
			arr[2] = "Ig = " + getCurrentText(ig);
			arr[3] = "Vac = " + getVoltageText(vac);
			arr[4] = "Vag = " + getVoltageText(vag);
			arr[5] = "Vgc = " + getVoltageText(vgc);
		}

		public override void calculateCurrent() {
			ic = (volts[cnode] - volts[gnode]) / CResistance;
			ia = (volts[anode] - volts[inode]) / aresistance;
			ig = -ic - ia;
		}

	}
}