using System;
using System.Collections.Generic;
using System.Linq;

using SharpCircuit;
using NUnit.Framework;

namespace SharpCircuitTest {

	public class SimpleTest {

		[TestCase(0.01)]
		[TestCase(0.02)]
		[TestCase(0.04)]
		[TestCase(0.06)]
		[TestCase(0.08)]
		[TestCase(0.10)]
		public void CurrentElmTest(double current) {
			Circuit sim = new Circuit();

			var source0 = sim.Create<CurrentSourceElm>();
			source0.sourceCurrent = current;
			var res0 = sim.Create<ResistorElm>();

			sim.Connect(source0.leadOut, res0.leadIn);
			sim.Connect(res0.leadOut, source0.leadIn);

			var source1 = sim.Create<CurrentSourceElm>();
			source1.sourceCurrent = current;
			var res1 = sim.Create<ResistorElm>();

			sim.Connect(source1.leadOut, res1.leadOut);
			sim.Connect(res1.leadIn, source1.leadIn);

			sim.doTicks(100);

			Assert.AreEqual(current, res0.getCurrent());
			Assert.AreEqual(-current, res1.getCurrent());
			Assert.AreEqual(current * res0.resistance, res0.getVoltageDelta());
		}

		[TestCase(20)]
		[TestCase(40)]
		[TestCase(60)]
		[TestCase(80)]
		[TestCase(100)]
		[TestCase(120)]
		[TestCase(140)]
		[TestCase(160)]
		[TestCase(180)]
		public void ResistorElmTest(double resistance) {
			Circuit sim = new Circuit();

			var volt0 = sim.Create<DCVoltageElm>();
			var res0 = sim.Create<ResistorElm>();

			sim.Connect(volt0.leadPos, res0.leadOut);
			sim.Connect(res0.leadIn, volt0.leadNeg);

			for(int x = 1; x <= 100; x++) {
				sim.update();
				// Ohm's Law
				Assert.AreEqual(res0.getVoltageDelta(), res0.resistance * res0.getCurrent()); // V = I x R
				Assert.AreEqual(res0.getCurrent(), res0.getVoltageDelta() / res0.resistance); // I = V / R
				Assert.AreEqual(res0.resistance, res0.getVoltageDelta() / res0.getCurrent()); // R = V / I
			}
		}

		[TestCase(20)]
		[TestCase(40)]
		[TestCase(60)]
		[TestCase(80)]
		[TestCase(100)]
		[TestCase(120)]
		[TestCase(140)]
		[TestCase(160)]
		[TestCase(180)]
		public void LinearResistorElmTest(double resistance) {
			Circuit sim = new Circuit();

			var volt0 = sim.Create<VoltageInputElm>(VoltageElm.WaveType.DC);
			var res0 = sim.Create<ResistorElm>();
			var ground0 = sim.Create<GroundElm>();

			sim.Connect(volt0.leadVoltage, res0.leadIn);
			sim.Connect(res0.leadOut, ground0.leadIn);

			for(int x = 1; x <= 100; x++) {
				sim.update();
				// Ohm's Law
				Assert.AreEqual(res0.getVoltageDelta(), res0.resistance * res0.getCurrent()); // V = I x R
				Assert.AreEqual(res0.getCurrent(), res0.getVoltageDelta() / res0.resistance); // I = V / R
				Assert.AreEqual(res0.resistance, res0.getVoltageDelta() / res0.getCurrent()); // R = V / I
			}
		}

		[TestCase(1)]
		[TestCase(0.02)]
		[TestCase(0.4)]
		public void InductorElmTest(double inductance) {
			Circuit sim = new Circuit();

			var source0 = sim.Create<DCVoltageElm>();
			var inductor0 = sim.Create<InductorElm>(inductance);

			sim.Connect(source0.leadPos, inductor0.leadIn);
			sim.Connect(inductor0.leadOut, source0.leadNeg);

			double cycleTime = 1 / source0.frequency;
			double quarterCycleTime = cycleTime / 4;

			sim.doTicks((int)(cycleTime / sim.timeStep));

			double flux = inductor0.inductance * inductor0.getCurrent();	// F = I x L
			Debug.Log(inductor0.getCurrent(), flux / inductor0.inductance); // I = F / L
			Debug.Log(inductor0.inductance, flux / inductor0.getCurrent()); // L = F / I

			Assert.Ignore();
		}

		[TestCase(1)]
		[TestCase(0.02)]
		[TestCase(0.4)]
		public void LinearInductorElmTest(double inductance) {
			Circuit sim = new Circuit();

			var source0 = sim.Create<VoltageInputElm>(VoltageElm.WaveType.AC);
			var inductor0 = sim.Create<InductorElm>(inductance);
			var out0 = sim.Create<GroundElm>();

			sim.Connect(source0.leadVoltage, inductor0.leadIn);
			sim.Connect(inductor0.leadOut, out0.leadIn);

			double cycleTime = 1 / source0.frequency;
			double quarterCycleTime = cycleTime / 4;

			sim.doTicks((int)(cycleTime / sim.timeStep));

			Debug.Log(inductor0.getCurrent());
			Assert.Ignore();
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

			sim.doTicks(100);

			Assert.AreEqual(out0, logicOut.isHigh());
		}

	}
}
