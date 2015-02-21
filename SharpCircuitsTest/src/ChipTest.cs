using System;
using System.Collections.Generic;
using System.Linq;

using SharpCircuit;
using NUnit.Framework;

namespace SharpCircuitTest {

	[TestFixture]
	public class ChipTest {

		[TestCase(0, 0, 0)]
		[TestCase(1, 0, 1)]
		[TestCase(0, 1, 1)]
		[TestCase(1, 1, 2)]
		public void HalfAdderTest(int in0, int in1, int i0) {
			Circuit sim = new Circuit();

			var logicIn0 = sim.Create<LogicInputElm>();
			var logicIn1 = sim.Create<LogicInputElm>();

			var chip0 = sim.Create<HalfAdderElm>();

			var logicOut0 = sim.Create<LogicOutputElm>();
			var logicOut1 = sim.Create<LogicOutputElm>();

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

			var logicIn0 = sim.Create<LogicInputElm>();
			var logicIn1 = sim.Create<LogicInputElm>();
			var logicIn2 = sim.Create<LogicInputElm>();

			var chip0 = sim.Create<FullAdderElm>();

			var logicOut0 = sim.Create<LogicOutputElm>();
			var logicOut1 = sim.Create<LogicOutputElm>();

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
			Assert.Ignore("Not Implemented!");
		}

		[Test]
		public void JKlipFlopTest() {
			Assert.Ignore("Not Implemented!");
		}

		[Test]
		public void TFlipFlopTest() {
			Assert.Ignore("Not Implemented!");
		}

	}
}
