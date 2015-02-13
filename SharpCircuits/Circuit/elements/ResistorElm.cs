using System;
using System.Collections;
using System.Collections.Generic;

namespace SharpCircuit {
	
	public class ResistorElm : CircuitElement {

		//public ElementLead leadIn	{ get { return lead0; }}
		//public ElementLead leadOut	{ get { return lead1; }}

		/// <summary>
		/// Resistance (ohms)
		/// </summary>
		public double resistance { get; set; }

		public ResistorElm(){
			resistance = 100;
		}

		public ResistorElm(double r) {
			resistance = r;
		}

		public override void calculateCurrent() {
			current = (volts[0] - volts[1]) / resistance;
			// System.out.print(this + " res current set to " + current + "\n");
		}

		public override void stamp() {
			sim.stampResistor(nodes[0], nodes[1], resistance);
		}

		public override void getInfo(String[] arr) {
			arr[0] = "resistor";
			getBasicInfo(arr);
			arr[3] = "R = " + getUnitText(resistance, CirSim.ohmString);
			arr[4] = "P = " + getUnitText(power(), "W");
		}

	}
}