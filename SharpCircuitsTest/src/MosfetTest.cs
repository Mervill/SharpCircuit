using System;
using System.Collections.Generic;
using System.Linq;

using SharpCircuit;
using NUnit.Framework;

namespace SharpCircuitTest {

	[TestFixture]
	public class MosfetTest {

		[Test]
		public void NMosfetTest() {
			Circuit sim = new Circuit();

			var volt0 = sim.Create<VoltageInputElm>(VoltageElm.WaveType.DC);
			volt0.maxVoltage = 3.5;

			var volt1 = sim.Create<VoltageInputElm>(VoltageElm.WaveType.DC);
			volt1.maxVoltage = 5;

			var nmosf0 = sim.Create<NMosfetElm>();

			var grnd0 = sim.Create<GroundElm>();

			sim.Connect(volt0.leadVoltage, nmosf0.leadGate);
			sim.Connect(volt1.leadVoltage, nmosf0.leadDrain);
			sim.Connect(grnd0.leadIn, nmosf0.leadSrc);

			sim.doTicks(100);

			Debug.Log("n-Vds", nmosf0.getVoltageDelta(), nmosf0.getState());
			Assert.AreEqual(0.04, Math.Round(volt1.getCurrent(), 6));
		}

		[Test]
		public void PMosfetTest() {
			Circuit sim = new Circuit();

			var volt0 = sim.Create<VoltageInputElm>(VoltageElm.WaveType.DC);
			volt0.maxVoltage = 2.5;

			var volt1 = sim.Create<VoltageInputElm>(VoltageElm.WaveType.DC);
			volt1.maxVoltage = 5;

			var volt2 = sim.Create<VoltageInputElm>(VoltageElm.WaveType.DC);
			volt2.maxVoltage = 3;

			var pmosf0 = sim.Create<PMosfetElm>();

			sim.Connect(volt0.leadVoltage, pmosf0.leadGate);
			sim.Connect(volt1.leadVoltage, pmosf0.leadSrc);
			sim.Connect(volt2.leadVoltage, pmosf0.leadDrain);

			sim.doTicks(100);

			Debug.Log("p-Vsd", pmosf0.getVoltageDelta(), pmosf0.getState());
			Assert.AreEqual(0.01, Math.Round(volt1.getCurrent(), 6));
		}

		[TestCase(true,  5E-08)]
		[TestCase(false, 0.01588403349)]
		public void SwitchTest(bool in0, double out0) {
			Circuit sim = new Circuit();

			var volt0 = sim.Create<VoltageInputElm>(VoltageElm.WaveType.DC);
			var switch0 = sim.Create<SwitchElm>();
			var res0 = sim.Create<ResistorElm>(300);
			var nmosf0 = sim.Create<NMosfetElm>();
			var grnd0 = sim.Create<GroundElm>();

			sim.Connect(volt0, 0, switch0, 0);
			sim.Connect(volt0, 0, res0, 0);
			sim.Connect(nmosf0, 0, switch0, 1);
			sim.Connect(nmosf0.leadDrain, res0.leadOut);
			sim.Connect(nmosf0.leadSrc, grnd0.leadIn);

			if(in0) switch0.toggle();

			sim.doTicks(100);

			Assert.AreEqual(out0, Math.Round(grnd0.getCurrent(), 12));
		}

		//SourceFollowerTest
		//CurrentSourceTest
		//CurrentRampTest
		//CurrentMirrorTest
		//CommonSourceAmplifierTest

		[TestCase(true,  false)]
		[TestCase(false, true)]
		public void CMOSInverterTest(bool in0, bool out0) {
			Circuit sim = new Circuit();

			var logicIn0 = sim.Create<LogicInputElm>();
			var logicOut0 = sim.Create<LogicOutputElm>();

			var volt0 = sim.Create<VoltageInputElm>(VoltageElm.WaveType.DC);
			var grnd0 = sim.Create<GroundElm>();

			var pmosf0 = sim.Create<PMosfetElm>();
			var nmosf0 = sim.Create<NMosfetElm>();

			sim.Connect(logicIn0.leadOut, pmosf0.leadGate);
			sim.Connect(logicIn0.leadOut, nmosf0.leadGate);

			sim.Connect(pmosf0.leadSrc, volt0.leadVoltage);
			sim.Connect(pmosf0.leadDrain, nmosf0.leadDrain);
			sim.Connect(nmosf0.leadSrc, grnd0.leadIn);

			sim.Connect(logicOut0.leadIn, pmosf0.leadDrain);

			if(in0) logicIn0.toggle();

			sim.doTicks(1000);

			Assert.AreEqual(out0, logicOut0.isHigh());
		}

		[Test]
		public void CMOSInverterCapacitanceTest() {
			Assert.Ignore();
		}

		[Test]
		public void CMOSInverterTransitionTest() {
			Assert.Ignore();
		}

		[TestCase(true,  4.316625)]
		[TestCase(false, 1E-05)]
		public void CMOSTransmissionGateTest(bool in0, double out0) {
			Circuit sim = new Circuit();

			var volt0 = sim.Create<VoltageInputElm>(VoltageElm.WaveType.AC);
			volt0.maxVoltage = 2.5;
			volt0.bias = 2.5;

			var logicIn0 = sim.Create<LogicInputElm>();
			var invert0 = sim.Create<InverterElm>();

			var pmosf0 = sim.Create<PMosfetElm>();
			var nmosf0 = sim.Create<NMosfetElm>();

			var res0 = sim.Create<ResistorElm>();
			var grnd0 = sim.Create<GroundElm>();

			sim.Connect(volt0.leadVoltage, pmosf0.leadDrain);

			sim.Connect(pmosf0.leadDrain, nmosf0.leadSrc);
			sim.Connect(pmosf0.leadSrc, nmosf0.leadDrain);

			sim.Connect(logicIn0.leadOut, invert0.leadIn);
			sim.Connect(invert0.leadOut, pmosf0.leadGate);
			sim.Connect(logicIn0.leadOut, nmosf0.leadGate);

			sim.Connect(pmosf0.leadSrc, res0.leadIn);
			sim.Connect(res0.leadOut, grnd0.leadIn);

			var res0Scope = sim.Watch(res0);

			if(in0) logicIn0.toggle();

			double cycleTime = 1 / volt0.frequency;
			double quarterCycleTime = cycleTime / 4;
			int steps = (int)(cycleTime / sim.timeStep);
			sim.doTicks(steps);

			Assert.AreEqual(out0, Math.Round(res0Scope.Max((e) => e.voltage), 6));
		}

		[Test]
		public void CMOSMultiplexerTest() {
			Assert.Ignore();
		}

		[Test]
		public void SampleAndHoldTest() {
			Assert.Ignore();
		}

		//DelayedBufferTest

		[Test]
		public void LeadingEdgeDetectorTest(){
			Assert.Ignore();
		}

		[Test]
		public void SwitchableFilterTest() {
			Assert.Ignore();
		}

		//Voltage Inverter
		//Inverter Amplifier
		//Inverter Oscillator
	}
}
