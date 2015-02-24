using System;
using System.Collections;
using System.Collections.Generic;

namespace SharpCircuit {

	public abstract class LogicGate : CircuitElement {

		/*public enum Op {
			Or,
			And,
			Nor,
			Nand,
			Xor,
		}*/

		public Circuit.Lead leadOut { get { return new Circuit.Lead(this, inputCount); } }

		//public Op logicOp { get; set; }

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

		public LogicGate() : base() {
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

		public override void stamp(Circuit sim) {
			sim.stampVoltageSource(0, lead_node[inputCount], voltSource);
		}

		public bool getInput(int x) {
			return lead_volt[x] > 2.5;
		}

		public override void step(Circuit sim) {
			bool f = calcFunction();
			if(isInverting()) f = !f;
			sim.updateVoltageSource(0, lead_node[inputCount], voltSource, f ? 5 : 0);
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