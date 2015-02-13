using System;
using System.Collections;
using System.Collections.Generic;

namespace SharpCircuit {
	
	public abstract class GateElm : CircuitElement {

		/// <summary>
		/// Number of inputs
		/// </summary>
		public int inputCount {
			get {
				return _inputCount;
			}
			set {
				_inputCount = value;
				allocNodes();
			}
		}

		protected int _inputCount;

		public bool lastOutput { get; set; }

		public GateElm() {
			inputCount = 2;
		}

		public abstract bool calcFunction();
		public abstract String getGateName();

		public virtual bool isInverting() {
			return false;
		}

		public override int getLeadCount() {
			return inputCount + 1;
		}

		public override int getVoltageSourceCount() {
			return 1;
		}

		public override void getInfo(String[] arr) {
			arr[0] = getGateName();
			arr[1] = "Vout = " + getVoltageText(volts[inputCount]);
			arr[2] = "Iout = " + getCurrentText(current);
		}

		public override void stamp() {
			sim.stampVoltageSource(0, nodes[inputCount], voltSource);
		}

		public bool getInput(int x) {
			return volts[x] > 2.5;
		}

		public override void doStep() {
			bool f = calcFunction();
			if (isInverting()) {
				f = !f;
			}
			lastOutput = f;
			double res = f ? 5 : 0;
			sim.updateVoltageSource(0, nodes[inputCount], voltSource, res);
		}

		// There is no current path through the gate inputs, but there
		// is an indirect path through the output to ground.
		public override bool getConnection(int n1, int n2) {
			return false;
		}

		public override bool hasGroundConnection(int n1) {
			return (n1 == inputCount);
		}
	}
}