using System;
using System.Collections;
using System.Collections.Generic;

namespace Circuits.Tests {

	public class OpAmpTest : CircuitTest {

		public OpAmpElm opamp;
		public VarRailElm upperVoltage;
		public VarRailElm lowerVoltage;
		public OutputElm output;

		public OpAmpTest() {

			opamp = new OpAmpElm(sim);

			upperVoltage = new VarRailElm(sim);
			upperVoltage.maxVoltage = 4;

			lowerVoltage = new VarRailElm(sim);
			lowerVoltage.maxVoltage = 3;

			output = new OutputElm(sim);

			upperVoltage.Attach(0,opamp.leadNeg);
			lowerVoltage.Attach(0,opamp.leadPos);

			opamp.leadOut.Connect(output.leadIn);

			sim.needAnalyze();
		}

	}


}