using System;
using System.Collections;
using System.Collections.Generic;

namespace SharpCircuit {

	public abstract class GateElm : CircuitElement {

		public Circuit.Lead leadOut { get { return new Circuit.Lead(this, inputCount); } }

		/// <summary>
		/// Number of inputs
		/// </summary>
		public int inputCount {
			get {
				return _inputCount;
			}
			set {
				_inputCount = value;
				allocLeads();
			}
		}

		protected int _inputCount;

		public GateElm() : base() {
			inputCount = 2;
		}

		public abstract bool calcFunction();

		public virtual bool isInverting() {
			return false;
		}

		public override int getLeadCount() {
			return inputCount + 1;
		}

		public override int getVoltageSourceCount() {
			return 1;
		}

		/*public override void getInfo(String[] arr) {
			arr[0] = "Logic Gate";
			arr[1] = "Vout = " + getVoltageText(lead_volt[inputCount]);
			arr[2] = "Iout = " + getCurrentText(current);
		}*/

		public override void stamp(Circuit sim) {
			sim.stampVoltageSource(0, lead_node[inputCount], voltSource);
		}

		public bool getInput(int x) {
			return lead_volt[x] > 2.5;
		}

		public override void step(Circuit sim) {
			bool f = calcFunction();
			if(isInverting()) f = !f;
			double res = f ? 5 : 0;
			sim.updateVoltageSource(0, lead_node[inputCount], voltSource, res);
		}

		// There is no current path through the gate inputs, but there
		// is an indirect path through the output to ground.
		public override bool leadsAreConnected(int n1, int n2) {
			return false;
		}

		public override bool leadIsGround(int n1) {
			return (n1 == inputCount);
		}
	}
}