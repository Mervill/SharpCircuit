using System;
using System.Collections.Generic;
using System.Linq;

using SharpCircuit;
using ServiceStack.Text;
using NUnit.Framework;

namespace SharpCircuitTest {

	[TestFixture]
	public class VoltageWaveTest {

		[Test]
		public void DCWaveTest() {
			// Flat ... is a type of wave... I guess.
			Assert.Ignore("Not Implemented!");
		}

		[Test]
		public void ACWaveTest() {
			Circuit sim = new Circuit();

			var voltage0 = sim.Create<VoltageInput>(Voltage.WaveType.AC);

			var res0 = sim.Create<Resistor>();
			var ground0 = sim.Create<Ground>();

			sim.Connect(voltage0.leadPos, res0.leadIn);
			sim.Connect(res0.leadOut, ground0.leadIn);

			var resScope0 = sim.Watch(res0);

			double cycleTime = 1 / voltage0.frequency;
			double quarterCycleTime = cycleTime / 4;

			int steps = (int)(cycleTime / sim.timeStep);
			for(int x = 1; x <= steps; x++)
				sim.doTick();

			double voltageHigh = resScope0.Max((f) => f.voltage);
			int voltageHighNdx = resScope0.FindIndex((f) => f.voltage == voltageHigh);

			TestUtils.Compare(voltageHigh, voltage0.dutyCycle, 4);
			TestUtils.Compare(resScope0[voltageHighNdx].time, quarterCycleTime, 4);

			double voltageLow = resScope0.Min((f) => f.voltage);
			int voltageLowNdx = resScope0.FindIndex((f) => f.voltage == voltageLow);

			TestUtils.Compare(voltageLow, -voltage0.dutyCycle, 4);
			TestUtils.Compare(resScope0[voltageLowNdx].time, quarterCycleTime * 3, 4);

			double currentHigh = resScope0.Max((f) => f.current);
			int currentHighNdx = resScope0.FindIndex((f) => f.current == currentHigh);
			Debug.Log(currentHigh, "currentHigh");

			double currentLow = resScope0.Min((f) => f.current);
			int currentLowNdx = resScope0.FindIndex((f) => f.current == currentLow);
			Debug.Log(Math.Round(currentLow, 4), "currentLow");
		}

		[Test]
		public void SquareWaveTest() {
			Circuit sim = new Circuit();

			var voltage0 = sim.Create<VoltageInput>(Voltage.WaveType.SQUARE);

			var res0 = sim.Create<Resistor>();
			var ground0 = sim.Create<Ground>();

			sim.Connect(voltage0.leadPos, res0.leadIn);
			sim.Connect(res0.leadOut, ground0.leadIn);

			var resScope0 = sim.Watch(res0);

			double cycleTime = 1 / voltage0.frequency;
			double quarterCycleTime = cycleTime / 4;

			int steps = (int)(cycleTime / sim.timeStep);
			for(int x = 1; x <= steps; x++)
				sim.doTick();

			double voltageHigh = resScope0.Max((f) => f.voltage);
			int voltageHighNdx = resScope0.FindIndex((f) => f.voltage == voltageHigh);

			Assert.AreEqual(voltageHigh, voltage0.dutyCycle);
			Assert.AreEqual(0, voltageHighNdx);

			double voltageLow = resScope0.Min((f) => f.voltage);
			int voltageLowNdx = resScope0.FindIndex((f) => f.voltage == voltageLow);

			Assert.AreEqual(voltageLow, -voltage0.dutyCycle);
			Assert.AreEqual(2501, voltageLowNdx);

			double currentHigh = resScope0.Max((f) => f.current);
			int currentHighNdx = resScope0.FindIndex((f) => f.current == currentHigh);
			Assert.AreEqual(voltageHigh / res0.resistance, currentHigh);

			double currentLow = resScope0.Min((f) => f.current);
			int currentLowNdx = resScope0.FindIndex((f) => f.current == currentLow);
			Assert.AreEqual(voltageLow / res0.resistance, currentLow);
		}

		[Test]
		public void TriangleWaveVoltageTest() {
			Assert.Ignore("Not Implemented!");
		}

		[Test]
		public void SawtoothWaveVoltageTest() {
			Assert.Ignore("Not Implemented!");
		}

		[Test]
		public void PulseWaveVoltageTest() {
			Assert.Ignore("Not Implemented!");
		}

		[Test]
		public void VarWaveVoltageTest() {
			Assert.Ignore("Not Implemented!");
		}

	}
}
