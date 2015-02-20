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

			var volt0 = sim.Create<VoltageInputElm>(VoltageElm.WaveType.DC);
			volt0.maxVoltage = 3;

			var volt1 = sim.Create<VoltageInputElm>(VoltageElm.WaveType.DC);
			volt1.maxVoltage = 4;

			var opAmp0 = sim.Create<OpAmpElm>();
			var analogOut0 = sim.Create<OutputElm>();

			sim.Connect(volt0.leadVoltage, opAmp0.leadNeg);
			sim.Connect(volt1.leadVoltage, opAmp0.leadPos);
			sim.Connect(opAmp0.leadOut, analogOut0.leadIn);

			for(int x = 1; x <= 100; x++)
				sim.update();

			Debug.Log("Neg", opAmp0.getLeadVoltage(0));
			Debug.Log("Pos", opAmp0.getLeadVoltage(1));
			Debug.Log("Out", opAmp0.getLeadVoltage(2));
			Debug.Log("D", opAmp0.getVoltageDelta());
			Debug.Log(analogOut0.getLeadVoltage(0));
			TestUtils.Compare(analogOut0.getVoltageDelta(), 15, 2);
		}

		[Test]
		public void OpAmpFeedbackTest() {
			Circuit sim = new Circuit();

			var volt0 = sim.Create<VoltageInputElm>(VoltageElm.WaveType.DC);
			var opAmp0 = sim.Create<OpAmpElm>();
			var analogOut0 = sim.Create<OutputElm>();

			sim.Connect(opAmp0.leadOut, opAmp0.leadNeg);
			sim.Connect(volt0.leadVoltage, opAmp0.leadPos);
			sim.Connect(analogOut0.leadIn, opAmp0.leadOut);

			for(int x = 1; x <= 100; x++)
				sim.update();

			TestUtils.Compare(analogOut0.getVoltageDelta(), 5, 2);
		}

	}
}
