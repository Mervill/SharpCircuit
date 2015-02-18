using System;
using System.Collections.Generic;
using System.Linq;

using SharpCircuit;
using NUnit.Framework;

namespace SharpCircuitTest {

	[TestFixture]
	public class ACVoltageTest {

		[TestCase(20)]
		[TestCase(24)]
		[TestCase(26)]
		[TestCase(28)]
		[TestCase(40)]
		[TestCase(42)]
		[TestCase(44)]
		[TestCase(46)]
		[TestCase(48)]
		[TestCase(60)]
		public void SimpleACVoltageTest(double frequency) {
			Circuit sim = new Circuit();

			var voltage0 = sim.Create<ACRailElm>();
			voltage0.frequency = frequency;

			var resistor = sim.Create<ResistorElm>();
			var ground = sim.Create<GroundElm>();

			sim.Connect(voltage0.leadOut, resistor.leadIn);
			sim.Connect(resistor.leadOut, ground.leadIn);

			var voltScope = sim.Watch(voltage0);

			// Cycle time is:
			//  1s
			// ---
			// (X)Hz
			double cycleTime = 1 / voltage0.frequency;

			// Peak values of the wave occur on 
			// quarter values of the cycle time
			double quarterCycleTime = cycleTime / 4;

			int steps = (int)(cycleTime / sim.timeStep);
			for(int x = 1; x <= steps; x++)
				sim.update(sim.timeStep);

			double voltageHigh = voltScope.Max((f) => f.voltage);
			int voltageHighNdx = voltScope.FindIndex((f) => f.voltage == voltageHigh);

			TestUtils.Compare(voltageHigh, voltage0.dutyCycle, 4);
			TestUtils.Compare(voltScope[voltageHighNdx].time, quarterCycleTime, 4); // Max voltage is reached by 1/4 of a cycle

			double voltageLow = voltScope.Min((f) => f.voltage);
			int voltageLowNdx = voltScope.FindIndex((f) => f.voltage == voltageLow);

			TestUtils.Compare(voltageLow, -voltage0.dutyCycle, 4);
			TestUtils.Compare(voltScope[voltageLowNdx].time, quarterCycleTime * 3, 4); // Min voltage is reached by 3/4 of a cycle

			double currentHigh = voltScope.Max((f) => f.current);
			int currentHighNdx = voltScope.FindIndex((f) => f.current == currentHigh);
			Debug.Log(currentHigh, "currentHigh");

			double currentLow = voltScope.Min((f) => f.current);
			int currentLowNdx = voltScope.FindIndex((f) => f.current == currentLow);
			Debug.Log(Math.Round(currentLow, 4), "currentLow");
		}

		[TestCase(8E-5, 0.02478541)]
		[TestCase(1E-5, 0.01772441)]
		[TestCase(1E-6, 0.00250067)]
		public void CapacitorCapacitanceTest(double capacitance, double current) {
			Circuit sim = new Circuit();

			VoltageElm source0 = sim.Create<VoltageElm>(VoltageElm.WaveType.AC);
			source0.frequency = 80;

			ResistorElm resistor0 = sim.Create<ResistorElm>(200);
			CapacitorElm cap0 = sim.Create<CapacitorElm>(capacitance);

			sim.Connect(source0, 1, resistor0, 0);
			sim.Connect(resistor0, 1, cap0, 0);
			sim.Connect(cap0, 1, source0, 0);

			var capScope = sim.Watch(cap0);

			double cycleTime = 1 / source0.frequency;
			double quarterCycleTime = cycleTime / 4;

			int steps = (int)(cycleTime / sim.timeStep);
			for(int x = 1; x <= steps; x++)
				sim.update(sim.timeStep);

			Assert.AreEqual(current, Math.Round(capScope.Max((f) => f.current), 8));
		}

		[TestCase(15, 0.01230581)]
		[TestCase(40, 0.02083503)]
		[TestCase(80, 0.02372956)]
		public void CapacitorFrequencyTest(double frequency, double current) {
			Circuit sim = new Circuit();

			VoltageElm source0 = sim.Create<VoltageElm>(VoltageElm.WaveType.AC);
			source0.frequency = frequency;

			ResistorElm resistor0 = sim.Create<ResistorElm>(200);
			CapacitorElm cap0 = sim.Create<CapacitorElm>(3E-5);
			
			sim.Connect(source0, 1, resistor0, 0);
			sim.Connect(resistor0, 1, cap0, 0);
			sim.Connect(cap0, 1, source0, 0);

			var capScope = sim.Watch(cap0);

			double cycleTime = 1 / source0.frequency;
			double quarterCycleTime = cycleTime / 4;

			int steps = (int)(cycleTime / sim.timeStep);
			for(int x = 1; x <= steps; x++)
				sim.update(sim.timeStep);

			Assert.AreEqual(current, Math.Round(capScope.Max((f) => f.current), 8));
		}

		[Test]
		public void InductorInductanceTest(double inductance) {
			Assert.Fail();
		}

		[Test]
		public void InductorFrequencyTest(double frequency) {
			Assert.Fail();
		}

		[Test]
		public void SeriesResonanceTest(double frequency) {
			Assert.Fail();
		}

		[Test]
		public void ParallelResonanceTest(double frequency) {
			Assert.Fail();
		}

	}
}
