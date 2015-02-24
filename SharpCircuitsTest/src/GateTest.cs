using System;
using System.Collections.Generic;
using System.Linq;

using SharpCircuit;
using ServiceStack.Text;
using NUnit.Framework;

namespace SharpCircuitTest {

	[TestFixture]
	public class GateTest {

		[TestCase(0, 0, false)]
		[TestCase(1, 0, false)]
		[TestCase(0, 1, false)]
		[TestCase(1, 1, true )]
		public void AndGateTest(int in0, int in1, bool out0) {
			/*string js = System.IO.File.ReadAllText(string.Format("./{0}.json", "AndGateTest"));
			Circuit sim = JsonSerializer.DeserializeFromString<Circuit>(js);
			var voltage0 = sim.getElm(0) as LogicInputElm;
			var voltage1 = sim.getElm(1) as LogicInputElm;
			var logicOut = sim.getElm(2) as LogicOutputElm;*/

			Circuit sim = new Circuit();

			var logicIn0 = sim.Create<LogicInput>();
			var logicIn1 = sim.Create<LogicInput>();
			var logicOut = sim.Create<LogicOutput>();

			var gate = sim.Create<AndGate>();

			sim.Connect(logicIn0, 0, gate, 0);
			sim.Connect(logicIn1, 0, gate, 1);
			sim.Connect(logicOut.leadIn, gate.leadOut);

			//string js = JsonSerializer.SerializeToString(sim);
			//System.IO.File.WriteAllText(string.Format("./{0}.json", "AndGateTest"), js);

			logicIn0.setPosition(in0);
			logicIn1.setPosition(in1);

			sim.analyze();
			sim.doTicks(100);

			Assert.AreEqual(out0, logicOut.isHigh());

		}

		[TestCase(0, 0, false)]
		[TestCase(1, 0, true )]
		[TestCase(0, 1, true )]
		[TestCase(1, 1, true )]
		public void OrGateTest(int in0, int in1, bool out0) {
			Circuit sim = new Circuit();

			var logicIn0 = sim.Create<LogicInput>();
			var logicIn1 = sim.Create<LogicInput>();
			var logicOut = sim.Create<LogicOutput>();

			var gate = sim.Create<OrGate>();

			sim.Connect(logicIn0, 0, gate, 0);
			sim.Connect(logicIn1, 0, gate, 1);
			sim.Connect(logicOut.leadIn, gate.leadOut);

			logicIn0.setPosition(in0);
			logicIn1.setPosition(in1);
			
			sim.analyze();
			sim.doTicks(100);

			Assert.AreEqual(out0, logicOut.isHigh());
		}

		[TestCase(0, 0, true )]
		[TestCase(1, 0, true )]
		[TestCase(0, 1, true )]
		[TestCase(1, 1, false)]
		public void NandGateTest(int in0, int in1, bool out0) {
			Circuit sim = new Circuit();

			var logicIn0 = sim.Create<LogicInput>();
			var logicIn1 = sim.Create<LogicInput>();
			var logicOut = sim.Create<LogicOutput>();

			var gate = sim.Create<NandGate>();

			sim.Connect(logicIn0, 0, gate, 0);
			sim.Connect(logicIn1, 0, gate, 1);
			sim.Connect(logicOut.leadIn, gate.leadOut);

			logicIn0.setPosition(in0);
			logicIn1.setPosition(in1);

			sim.analyze();
			sim.doTicks(100);

			Assert.AreEqual(out0, logicOut.isHigh());
		}

		[TestCase(0, 0, true )]
		[TestCase(1, 0, false)]
		[TestCase(0, 1, false)]
		[TestCase(1, 1, false)]
		public void NorGateTest(int in0, int in1, bool out0) {
			Circuit sim = new Circuit();

			var logicIn0 = sim.Create<LogicInput>();
			var logicIn1 = sim.Create<LogicInput>();
			var logicOut = sim.Create<LogicOutput>();

			var gate = sim.Create<NorGate>();

			sim.Connect(logicIn0, 0, gate, 0);
			sim.Connect(logicIn1, 0, gate, 1);
			sim.Connect(logicOut, 0, gate, 2);

			logicIn0.setPosition(in0);
			logicIn1.setPosition(in1);
			
			sim.analyze();
			sim.doTicks(100);

			Assert.AreEqual(out0, logicOut.isHigh());
		}

		[TestCase(0, 0, false)]
		[TestCase(1, 0, true )]
		[TestCase(0, 1, true )]
		[TestCase(1, 1, false)]
		public void XorGateTest(int in0, int in1, bool out0) {
			Circuit sim = new Circuit();

			var logicIn0 = sim.Create<LogicInput>();
			var logicIn1 = sim.Create<LogicInput>();
			var logicOut = sim.Create<LogicOutput>();

			var gate = sim.Create<XorGate>();

			sim.Connect(logicIn0, 0, gate, 0);
			sim.Connect(logicIn1, 0, gate, 1);
			sim.Connect(logicOut.leadIn, gate.leadOut);

			logicIn0.setPosition(in0);
			logicIn1.setPosition(in1);
			
			sim.analyze();
			sim.doTicks(100);
			
			Assert.AreEqual(out0, logicOut.isHigh());
		}

		[TestCase(0, 0, false)]
		[TestCase(1, 0, true )]
		[TestCase(0, 1, true )]
		[TestCase(1, 1, false)]
		public void NandXorTest(int in0, int in1, bool out0) {
			Circuit sim = new Circuit();

			var logicIn0 = sim.Create<LogicInput>();
			var logicIn1 = sim.Create<LogicInput>();

			var nand0 = sim.Create<NandGate>();
			var nand1 = sim.Create<NandGate>();
			var nand2 = sim.Create<NandGate>();
			var nand3 = sim.Create<NandGate>();

			var logicOut = sim.Create<LogicOutput>();

			// upper input
			sim.Connect(logicIn0, 0, nand1, 0);
			sim.Connect(logicIn0, 0, nand0, 0);
			
			// lower input
			sim.Connect(logicIn1, 0, nand0, 1);
			sim.Connect(logicIn1, 0, nand2, 1);

			// connect 0
			sim.Connect(nand0, 2, nand1, 1);
			sim.Connect(nand0, 2, nand2, 0);

			// connect 3
			sim.Connect(nand1, 2, nand3, 0);
			sim.Connect(nand2, 2, nand3, 1);

			sim.Connect(logicOut, 0, nand3, 2);

			logicIn0.setPosition(in0);
			logicIn1.setPosition(in1);
			
			sim.analyze();
			sim.doTicks(100);

			Assert.AreEqual(out0, logicOut.isHigh());
		}

		[TestCase(0, 0, 0)]
		[TestCase(1, 0, 1)]
		[TestCase(0, 1, 1)]
		[TestCase(1, 1, 2)]
		public void HalfAdderTest(int in0, int in1, int i0) {
			Circuit sim = new Circuit();

			var logicIn0 = sim.Create<LogicInput>();
			var logicIn1 = sim.Create<LogicInput>();

			var andGate = sim.Create<AndGate>();
			var xorGate = sim.Create<XorGate>();

			var logicOut0 = sim.Create<LogicOutput>();
			var logicOut1 = sim.Create<LogicOutput>();

			sim.Connect(logicIn0, 0, andGate, 1);
			sim.Connect(logicIn0, 0, xorGate, 1);

			sim.Connect(logicIn1, 0, andGate, 0);
			sim.Connect(logicIn1, 0, xorGate, 0);

			sim.Connect(logicOut0.leadIn, andGate.leadOut);
			sim.Connect(logicOut1.leadIn, xorGate.leadOut);

			logicIn0.setPosition(in0);
			logicIn1.setPosition(in1);
			
			sim.analyze();
			sim.doTicks(100);

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

			var andGate0 = sim.Create<AndGate>();
			var andGate1 = sim.Create<AndGate>();
			
			var orGate0 = sim.Create<OrGate>();

			var xorGate0 = sim.Create<XorGate>();
			var xorGate1 = sim.Create<XorGate>();

			var logicOut0 = sim.Create<LogicOutput>();
			var logicOut1 = sim.Create<LogicOutput>();

			sim.Connect(logicIn0, 0, andGate1, 0);
			sim.Connect(logicIn0, 0, xorGate1, 0);

			sim.Connect(logicIn1, 0, andGate0, 0);
			sim.Connect(logicIn1, 0, xorGate0, 0);

			sim.Connect(logicIn2, 0, andGate0, 1);
			sim.Connect(logicIn2, 0, xorGate0, 1);

			sim.Connect(xorGate0, 2, andGate1, 1);
			sim.Connect(xorGate0, 2, xorGate1, 1);

			sim.Connect(orGate0, 0, andGate0, 2);
			sim.Connect(orGate0, 1, andGate1, 2);

			sim.Connect(logicOut0.leadIn, orGate0.leadOut);
			sim.Connect(logicOut1.leadIn, xorGate1.leadOut);

			logicIn0.setPosition(in0);
			logicIn1.setPosition(in1);
			logicIn2.setPosition(in2);
			
			sim.analyze();
			sim.doTicks(100);

			int i = 0;
			if(logicOut0.isHigh()) i += 2;
			if(logicOut1.isHigh()) i += 1;

			Assert.AreEqual(i0, i);
		}

		[TestCase(0, 0, 1)]
		[TestCase(0, 1, 2)]
		[TestCase(1, 0, 3)]
		[TestCase(1, 1, 4)]
		public void OneOfFourDecoderTest(int in0, int in1, int i0) {
			Circuit sim = new Circuit();

			var logicIn0 = sim.Create<LogicInput>();
			var logicIn1 = sim.Create<LogicInput>();

			var invert0 = sim.Create<Inverter>();
			var invert1 = sim.Create<Inverter>();

			var and0 = sim.Create<AndGate>();
			var and1 = sim.Create<AndGate>();
			var and2 = sim.Create<AndGate>();
			var and3 = sim.Create<AndGate>();

			var logicOut0 = sim.Create<LogicOutput>();
			var logicOut1 = sim.Create<LogicOutput>();
			var logicOut2 = sim.Create<LogicOutput>();
			var logicOut3 = sim.Create<LogicOutput>();

			sim.Connect(logicIn0, 0, and0, 0);
			sim.Connect(logicIn0, 0, and1, 0);

			sim.Connect(logicIn0.leadOut, invert0.leadIn);
			sim.Connect(invert0, 1, and2, 0);
			sim.Connect(invert0, 1, and3, 0);

			sim.Connect(logicIn1, 0, and0, 1);
			sim.Connect(logicIn1, 0, and2, 1);

			sim.Connect(logicIn1.leadOut, invert1.leadIn);
			sim.Connect(invert1, 1, and1, 1);
			sim.Connect(invert1, 1, and3, 1);

			sim.Connect(and0.leadOut, logicOut0.leadIn);
			sim.Connect(and1.leadOut, logicOut1.leadIn);
			sim.Connect(and2.leadOut, logicOut2.leadIn);
			sim.Connect(and3.leadOut, logicOut3.leadIn);

			logicIn0.setPosition(in0);
			logicIn1.setPosition(in1);
			
			sim.analyze();
			sim.doTicks(100);

			int i = 0;
			if(logicOut0.isHigh()) i += 4;
			if(logicOut1.isHigh()) i += 3;
			if(logicOut2.isHigh()) i += 2;
			if(logicOut3.isHigh()) i += 1;
			
			Assert.AreEqual(i0, i);
		}

		[TestCase(0, 0, 0, false)]
		[TestCase(0, 0, 1, false)]
		[TestCase(1, 0,	0, true )]
		[TestCase(1, 0, 1, false)]
		[TestCase(1, 1, 0, true )]
		[TestCase(1, 1, 1, true )]
		[TestCase(0, 1, 0, false)]
		[TestCase(0, 1, 1, true )]
		public void TwoToOneMuxTest(int in0, int in1, int in2, bool out0) {
			Circuit sim = new Circuit();

			// tri-state buffer 0

			var volt0 = sim.Create<VoltageInput>();
			var logicIn0 = sim.Create<LogicInput>();
			var nand0 = sim.Create<NandGate>();
			var and0 = sim.Create<AndGate>();
			var invert0 = sim.Create<Inverter>();
			var pmos0 = sim.Create<PMosfet>();
			var nmos0 = sim.Create<NMosfet>();
			var grnd0 = sim.Create<Ground>();

			sim.Connect(nand0, 0, and0, 1);
			sim.Connect(nand0, 1, logicIn0, 0);
			sim.Connect(nand0, 1, invert0, 0);
			sim.Connect(invert0, 1, and0, 0);

			sim.Connect(logicIn0.leadOut, invert0.leadIn);

			sim.Connect(pmos0.leadGate, nand0.leadOut);
			sim.Connect(nmos0.leadGate, and0.leadOut);
			sim.Connect(volt0.leadPos, pmos0.leadSrc);
			sim.Connect(pmos0.leadDrain, nmos0.leadDrain);
			sim.Connect(nmos0.leadSrc, grnd0.leadIn);

			// tri-state buffer 1

			var volt1 = sim.Create<VoltageInput>();
			var logicIn1 = sim.Create<LogicInput>();
			var nand1 = sim.Create<NandGate>();
			var and1 = sim.Create<AndGate>();
			var invert1 = sim.Create<Inverter>();
			var pmos1 = sim.Create<PMosfet>();
			var nmos1 = sim.Create<NMosfet>();
			var grnd1 = sim.Create<Ground>();

			sim.Connect(nand1, 0, and1, 1);
			sim.Connect(nand1, 1, logicIn1, 0);
			sim.Connect(nand1, 1, invert1, 0);
			sim.Connect(invert1, 1, and1, 0);

			sim.Connect(logicIn1.leadOut, invert1.leadIn);
			
			sim.Connect(pmos1.leadGate, nand1.leadOut);
			sim.Connect(nmos1.leadGate, and1.leadOut);
			sim.Connect(volt1.leadPos, pmos1.leadSrc);
			sim.Connect(pmos1.leadDrain, nmos1.leadDrain);
			sim.Connect(nmos1.leadSrc, grnd1.leadIn);

			//

			var invert2 = sim.Create<Inverter>();
			var logicIn2 = sim.Create<LogicInput>();
			var logicOut0 = sim.Create<LogicOutput>();

			var or0 = sim.Create<OrGate>();

			sim.Connect(invert2, 1, nand0, 0);
			sim.Connect(invert2, 1, and0, 1);
			sim.Connect(invert2, 0, nand1, 0);
			sim.Connect(invert2, 0, and1, 1);

			sim.Connect(logicIn2, 0, and1, 1);
			sim.Connect(or0, 0, nmos0, 2);
			sim.Connect(or0, 1, nmos1, 2);
			sim.Connect(or0.leadOut, logicOut0.leadIn);

			logicIn0.setPosition(in0);
			logicIn1.setPosition(in1);
			logicIn2.setPosition(in2);

			sim.analyze();
			sim.doTicks(100);

			Assert.AreEqual(out0, logicOut0.isHigh());
		}

		[TestCase(0, 0, 0, false)]
		[TestCase(1, 0, 0, false)]
		[TestCase(0, 1, 0, false)]
		[TestCase(0, 0, 1, false)]
		[TestCase(1, 1, 0, true )]
		[TestCase(1, 0, 1, true )]
		[TestCase(0, 1, 1, true )]
		[TestCase(1, 1, 1, true )]
		public void MajorityLogicTest(int in0, int in1, int in2, bool out0) {
			Circuit sim = new Circuit();

			var logicIn0 = sim.Create<LogicInput>();
			var logicIn1 = sim.Create<LogicInput>();
			var logicIn2 = sim.Create<LogicInput>();
			
			var nand0 = sim.Create<NandGate>();
			var nand1 = sim.Create<NandGate>();
			var nand2 = sim.Create<NandGate>();
			var nand3 = sim.Create<NandGate>();
			nand3.inputCount = 3;

			var logicOut = sim.Create<LogicOutput>();

			sim.Connect(logicIn0, 0, nand0, 0);
			sim.Connect(logicIn0, 0, nand2, 0);

			sim.Connect(logicIn1, 0, nand0, 1);
			sim.Connect(logicIn1, 0, nand1, 0);

			sim.Connect(logicIn2, 0, nand1, 1);
			sim.Connect(logicIn2, 0, nand2, 1);

			sim.Connect(nand3, 0, nand0, 2);
			sim.Connect(nand3, 1, nand1, 2);
			sim.Connect(nand3, 2, nand2, 2);

			sim.Connect(nand3, 4, logicOut, 0);

			logicIn0.setPosition(in0);
			logicIn1.setPosition(in1);
			logicIn2.setPosition(in2);
			
			sim.analyze();
			sim.doTicks(100);

			Assert.AreEqual(out0, logicOut.isHigh());
		}

	}
}
