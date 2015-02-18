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
			
			int steps = 100;
			for(int x = 1; x <= steps; x++)
				sim.update(x);

			Assert.AreEqual(15, Math.Round(analogOut0.getVoltageDiff(), 2));
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

			int steps = 100;
			for(int x = 1; x <= steps; x++)
				sim.update(x);

			Assert.AreEqual(5, Math.Round(analogOut0.getVoltageDiff(), 4));
		}

	}
}
