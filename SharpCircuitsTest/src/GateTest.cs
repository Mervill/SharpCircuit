using System;
using System.Collections.Generic;
using System.Linq;

using SharpCircuit;
using ServiceStack.Text;
using NUnit.Framework;

namespace SharpCircuitTest {

	[TestFixture]
	public class GateTest {

		[TestCase(false, false, false)]
		[TestCase(true , false, false)]
		[TestCase(false, true , false)]
		[TestCase(true , true , true )]
		public void AndGateTest(bool volt0, bool volt1, bool isHigh) {

			string js = System.IO.File.ReadAllText(string.Format("./{0}.json", "AndGateTest"));
			Circuit sim = JsonSerializer.DeserializeFromString<Circuit>(js);

			var voltage0 = sim.getElm(0) as LogicInputElm;
			var voltage1 = sim.getElm(1) as LogicInputElm;
			var logicOut = sim.getElm(2) as LogicOutputElm;

			/*CirSim sim = new CirSim();

			var voltage0 = sim.Create<LogicInputElm>();
			var voltage1 = sim.Create<LogicInputElm>();
			var logicOut = sim.Create<LogicOutputElm>();

			var gate = sim.Create<AndGateElm>();

			sim.Connect(voltage0, 0, gate, 0);
			sim.Connect(voltage1, 0, gate, 1);
			sim.Connect(logicOut, 0, gate, 2);*/

			sim.update(1);

			//string js = JsonSerializer.SerializeToString(sim);
			//System.IO.File.WriteAllText(string.Format("./{0}.json", "AndGateTest"), js);

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

			Circuit sim = new Circuit();

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
			Circuit sim = new Circuit();

			var voltage0 = sim.Create<LogicInputElm>();
			var voltage1 = sim.Create<LogicInputElm>();
			var logicOut = sim.Create<LogicOutputElm>();

			if(volt1) voltage1.toggle();
			if(volt0) voltage0.toggle();

			var gate = sim.Create<OrGateElm>();

			sim.Connect(voltage0, 0, gate, 0);
			sim.Connect(voltage1, 0, gate, 1);
			sim.Connect(logicOut, 0, gate, 2);

			int steps = 0;
			sim.update(++steps);

			Assert.AreEqual(isHigh, logicOut.isHigh());
		}

		[TestCase(false, false, true )]
		[TestCase(true , false, true )]
		[TestCase(false, true , true )]
		[TestCase(true , true , false)]
		public void NandGateTest(bool volt0, bool volt1, bool isHigh) {
			Circuit sim = new Circuit();

			var voltage0 = sim.Create<LogicInputElm>();
			var voltage1 = sim.Create<LogicInputElm>();
			var logicOut = sim.Create<LogicOutputElm>();

			if(volt1) voltage1.toggle();
			if(volt0) voltage0.toggle();

			var gate = sim.Create<NandGateElm>();

			sim.Connect(voltage0, 0, gate, 0);
			sim.Connect(voltage1, 0, gate, 1);
			sim.Connect(logicOut, 0, gate, 2);

			int steps = 0;
			sim.update(++steps);

			Assert.AreEqual(isHigh, logicOut.isHigh());
		}

		[TestCase(false, false, true )]
		[TestCase(true , false, false)]
		[TestCase(false, true , false)]
		[TestCase(true , true , false)]
		public void NorGateTest(bool volt0, bool volt1, bool isHigh) {
			Circuit sim = new Circuit();

			var voltage0 = sim.Create<LogicInputElm>();
			var voltage1 = sim.Create<LogicInputElm>();
			var logicOut = sim.Create<LogicOutputElm>();

			if(volt1) voltage1.toggle();
			if(volt0) voltage0.toggle();

			var gate = sim.Create<NorGateElm>();

			sim.Connect(voltage0, 0, gate, 0);
			sim.Connect(voltage1, 0, gate, 1);
			sim.Connect(logicOut, 0, gate, 2);

			int steps = 0;
			sim.update(++steps);

			Assert.AreEqual(isHigh, logicOut.isHigh());
		}

		[TestCase(false, false, false)]
		[TestCase(true , false, true )]
		[TestCase(false, true , true )]
		[TestCase(true , true , false)]
		public void XorGateTest(bool volt0, bool volt1, bool isHigh) {
			Circuit sim = new Circuit();

			var voltage0 = sim.Create<LogicInputElm>();
			var voltage1 = sim.Create<LogicInputElm>();
			var logicOut = sim.Create<LogicOutputElm>();

			if(volt1) voltage1.toggle();
			if(volt0) voltage0.toggle();

			var gate = sim.Create<XorGateElm>();

			sim.Connect(voltage0, 0, gate, 0);
			sim.Connect(voltage1, 0, gate, 1);
			sim.Connect(logicOut, 0, gate, 2);

			int steps = 0;
			sim.update(++steps);

			Assert.AreEqual(isHigh, logicOut.isHigh());
		}

		[TestCase(false, false, false)]
		[TestCase(true,  false, true )]
		[TestCase(false, true,  true )]
		[TestCase(true,  true,  false)]
		public void ExclusiveOrTest(bool switch0, bool switch1, bool isHigh) {
			Circuit sim = new Circuit();

			var volt0 = sim.Create<LogicInputElm>();
			var volt1 = sim.Create<LogicInputElm>();

			var nand0 = sim.Create<NandGateElm>();
			var nand1 = sim.Create<NandGateElm>();
			var nand2 = sim.Create<NandGateElm>();
			var nand3 = sim.Create<NandGateElm>();

			var logicOut = sim.Create<LogicOutputElm>();

			if(switch0) volt0.toggle();
			if(switch1) volt1.toggle();

			// upper input
			sim.Connect(volt0, 0, nand1, 0);
			sim.Connect(volt0, 0, nand0, 0);
			
			// lower input
			sim.Connect(volt1, 0, nand0, 1);
			sim.Connect(volt1, 0, nand2, 1);

			// connect 0
			sim.Connect(nand0, 2, nand1, 1);
			sim.Connect(nand0, 2, nand2, 0);

			// connect 3
			sim.Connect(nand1, 2, nand3, 0);
			sim.Connect(nand2, 2, nand3, 1);

			sim.Connect(logicOut, 0, nand3, 2);

			int steps = 0;
			sim.update(++steps);
			sim.update(++steps);
			
			Assert.AreEqual(isHigh, logicOut.isHigh());
		}

	}
}
