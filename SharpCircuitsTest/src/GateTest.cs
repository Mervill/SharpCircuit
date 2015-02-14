using System;
using System.Collections.Generic;
using System.Linq;

using SharpCircuit;
using NUnit.Framework;

namespace SharpCircuitTest {

	[TestFixture]
	public class GateTest {

		[TestCase(false, false, false)]
		[TestCase(true , false, false)]
		[TestCase(false, true , false)]
		[TestCase(true , true , true )]
		public void AndGateTest(bool volt0, bool volt1, bool isHigh) {

			CirSim sim = new CirSim();

			var voltage0 = sim.Create<LogicInputElm>();
			var voltage1 = sim.Create<LogicInputElm>();
			var logicOut = sim.Create<LogicOutputElm>();

			var gate = sim.Create<AndGateElm>();

			sim.Connect(voltage0, 0, gate, 0);
			sim.Connect(voltage1, 0, gate, 1);
			sim.Connect(logicOut, 0, gate, 2);

			sim.update(1);

			if(volt1) voltage1.toggle();
			if(volt0) voltage0.toggle();
			sim.needAnalyze();

			sim.update(2);
			Assert.AreEqual(isHigh, logicOut.isHigh());
		}

		[Test]
		public void AndGateVarTest() {
			Tuple<bool, bool, bool>[] cases = new Tuple<bool, bool, bool>[] {
				new Tuple<bool, bool, bool>(false, true , false),
				new Tuple<bool, bool, bool>(false, true , true ),
				new Tuple<bool, bool, bool>(true , false, false),
				new Tuple<bool, bool, bool>(false, false, false)
			};

			CirSim sim = new CirSim();

			var voltage0 = sim.Create<LogicInputElm>();
			var voltage1 = sim.Create<LogicInputElm>();
			var logicOut = sim.Create<LogicOutputElm>();

			var gate = sim.Create<AndGateElm>();

			sim.Connect(voltage0, 0, gate, 0);
			sim.Connect(voltage1, 0, gate, 1);
			sim.Connect(logicOut, 0, gate, 2);

			sim.update(1);
			Debug.Log(logicOut.isHigh());

			voltage0.toggle();
			voltage1.toggle();
			sim.update(2);
			Debug.Log(logicOut.isHigh());

			Assert.Fail();
		}

		[TestCase(false, false, false)]
		[TestCase(true , false, true )]
		[TestCase(false, true , true )]
		[TestCase(true , true , true )]
		public void OrGateTest(bool volt0, bool volt1, bool isHigh) {

			CirSim sim = new CirSim();

			var voltage0 = sim.Create<LogicInputElm>();
			var voltage1 = sim.Create<LogicInputElm>();
			var logicOut = sim.Create<LogicOutputElm>();

			if(volt1) voltage1.toggle();
			if(volt0) voltage0.toggle();

			var gate = sim.Create<OrGateElm>();

			sim.Connect(voltage0, 0, gate, 0);
			sim.Connect(voltage1, 0, gate, 1);
			sim.Connect(logicOut, 0, gate, 2);

			sim.update(1);
			Assert.AreEqual(isHigh, logicOut.isHigh());
		}

		[TestCase(false, false, true )]
		[TestCase(true , false, true )]
		[TestCase(false, true , true )]
		[TestCase(true , true , false)]
		public void NandGateTest(bool volt0, bool volt1, bool isHigh) {

			CirSim sim = new CirSim();

			var voltage0 = sim.Create<LogicInputElm>();
			var voltage1 = sim.Create<LogicInputElm>();
			var logicOut = sim.Create<LogicOutputElm>();

			if(volt1) voltage1.toggle();
			if(volt0) voltage0.toggle();

			var gate = sim.Create<NandGateElm>();

			sim.Connect(voltage0, 0, gate, 0);
			sim.Connect(voltage1, 0, gate, 1);
			sim.Connect(logicOut, 0, gate, 2);

			sim.update(1);
			Assert.AreEqual(isHigh, logicOut.isHigh());
		}

		[TestCase(false, false, true )]
		[TestCase(true , false, false)]
		[TestCase(false, true , false)]
		[TestCase(true , true , false)]
		public void NorGateTest(bool volt0, bool volt1, bool isHigh) {

			CirSim sim = new CirSim();

			var voltage0 = sim.Create<LogicInputElm>();
			var voltage1 = sim.Create<LogicInputElm>();
			var logicOut = sim.Create<LogicOutputElm>();

			if(volt1) voltage1.toggle();
			if(volt0) voltage0.toggle();

			var gate = sim.Create<NorGateElm>();

			sim.Connect(voltage0, 0, gate, 0);
			sim.Connect(voltage1, 0, gate, 1);
			sim.Connect(logicOut, 0, gate, 2);

			sim.update(1);
			Assert.AreEqual(isHigh, logicOut.isHigh());
		}

		[TestCase(false, false, false)]
		[TestCase(true , false, true )]
		[TestCase(false, true , true )]
		[TestCase(true , true , false)]
		public void XorGateTest(bool volt0, bool volt1, bool isHigh) {

			CirSim sim = new CirSim();

			var voltage0 = sim.Create<LogicInputElm>();
			var voltage1 = sim.Create<LogicInputElm>();
			var logicOut = sim.Create<LogicOutputElm>();

			if(volt1) voltage1.toggle();
			if(volt0) voltage0.toggle();

			var gate = sim.Create<XorGateElm>();

			sim.Connect(voltage0, 0, gate, 0);
			sim.Connect(voltage1, 0, gate, 1);
			sim.Connect(logicOut, 0, gate, 2);

			sim.update(1);
			Assert.AreEqual(isHigh, logicOut.isHigh());
		}
	}
}
