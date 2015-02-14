using System;
using System.Collections.Generic;
using System.Linq;

using SharpCircuit;
using NUnit.Framework;

namespace SharpCircuitTest {

	[TestFixture]
	public class ACVoltageTest {

		[Test]
		public void OhmsLawTest() {
			Assert.Fail();
		}

		[TestCase(8E-5, 0.02427305)]
		[TestCase(1E-5, 0.0177244 )]
		[TestCase(1E-6, 0.00250066)]
		public void CapacitorCapacitanceTest(double capacitance, double current) {
			CirSim sim = new CirSim();

			VoltageElm source0 = sim.Create<VoltageElm>(VoltageElm.WaveType.AC);
			source0.frequency = 80;

			ResistorElm resistor0 = sim.Create<ResistorElm>(200);
			CapacitorElm cap0 = sim.Create<CapacitorElm>(capacitance);

			sim.Connect(source0, 1, resistor0, 0);
			sim.Connect(resistor0, 1, cap0, 0);
			sim.Connect(cap0, 1, source0, 0);

			var capScope = sim.Watch(cap0);

			int steps = 4000;
			for(int x = 1; x <= steps; x++)
				sim.update(x);

			Assert.AreEqual(0.04, Math.Round(sim.time, 4));
			Assert.AreEqual(current, Math.Round(capScope.Max((f) => f.current), 8));
		}

		[TestCase(15, 0.00923844)]
		[TestCase(40, 0.02074225)]
		[TestCase(80, 0.02372003)]
		public void CapacitorFrequencyTest(double frequency, double current) {
			CirSim sim = new CirSim();

			VoltageElm source0 = sim.Create<VoltageElm>(VoltageElm.WaveType.AC);
			source0.frequency = frequency;

			ResistorElm resistor0 = sim.Create<ResistorElm>(200);
			CapacitorElm cap0 = sim.Create<CapacitorElm>(3E-5);
			
			sim.Connect(source0, 1, resistor0, 0);
			sim.Connect(resistor0, 1, cap0, 0);
			sim.Connect(cap0, 1, source0, 0);

			var capScope = sim.Watch(cap0);

			int steps = 4000;
			for(int x = 1; x <= steps; x++)
				sim.update(x);

			Assert.AreEqual(0.04, Math.Round(sim.time, 4));
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
