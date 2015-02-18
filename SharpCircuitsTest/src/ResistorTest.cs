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
		public void SimpleResistorTest() {
			Circuit sim = new Circuit();

			var volt0 = sim.Create<RailElm>();
			var res1 = sim.Create<ResistorElm>();
			var ground0 = sim.Create<GroundElm>();

			sim.Connect(volt0.leadOut, res1.leadIn);
			sim.Connect(res1.leadOut, ground0.leadIn);

			for(int x = 1; x <= 100; x++)
				sim.update(sim.timeStep);

			TestUtils.Compare(ground0.getCurrent(), 0.05, 8);
		}

		[Test]
		public void OhmsLawTest() {
			Circuit sim = new Circuit();

			var volt0 = sim.Create<RailElm>();
			var res0 = sim.Create<ResistorElm>( 100);
			var res1 = sim.Create<ResistorElm>(1000);
			var ground0 = sim.Create<GroundElm>();
			var ground1 = sim.Create<GroundElm>();

			sim.Connect(volt0, 0, res0,    0);
			sim.Connect(volt0, 0, res1,    0);
			sim.Connect(res0,  1, ground0, 0);
			sim.Connect(res1,  1, ground1, 0);

			for(int x = 1; x <= 100; x++)
				sim.update(sim.timeStep);
			
			TestUtils.Compare(ground0.getCurrent(), 0.05, 8);
			TestUtils.Compare(ground1.getCurrent(), 0.005, 8);
		}

		[Test]
		public void VoltageDividerTest() {
			Circuit sim = new Circuit();

			var volt0 = sim.Create<DCVoltageElm>();
			volt0.maxVoltage = 10;

			var res0 = sim.Create<ResistorElm>(10000);
			var res1 = sim.Create<ResistorElm>(10000);

			sim.Connect(volt0, 1, res0,  0);
			sim.Connect(res0,  1, res1,  0);
			sim.Connect(res1,  1, volt0, 0);

			var res2 = sim.Create<ResistorElm>(10000);
			var res3 = sim.Create<ResistorElm>(10000);
			var res4 = sim.Create<ResistorElm>(10000);
			var res5 = sim.Create<ResistorElm>(10000);

			sim.Connect(volt0, 1, res2,  0);
			sim.Connect(res2,  1, res3,  0);
			sim.Connect(res3,  1, res4,  0);
			sim.Connect(res4,  1, res5,  0);
			sim.Connect(res5,  1, volt0, 0);

			var out0 = sim.Create<OutputElm>();
			var out1 = sim.Create<OutputElm>();
			var out2 = sim.Create<OutputElm>();
			var out3 = sim.Create<OutputElm>();

			sim.Connect(out0, 0, res0, 1);
			sim.Connect(out1, 0, res2, 1);
			sim.Connect(out2, 0, res3, 1);
			sim.Connect(out3, 0, res4, 1);

			for(int x = 1; x <= 100; x++)
				sim.update(sim.timeStep);

			TestUtils.Compare(res0.getVoltageDiff(), 5.0, 8);
			TestUtils.Compare(res1.getVoltageDiff(), 5.0, 8);
			TestUtils.Compare(res2.getVoltageDiff(), 2.5, 8);
			TestUtils.Compare(res3.getVoltageDiff(), 2.5, 8);
			TestUtils.Compare(res4.getVoltageDiff(), 2.5, 8);
			TestUtils.Compare(res5.getVoltageDiff(), 2.5, 8);

			TestUtils.Compare(out0.getVoltageDiff(), 5.0, 8);
			TestUtils.Compare(out1.getVoltageDiff(), 7.5, 8);
			TestUtils.Compare(out2.getVoltageDiff(), 5.0, 8);
			TestUtils.Compare(out3.getVoltageDiff(), 2.5, 8);
		}

		[Test]
		public void WheatstoneBridgeTest() {
			Circuit sim = new Circuit();

			var volt0 = sim.Create<DCVoltageElm>();

			var res0 = sim.Create<ResistorElm>(200);
			var res1 = sim.Create<ResistorElm>(400);
			
			sim.Connect(volt0, 1, res0, 0);
			sim.Connect(volt0, 1, res1, 0);

			var wire0 = sim.Create<WireElm>();

			sim.Connect(wire0, 0, res0, 1);
			sim.Connect(wire0, 1, res1, 1);

			var res2 = sim.Create<ResistorElm>(100);
			var resX = sim.Create<ResistorElm>(200);

			sim.Connect(res0, 1, res2, 0);
			sim.Connect(res1, 1, resX, 0);

			sim.Connect(volt0, 0, res2, 1);
			sim.Connect(volt0, 0, resX, 1);

			for(int x = 1; x <= 100; x++)
				sim.update(sim.timeStep);

			TestUtils.Compare(0.025, volt0.getCurrent(), 3);

			TestUtils.Compare(res0.getCurrent(), 0.01666667, 8);
			TestUtils.Compare(res1.getCurrent(), 0.00833334, 8);
			TestUtils.Compare(res2.getCurrent(), 0.01666667, 8);
			TestUtils.Compare(resX.getCurrent(), 0.00833334, 8);

			Assert.AreEqual(0, wire0.getCurrent());
		}

	}
}
