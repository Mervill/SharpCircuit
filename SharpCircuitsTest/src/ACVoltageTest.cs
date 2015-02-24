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

			var voltage0 = sim.Create<VoltageInput>(Voltage.WaveType.AC);
			voltage0.frequency = frequency;

			var resistor = sim.Create<Resistor>();
			var ground = sim.Create<Ground>();

			sim.Connect(voltage0.leadPos, resistor.leadIn);
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
				sim.doTick();

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

		[TestCase(8E-5, 0.022253913066)]
		[TestCase(1E-5, 0.012722352752)]
		[TestCase(1E-6, 0.002487492691)]
		public void CapacitorCapacitanceTest(double capacitance, double current) {
			Circuit sim = new Circuit();

			var source0 = sim.Create<Voltage>(Voltage.WaveType.AC);
			source0.frequency = 80;

			var resistor0 = sim.Create<Resistor>(200);
			var cap0 = sim.Create<CapacitorElm>(capacitance);

			sim.Connect(source0, 1, resistor0, 0);
			sim.Connect(resistor0, 1, cap0, 0);
			sim.Connect(cap0, 1, source0, 0);

			var capScope = sim.Watch(cap0);

			double cycleTime = 1 / source0.frequency;
			double quarterCycleTime = cycleTime / 4;

			int steps = (int)(cycleTime / sim.timeStep);
			for(int x = 1; x <= steps; x++)
				sim.doTick();

			double charge = cap0.capacitance * cap0.getVoltageDelta();

			Debug.Log(charge); // F = I x L
			Debug.Log(cap0.getCurrent(), charge / cap0.capacitance); // I = F / L
			Debug.Log(cap0.capacitance, charge / cap0.getCurrent()); // L = F / I
			
			Assert.AreEqual(current, Math.Round(capScope.Max((f) => f.current), 12));
		}

		[TestCase(15, 0.010707831103)]
		[TestCase(40, 0.015182310901)]
		[TestCase(80, 0.018851527487)]
		public void CapacitorFrequencyTest(double frequency, double current) {
			Circuit sim = new Circuit();

			var source0 = sim.Create<Voltage>(Voltage.WaveType.AC);
			source0.frequency = frequency;

			var resistor0 = sim.Create<Resistor>(200);
			var cap0 = sim.Create<CapacitorElm>(3E-5);
			
			sim.Connect(source0, 1, resistor0, 0);
			sim.Connect(resistor0, 1, cap0, 0);
			sim.Connect(cap0, 1, source0, 0);

			var capScope = sim.Watch(cap0);

			double cycleTime = 1 / source0.frequency;
			double quarterCycleTime = cycleTime / 4;

			int steps = (int)(cycleTime / sim.timeStep);
			for(int x = 1; x <= steps; x++)
				sim.doTick();

			Assert.AreEqual(current, Math.Round(capScope.Max((f) => f.current), 12));
		}

		[TestCase(1,    0.008321482318)]
		[TestCase(0.02, 0.049487068655)]
		[TestCase(0.4,  0.016742709632)]
		public void InductorInductanceTest(double inductance, double current) {
			Circuit sim = new Circuit();

			var source0 = sim.Create<Voltage>(Voltage.WaveType.AC);
			source0.frequency = 80;
			source0.phaseShift = 90;

			var resistor0 = sim.Create<Resistor>(100);
			var induct0 = sim.Create<InductorElm>(inductance);

			sim.Connect(source0, 1, resistor0, 0);
			sim.Connect(resistor0, 1, induct0, 0);
			sim.Connect(induct0, 1, source0, 0);

			var inductScope = sim.Watch(induct0);

			double cycleTime = 1 / source0.frequency;
			double quarterCycleTime = cycleTime / 4;

			int steps = (int)(cycleTime / sim.timeStep);
			for(int x = 1; x <= steps; x++)
				sim.doTick();
			
			Assert.AreEqual(current, Math.Round(inductScope.Max((f) => f.current), 12));
		}

		[TestCase(40, 0.025321400958)]
		[TestCase(80, 0.016742709632)]
		[TestCase(200, 0.008335618187)]
		public void InductorFrequencyTest(double frequency, double current) {
			Circuit sim = new Circuit();

			var source0 = sim.Create<Voltage>(Voltage.WaveType.AC);
			source0.frequency = frequency;
			source0.phaseShift = 90;

			var resistor0 = sim.Create<Resistor>(100);
			var induct0 = sim.Create<InductorElm>(0.4);

			sim.Connect(source0, 1, resistor0, 0);
			sim.Connect(resistor0, 1, induct0, 0);
			sim.Connect(induct0, 1, source0, 0);

			var inductScope = sim.Watch(induct0);

			double cycleTime = 1 / source0.frequency;
			double quarterCycleTime = cycleTime / 4;

			int steps = (int)(cycleTime / sim.timeStep);
			for(int x = 1; x <= steps; x++)
				sim.doTick();

			Assert.AreEqual(current, Math.Round(inductScope.Max((f) => f.current), 12));
		}

		[Test]
		public void SeriesResonanceTest() {
			Assert.Ignore("Not Implemented!");
		}

		[Test]
		public void ParallelResonanceTest() {
			Assert.Ignore("Not Implemented!");
		}

	}
}
