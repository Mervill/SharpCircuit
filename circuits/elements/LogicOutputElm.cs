using System;
using System.Collections;
using System.Collections.Generic;

namespace Circuits {

	public class LogicOutputElm : CircuitElement {

		public static readonly int FLAG_TERNARY = 1;
		public static readonly int FLAG_NUMERIC = 2;
		public static readonly int FLAG_PULLDOWN = 4;

		/// <summary>
		/// The Threshold Voltage.
		/// </summary>
		public double threshold{ get; set; }

		public LogicOutputElm( CirSim s) : base(s) {
			threshold = 2.5;
		}

		public override int getLeadCount() {
			return 1;
		}

		public bool isTernary() {
			return (flags & FLAG_TERNARY) != 0;
		}

		public bool isNumeric() {
			return (flags & (FLAG_TERNARY | FLAG_NUMERIC)) != 0;
		}

		public bool needsPullDown() {
			return (flags & FLAG_PULLDOWN) != 0;
		}

		public override void stamp() {
			if (needsPullDown()) {
				sim.stampResistor(nodes[0], 0, 1E6);
			}
		}

		public override double getVoltageDiff() {
			return volts[0];
		}

		public override void getInfo(String[] arr) {
			arr[0] = "logic output";
			arr[1] = (volts[0] < threshold) ? "low" : "high";
			arr[2] = "V = " + getVoltageText(volts[0]);
		}

	}
}