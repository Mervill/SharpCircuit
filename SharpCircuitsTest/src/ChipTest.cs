using System;
using System.Collections.Generic;
using System.Linq;

using SharpCircuit;
using NUnit.Framework;

namespace SharpCircuitTest {

	[TestFixture]
	public class ChipTest {

		[Test]
		public void CounterTest() {
			Circuit sim = new Circuit();

			var logicIn0 = sim.Create<LogicInput>();
			var logicIn1 = sim.Create<LogicInput>();

			var counter0 = sim.Create<CounterElm>();
		}

		[TestCase(0, 0, 0)]
		[TestCase(1, 0, 1)]
		[TestCase(0, 1, 1)]
		[TestCase(1, 1, 2)]
		public void HalfAdderTest(int in0, int in1, int i0) {
			Circuit sim = new Circuit();

			var logicIn0 = sim.Create<LogicInput>();
			var logicIn1 = sim.Create<LogicInput>();

			var chip0 = sim.Create<HalfAdder>();

			var logicOut0 = sim.Create<LogicOutput>();
			var logicOut1 = sim.Create<LogicOutput>();

			sim.Connect(logicIn0.leadOut, chip0.leadIn0);
			sim.Connect(logicIn1.leadOut, chip0.leadIn1);

			sim.Connect(logicOut0.leadIn, chip0.leadOut1);
			sim.Connect(logicOut1.leadIn, chip0.leadOut2);
			
			logicIn0.setPosition(in0);
			logicIn1.setPosition(in1);
			sim.analyze();

			for(int x = 1; x <= 100; x++)
				sim.doTick();

			int i = 0;
			if(logicOut0.isHigh()) i += 2;
			if(logicOut1.isHigh()) i += 1;
			
			Assert.AreEqual(i0, i);
		}

		[TestCase(0, 0, 0, 0)]
		[TestCase(1, 0, 0, 1)]
		[TestCase(0, 1, 0, 1)]
		[TestCase(0, 0, 1, 1)]
		[TestCase(1, 1, 0, 2)]
		[TestCase(1, 0, 1, 2)]
		[TestCase(0, 1, 1, 2)]
		[TestCase(1, 1, 1, 3)]
		public void FullAdderTest(int in0, int in1, int in2, int i0) {
			Circuit sim = new Circuit();

			var logicIn0 = sim.Create<LogicInput>();
			var logicIn1 = sim.Create<LogicInput>();
			var logicIn2 = sim.Create<LogicInput>();

			var chip0 = sim.Create<FullAdder>();

			var logicOut0 = sim.Create<LogicOutput>();
			var logicOut1 = sim.Create<LogicOutput>();

			sim.Connect(logicIn0.leadOut, chip0.leadIn0);
			sim.Connect(logicIn1.leadOut, chip0.leadIn1);
			sim.Connect(logicIn2.leadOut, chip0.leadIn2);

			sim.Connect(logicOut0.leadIn, chip0.leadOut1);
			sim.Connect(logicOut1.leadIn, chip0.leadOut2);

			logicIn0.setPosition(in0);
			logicIn1.setPosition(in1);
			logicIn2.setPosition(in2);
			sim.analyze();

			for(int x = 1; x <= 100; x++)
				sim.doTick();

			int i = 0;
			if(logicOut0.isHigh()) i += 1;
			if(logicOut1.isHigh()) i += 2;
			
			Assert.AreEqual(i0, i);
		}

		[Test]
		public void DFlipFlopTest() {
			Circuit sim = new Circuit();

			var logicIn0 = sim.Create<LogicInput>();
			var logicIn1 = sim.Create<LogicInput>();

			var dflip0 = sim.Create<DFlipFlop>();

			var logicOut0 = sim.Create<LogicOutput>();
			var logicOut1 = sim.Create<LogicOutput>();

			sim.Connect(logicIn0.leadOut, dflip0.leadD);
			sim.Connect(logicIn1.leadOut, dflip0.leadCLK);

			sim.Connect(logicOut0.leadIn, dflip0.leadQ);
			sim.Connect(logicOut0.leadIn, dflip0.leadQL);

			sim.doTicks(200);

			Debug.Log();
			Debug.Log("D  ", dflip0.getLeadVoltage(0));
			Debug.Log("CLK", dflip0.getLeadVoltage(3));
			Debug.Log(" Q ", dflip0.getLeadVoltage(1));
			Debug.Log("|Q ", dflip0.getLeadVoltage(2));
			Debug.Log();

			logicIn0.toggle();
			logicIn1.toggle();
			sim.analyze();
			sim.doTicks(200);

			Debug.Log();
			Debug.Log("D  ", dflip0.getLeadVoltage(0));
			Debug.Log("CLK", dflip0.getLeadVoltage(3));
			Debug.Log(" Q ", dflip0.getLeadVoltage(1));
			Debug.Log("|Q ", dflip0.getLeadVoltage(2));
			Debug.Log();

			logicIn0.toggle();
			logicIn1.toggle();
			sim.analyze();
			sim.doTicks(200);

			Debug.Log();
			Debug.Log("D  ", dflip0.getLeadVoltage(0));
			Debug.Log("CLK", dflip0.getLeadVoltage(3));
			Debug.Log(" Q ", dflip0.getLeadVoltage(1));
			Debug.Log("|Q ", dflip0.getLeadVoltage(2));
			Debug.Log();

			Assert.Ignore();
		}

		[Test]
		public void JKlipFlopTest() {
			Assert.Ignore("Not Implemented!");
		}

		[Test]
		public void TFlipFlopTest() {
			Circuit sim = new Circuit();

			var logicIn0 = sim.Create<LogicInput>();
			var logicIn1 = sim.Create<LogicInput>();

			var dflip0 = sim.Create<TFlipFlopElm>();

			var logicOut0 = sim.Create<LogicOutput>();
			var logicOut1 = sim.Create<LogicOutput>();

			sim.Connect(logicIn0.leadOut, dflip0.leadT);
			sim.Connect(logicIn1.leadOut, dflip0.leadCLK);

			sim.Connect(logicOut0.leadIn, dflip0.leadQ);
			sim.Connect(logicOut0.leadIn, dflip0.leadQL);

			logicIn0.toggle();
			sim.doTicks(200);

			Debug.Log();
			Debug.Log("T  ", dflip0.getLeadVoltage(0));
			Debug.Log("CLK", dflip0.getLeadVoltage(3));
			Debug.Log(" Q ", dflip0.getLeadVoltage(1));
			Debug.Log("|Q ", dflip0.getLeadVoltage(2));
			Debug.Log();

			//logicIn0.toggle();
			logicIn1.toggle();
			sim.analyze();
			sim.doTicks(200);

			Debug.Log();
			Debug.Log("T  ", dflip0.getLeadVoltage(0));
			Debug.Log("CLK", dflip0.getLeadVoltage(3));
			Debug.Log(" Q ", dflip0.getLeadVoltage(1));
			Debug.Log("|Q ", dflip0.getLeadVoltage(2));
			Debug.Log();

			//logicIn0.toggle();
			logicIn1.toggle();
			sim.analyze();
			sim.doTicks(200);

			Debug.Log();
			Debug.Log("T  ", dflip0.getLeadVoltage(0));
			Debug.Log("CLK", dflip0.getLeadVoltage(3));
			Debug.Log(" Q ", dflip0.getLeadVoltage(1));
			Debug.Log("|Q ", dflip0.getLeadVoltage(2));
			Debug.Log();

			Assert.Ignore();
		}

	}
}
