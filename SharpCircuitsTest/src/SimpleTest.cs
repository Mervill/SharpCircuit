using System;
using System.Collections.Generic;
using System.Linq;

using SharpCircuit;
using NUnit.Framework;

namespace SharpCircuitTest {

	public class SimpleTest {

		//Assert.That(1.0, Is.EqualTo(1.5).Within(0.5f));

		[Test]
		public void AnalogSwitchTest() {
			Circuit sim = new Circuit();

			var logicIn0 = sim.Create<LogicInput>();
			var logicIn1 = sim.Create<LogicInput>();

			var analogSwitch0 = sim.Create<AnalogSwitch>();

			var grnd = sim.Create<Ground>();

			sim.Connect(logicIn0.leadOut, analogSwitch0.leadIn);
			sim.Connect(logicIn1.leadOut, analogSwitch0.leadSwitch);
			sim.Connect(analogSwitch0.leadOut, grnd.leadIn);

			logicIn0.toggle();
			logicIn1.toggle();

			sim.doTicks(100);
			Assert.AreEqual(0.25, Math.Round(grnd.getCurrent(), 12));

			logicIn1.toggle();
			sim.analyze();
			sim.doTicks(100);
			Assert.AreEqual(5E-10, Math.Round(grnd.getCurrent(), 12));
		}

		[TestCase(0.01)]
		[TestCase(0.02)]
		[TestCase(0.04)]
		[TestCase(0.06)]
		[TestCase(0.08)]
		[TestCase(0.10)]
		public void CurrentSourceTest(double current) {
			Circuit sim = new Circuit();

			var source0 = sim.Create<CurrentSource>();
			source0.sourceCurrent = current;
			var res0 = sim.Create<Resistor>();

			sim.Connect(source0.leadOut, res0.leadIn);
			sim.Connect(res0.leadOut, source0.leadIn);

			var source1 = sim.Create<CurrentSource>();
			source1.sourceCurrent = current;
			var res1 = sim.Create<Resistor>();

			sim.Connect(source1.leadOut, res1.leadOut);
			sim.Connect(res1.leadIn, source1.leadIn);

			sim.doTicks(100);

			Assert.AreEqual(current, res0.getCurrent());
			Assert.AreEqual(-current, res1.getCurrent());
			Assert.AreEqual(current * res0.resistance, res0.getVoltageDelta());
		}

		[TestCase( 20)]
		[TestCase( 40)]
		[TestCase( 60)]
		[TestCase( 80)]
		[TestCase(100)]
		[TestCase(120)]
		[TestCase(140)]
		[TestCase(160)]
		[TestCase(180)]
		public void ResistorTest(double resistance) {
			Circuit sim = new Circuit();

			var volt0 = sim.Create<DCVoltageSource>();
			var res0 = sim.Create<Resistor>();
			
			sim.Connect(volt0.leadPos, res0.leadOut);
			sim.Connect(res0.leadIn, volt0.leadNeg);

			for(int x = 1; x <= 100; x++) {
				sim.doTick();
				// Ohm's Law
				Assert.AreEqual(res0.getVoltageDelta(), res0.resistance * res0.getCurrent()); // V = I x R
				Assert.AreEqual(res0.getCurrent(), res0.getVoltageDelta() / res0.resistance); // I = V / R
				Assert.AreEqual(res0.resistance, res0.getVoltageDelta() / res0.getCurrent()); // R = V / I
			}
		}

		[TestCase( 20)]
		[TestCase( 40)]
		[TestCase( 60)]
		[TestCase( 80)]
		[TestCase(100)]
		[TestCase(120)]
		[TestCase(140)]
		[TestCase(160)]
		[TestCase(180)]
		public void LinearResistorTest(double resistance) {
			Circuit sim = new Circuit();

			var volt0 = sim.Create<VoltageInput>(Voltage.WaveType.DC);
			var res0 = sim.Create<Resistor>();
			var ground0 = sim.Create<Ground>();

			sim.Connect(volt0.leadPos, res0.leadIn);
			sim.Connect(res0.leadOut, ground0.leadIn);

			for(int x = 1; x <= 100; x++) {
				sim.doTick();
				// Ohm's Law
				Assert.AreEqual(res0.getVoltageDelta(), res0.resistance * res0.getCurrent()); // V = I x R
				Assert.AreEqual(res0.getCurrent(), res0.getVoltageDelta() / res0.resistance); // I = V / R
				Assert.AreEqual(res0.resistance, res0.getVoltageDelta() / res0.getCurrent()); // R = V / I
			}
		}

		[TestCase(1)]
		[TestCase(0.02)]
		[TestCase(0.4)]
		public void InductorTest(double inductance) {
			Circuit sim = new Circuit();

			var source0 = sim.Create<DCVoltageSource>();
			var inductor0 = sim.Create<InductorElm>(inductance);

			sim.Connect(source0.leadPos, inductor0.leadIn);
			sim.Connect(inductor0.leadOut, source0.leadNeg);

			double cycleTime = 1 / source0.frequency;
			double quarterCycleTime = cycleTime / 4;

			sim.doTicks((int)(cycleTime / sim.timeStep));

			double flux = inductor0.inductance * inductor0.getCurrent();	// F = I x L
			Assert.AreEqual(inductor0.getCurrent(), flux / inductor0.inductance); // I = F / L
			Assert.AreEqual(inductor0.inductance, flux / inductor0.getCurrent()); // L = F / I
		}

		[TestCase(1   )]
		[TestCase(0.02)]
		[TestCase(0.4 )]
		public void LinearInductorTest(double inductance) {
			Circuit sim = new Circuit();

			var source0 = sim.Create<VoltageInput>(Voltage.WaveType.AC);
			var inductor0 = sim.Create<InductorElm>(inductance);
			var out0 = sim.Create<Ground>();

			sim.Connect(source0.leadPos, inductor0.leadIn);
			sim.Connect(inductor0.leadOut, out0.leadIn);

			double cycleTime = 1 / source0.frequency;
			double quarterCycleTime = cycleTime / 4;

			sim.doTicks((int)(cycleTime / sim.timeStep));

			Debug.Log(inductor0.getCurrent());
			Assert.Ignore();
		}

		[TestCase(true )]
		[TestCase(false)]
		public void SwitchSPSTTest(bool in0) {
			Circuit sim = new Circuit();

			var volt0 = sim.Create<VoltageInput>();
			var switch0 = sim.Create<SwitchSPST>();
			var res0 = sim.Create<Resistor>();
			var logicOut0 = sim.Create<LogicOutput>();

			sim.Connect(volt0, 0, switch0, 0);
			sim.Connect(switch0, 1, res0, 0);
			sim.Connect(res0, 1, logicOut0, 0);

			var volt1 = sim.Create<VoltageInput>();
			var switch1 = sim.Create<SwitchSPST>();
			var res1 = sim.Create<Resistor>();
			var grnd1 = sim.Create<Ground>();

			sim.Connect(volt1, 0, switch1, 0);
			sim.Connect(switch1, 1, res1, 0);
			sim.Connect(res1, 1, grnd1, 0);

			if(in0) {
				switch0.toggle();
				switch1.toggle();
			}

			sim.doTicks(100);

			Debug.Log(logicOut0.getVoltageDelta(), logicOut0.getCurrent());
			Debug.Log(grnd1.getVoltageDelta(), grnd1.getCurrent());
			Assert.Ignore();
		}

		[TestCase(1, false)]
		[TestCase(0, true )]
		public void InverterTest(int in0, bool out0) {
			Circuit sim = new Circuit();

			var logicIn0 = sim.Create<LogicInput>();
			var invert0 = sim.Create<Inverter>();
			var logicOut = sim.Create<LogicOutput>();

			sim.Connect(logicIn0.leadOut, invert0.leadIn);
			sim.Connect(invert0.leadOut, logicOut.leadIn);

			logicIn0.setPosition(in0);
			sim.analyze();

			sim.doTicks(100);

			Assert.AreEqual(out0, logicOut.isHigh());
		}

		/*[TestCase(true)]
		[TestCase(false)]
		public void LogicInputOutputTest(bool in0) {
			Circuit sim = new Circuit();

			var logicIn0 = sim.Create<LogicInput>();
			var logicOut0 = sim.Create<LogicOutput>();

			Assert.Ignore();
		}*/

		[TestCase(0.005)]
		[TestCase(0.200)]
		[TestCase(0.400)]
		[TestCase(0.5  )]
		[TestCase(0.600)]
		[TestCase(0.800)]
		[TestCase(0.995)]
		public void PotentiometerTest(double in0) {
			Circuit sim = new Circuit();

			var volt0 = sim.Create<LogicInput>();
			volt0.toggle();

			var potent0 = sim.Create<Potentiometer>();
			potent0.position = in0; // 0.995

			var grnd0 = sim.Create<Ground>();
			var grnd1 = sim.Create<Ground>();

			sim.Connect(potent0.leadVoltage, volt0.leadOut);
			sim.Connect(potent0.leadIn, grnd0.leadIn);
			sim.Connect(potent0.leadOut, grnd1.leadIn);

			sim.doTicks(100);

			Debug.Log("grnd0",  grnd0.getCurrent());
			Debug.Log("grnd1", grnd1.getCurrent());
			Debug.Log(grnd0.getCurrent() + grnd1.getCurrent());
			Assert.Ignore();
		}

		[Test]
		public void SiliconRectifierTest() {
			Circuit sim = new Circuit();

			var volt0 = sim.Create<VoltageInput>();
			var volt1 = sim.Create<VoltageInput>();

			var res0 = sim.Create<Resistor>();
			
			var scr0 = sim.Create<SiliconRectifier>();

			var grnd0 = sim.Create<Ground>();

			sim.Connect(volt0.leadPos, scr0.leadIn);

			sim.Connect(volt1.leadPos, res0.leadIn);
			sim.Connect(res0.leadOut, scr0.leadGate);
			sim.Connect(scr0.leadOut, grnd0.leadIn);

			sim.doTicks(1000);

			Debug.Log(sim.time);
			Debug.Log("in", scr0.getLeadVoltage(0));
			Debug.Log("out", scr0.getLeadVoltage(1));
			Debug.Log("gate", scr0.getLeadVoltage(2));

			Debug.Log();
			foreach(string s in scr0.getInfo()) {
				Debug.Log(s);
			}

			Assert.Ignore();
		}

		[Test]
		public void TriodeTest() {
			Circuit sim = new Circuit();

			var volt0 = sim.Create<VoltageInput>();
			volt0.maxVoltage = 500;
			var triode0 = sim.Create<Triode>();
			var grnd0 = sim.Create<Ground>();

			sim.Connect(volt0.leadPos, triode0.leadPlate);
			sim.Connect(triode0.leadCath, grnd0.leadIn);

			sim.doTicks(100);

			Assert.AreEqual(0.018332499042, Math.Round(volt0.getCurrent(), 12));
		}

		[TestCase(0, 0, false)]
		[TestCase(1, 0, false)]
		[TestCase(0, 1, false)]
		[TestCase(1, 1, true )]
		public void TriStateBufferTest(int in0, int in1, bool in3) {
			Circuit sim = new Circuit();

			var logicIn0 = sim.Create<LogicInput>();
			var logicIn1 = sim.Create<LogicInput>();
			var logicOut0 = sim.Create<LogicOutput>();
			var tri0 = sim.Create<TriStateBuffer>();

			sim.Connect(logicIn0.leadOut, tri0.leadIn);
			sim.Connect(logicIn1.leadOut, tri0.leadGate);
			sim.Connect(logicOut0.leadIn, tri0.leadOut);

			logicIn0.setPosition(in0);
			logicIn1.setPosition(in1);

			sim.doTicks(100);

			Assert.AreEqual(in3, logicOut0.isHigh());
		}

	}
}
