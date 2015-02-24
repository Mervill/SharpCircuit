using System;
using System.Collections.Generic;
using System.Linq;

using SharpCircuit;
using ServiceStack.Text;
using NUnit.Framework;

namespace SharpCircuitTest {

	[TestFixture]
	public class ResistorTest {

		[Test]
		public void LeastResistanceTest() {
			Circuit sim = new Circuit();

			var volt0 = sim.Create<VoltageInput>(Voltage.WaveType.DC);
			var res0 = sim.Create<Resistor>( 100);
			var res1 = sim.Create<Resistor>(1000);
			var ground0 = sim.Create<Ground>();
			var ground1 = sim.Create<Ground>();

			sim.Connect(volt0, 0, res0,    0);
			sim.Connect(volt0, 0, res1,    0);
			sim.Connect(res0,  1, ground0, 0);
			sim.Connect(res1,  1, ground1, 0);

			for(int x = 1; x <= 100; x++)
				sim.doTick();
			
			TestUtils.Compare(ground0.getCurrent(), 0.05, 8);
			TestUtils.Compare(ground1.getCurrent(), 0.005, 8);
		}

		[TestCase( 2)]
		[TestCase( 4)]
		[TestCase( 6)]
		[TestCase( 8)]
		[TestCase(10)]
		public void LawOfResistorsInSeriesTest(int in0) {
			Circuit sim = new Circuit();

			var volt0 = sim.Create<DCVoltageSource>();
			var volt1 = sim.Create<DCVoltageSource>();
			var resCompare = sim.Create<Resistor>(in0 * 100);

			List<Resistor> resistors = new List<Resistor>();
			for(int i = 0; i < in0; i++)
				resistors.Add(sim.Create<Resistor>());

			sim.Connect(volt0.leadPos, resistors.First().leadIn);
			
			for(int i = 1; i < in0 - 1; i++)
				sim.Connect(resistors[i - 1].leadOut, resistors[i].leadIn);
			
			sim.Connect(volt0.leadNeg, resistors.Last().leadOut);

			sim.Connect(volt1.leadPos, resCompare.leadIn);
			sim.Connect(resCompare.leadOut, volt1.leadNeg);

			sim.doTicks(100);

			Assert.AreEqual(Math.Round(resistors.Last().getCurrent(), 12), Math.Round(resCompare.getCurrent(), 12));
		}

		[Test]
		public void VoltageDividerTest() {
			Circuit sim = new Circuit();

			var volt0 = sim.Create<DCVoltageSource>();
			volt0.maxVoltage = 10;

			var res0 = sim.Create<Resistor>(10000);
			var res1 = sim.Create<Resistor>(10000);

			sim.Connect(volt0, 1, res0,  0);
			sim.Connect(res0,  1, res1,  0);
			sim.Connect(res1,  1, volt0, 0);

			var res2 = sim.Create<Resistor>(10000);
			var res3 = sim.Create<Resistor>(10000);
			var res4 = sim.Create<Resistor>(10000);
			var res5 = sim.Create<Resistor>(10000);

			sim.Connect(volt0, 1, res2,  0);
			sim.Connect(res2,  1, res3,  0);
			sim.Connect(res3,  1, res4,  0);
			sim.Connect(res4,  1, res5,  0);
			sim.Connect(res5,  1, volt0, 0);

			var out0 = sim.Create<Output>();
			var out1 = sim.Create<Output>();
			var out2 = sim.Create<Output>();
			var out3 = sim.Create<Output>();

			sim.Connect(out0, 0, res0, 1);
			sim.Connect(out1, 0, res2, 1);
			sim.Connect(out2, 0, res3, 1);
			sim.Connect(out3, 0, res4, 1);

			for(int x = 1; x <= 100; x++)
				sim.doTick();

			TestUtils.Compare(res0.getVoltageDelta(), 5.0, 8);
			TestUtils.Compare(res1.getVoltageDelta(), 5.0, 8);
			TestUtils.Compare(res2.getVoltageDelta(), 2.5, 8);
			TestUtils.Compare(res3.getVoltageDelta(), 2.5, 8);
			TestUtils.Compare(res4.getVoltageDelta(), 2.5, 8);
			TestUtils.Compare(res5.getVoltageDelta(), 2.5, 8);

			TestUtils.Compare(out0.getVoltageDelta(), 5.0, 8);
			TestUtils.Compare(out1.getVoltageDelta(), 7.5, 8);
			TestUtils.Compare(out2.getVoltageDelta(), 5.0, 8);
			TestUtils.Compare(out3.getVoltageDelta(), 2.5, 8);
		}

		[Test]
		public void WheatstoneBridgeTest() {
			Circuit sim = new Circuit();

			var volt0 = sim.Create<DCVoltageSource>();

			var res0 = sim.Create<Resistor>(200);
			var res1 = sim.Create<Resistor>(400);
			
			sim.Connect(volt0, 1, res0, 0);
			sim.Connect(volt0, 1, res1, 0);

			var wire0 = sim.Create<Wire>();

			sim.Connect(wire0, 0, res0, 1);
			sim.Connect(wire0, 1, res1, 1);

			var res2 = sim.Create<Resistor>(100);
			var resX = sim.Create<Resistor>(200);

			sim.Connect(res0, 1, res2, 0);
			sim.Connect(res1, 1, resX, 0);

			sim.Connect(volt0, 0, res2, 1);
			sim.Connect(volt0, 0, resX, 1);

			for(int x = 1; x <= 100; x++)
				sim.doTick();

			TestUtils.Compare(0.025, volt0.getCurrent(), 3);

			TestUtils.Compare(res0.getCurrent(), 0.01666667, 8);
			TestUtils.Compare(res1.getCurrent(), 0.00833334, 8);
			TestUtils.Compare(res2.getCurrent(), 0.01666667, 8);
			TestUtils.Compare(resX.getCurrent(), 0.00833334, 8);

			Assert.AreEqual(0, wire0.getCurrent());
		}

	}
}
