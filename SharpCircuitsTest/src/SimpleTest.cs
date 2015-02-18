using System;
using System.Collections.Generic;
using System.Linq;

using SharpCircuit;
using NUnit.Framework;

namespace SharpCircuitTest {

	public class SimpleTest {

		[TestCase(10)]
		[TestCase(20)]
		[TestCase(30)]
		[TestCase(40)]
		[TestCase(50)]
		public void CurrentElmTest(double current) {
			Circuit sim = new Circuit();

			var source0 = sim.Create<CurrentElm>();
			var res0 = sim.Create<ResistorElm>();

			sim.Connect(source0.leadOut, res0.leadIn);
			sim.Connect(res0.leadOut, source0.leadIn);

			for(int x = 1; x <= 100; x++)
				sim.update(sim.timeStep);

			Assert.AreEqual(1, source0.getVoltageDiff());
			Assert.AreEqual(current, res0.getCurrent());

		}

		[Test]
		public void ACWaveVoltageTest() {
			Assert.Pass("Not Implemented!");
		}

		[Test]
		public void DCVoltageTest() {
			Assert.Pass("Not Implemented!");
		}

		[Test]
		public void SquareWaveVoltageTest() {
			Assert.Pass("Not Implemented!");
		}

		[Test]
		public void TriangleWaveVoltageTest() {
			Assert.Pass("Not Implemented!");
		}

		[Test]
		public void SawtoothWaveVoltageTest() {
			Assert.Pass("Not Implemented!");
		}

		[Test]
		public void PulseWaveVoltageTest() {
			Assert.Pass("Not Implemented!");
		}

		[Test]
		public void VarWaveVoltageTest() {
			Assert.Pass("Not Implemented!");
		}

		[TestCase(1, false)]
		[TestCase(0, true)]
		public void InverterElmTest(int in0, bool out0) {
			Circuit sim = new Circuit();

			var logicIn0 = sim.Create<LogicInputElm>();
			var invert0 = sim.Create<InverterElm>();
			var logicOut = sim.Create<LogicOutputElm>();

			sim.Connect(logicIn0.leadOut, invert0.leadIn);
			sim.Connect(invert0.leadOut, logicOut.leadIn);

			logicIn0.setPosition(in0);
			sim.analyze();

			int steps = 100;
			for(int x = 1; x <= steps; x++)
				sim.update(x);

			Assert.AreEqual(out0, logicOut.isHigh());
		}

		[TestCase(20)]
		[TestCase(100)]
		[TestCase(200)]
		public void ResistorElmTest(double resistance) {
			Circuit sim = new Circuit();

			var source0 = sim.Create<DCVoltageElm>();
			var res0 = sim.Create<ResistorElm>(resistance);

			sim.Connect(source0.leadNeg, res0.leadIn);
			sim.Connect(res0.leadOut, source0.leadNeg);

			for(int x = 1; x <= 100; x++)
				sim.update(sim.timeStep);

			Assert.AreEqual(source0.maxVoltage / resistance, res0.getCurrent());
		}

		[TestCase(20)]
		[TestCase(100)]
		[TestCase(200)]
		public void LinearResistorElmTest(double resistance) {
			Circuit sim = new Circuit();

			var source0 = sim.Create<RailElm>(VoltageElm.WaveType.DC);
			var res0 = sim.Create<ResistorElm>(resistance);
			var out0 = sim.Create<GroundElm>();

			sim.Connect(source0.leadOut, res0.leadIn);
			sim.Connect(res0.leadOut, out0.leadIn);

			for(int x = 1; x <= 100; x++)
				sim.update(sim.timeStep);

			Assert.AreEqual(source0.maxVoltage / resistance, res0.getCurrent());
		}

		[TestCase(1)]
		[TestCase(0.02)]
		[TestCase(0.4)]
		public void InductorElmTest(double inductance) {
			Circuit sim = new Circuit();

			var source0 = sim.Create<ACVoltageElm>();
			var inductor0 = sim.Create<InductorElm>(inductance);

			sim.Connect(source0.leadPos, inductor0.leadIn);
			sim.Connect(inductor0.leadOut, source0.leadNeg);

			double cycleTime = 1 / source0.frequency;
			double quarterCycleTime = cycleTime / 4;

			int steps = (int)(cycleTime / sim.timeStep);
			for(int x = 1; x <= 100; x++)
				sim.update(sim.timeStep);

			Debug.Log(inductor0.getCurrent());
			Assert.Ignore();
		}

		[TestCase(1)]
		[TestCase(0.02)]
		[TestCase(0.4)]
		public void LinearInductorElmTest(double inductance) {
			Circuit sim = new Circuit();

			var source0 = sim.Create<RailElm>(VoltageElm.WaveType.AC);
			var inductor0 = sim.Create<InductorElm>(inductance);
			var out0 = sim.Create<GroundElm>();

			sim.Connect(source0.leadOut, inductor0.leadIn);
			sim.Connect(inductor0.leadOut, out0.leadIn);

			double cycleTime = 1 / source0.frequency;
			double quarterCycleTime = cycleTime / 4;

			int steps = (int)(cycleTime / sim.timeStep);
			for(int x = 1; x <= 100; x++)
				sim.update(sim.timeStep);

			Debug.Log(inductor0.getCurrent());
			Assert.Ignore();
		}

	}
}
