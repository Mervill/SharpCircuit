using System;
using System.Collections.Generic;
using System.Linq;

using SharpCircuit;
using ServiceStack.Text;
using NUnit.Framework;

namespace SharpCircuitTest {

	[TestFixture]
	public class OpAmpTest {

		[Test]
		public void SimpleOpAmpTest() {
			Circuit sim = new Circuit();

			var volt0 = sim.Create<RailElm>();
			volt0.maxVoltage = 3;

			var volt1 = sim.Create<RailElm>();
			volt1.maxVoltage = 4;

			var opAmp0 = sim.Create<OpAmpElm>();
			var analogOut0 = sim.Create<OutputElm>();

			sim.Connect(volt0.leadOut, opAmp0.leadNeg);
			sim.Connect(volt1.leadOut, opAmp0.leadPos);
			sim.Connect(opAmp0.leadOut, analogOut0.leadIn);

			for(int x = 1; x <= 100; x++)
				sim.update(sim.timeStep);

			TestUtils.Compare(analogOut0.getVoltageDiff(), 15, 2);
		}

		[Test]
		public void OpAmpFeedbackTest() {
			Circuit sim = new Circuit();
			
			var volt0 = sim.Create<RailElm>();
			var opAmp0 = sim.Create<OpAmpElm>();
			var analogOut0 = sim.Create<OutputElm>();

			sim.Connect(opAmp0.leadOut, opAmp0.leadNeg);
			sim.Connect(volt0.leadOut, opAmp0.leadPos);
			sim.Connect(analogOut0.leadIn, opAmp0.leadOut);

			for(int x = 1; x <= 100; x++)
				sim.update(sim.timeStep);

			TestUtils.Compare(analogOut0.getVoltageDiff(), 5, 2);
		}

	}
}
