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

			int steps = 100;
			for(int x = 1; x <= steps; x++)
				sim.update(x);

			Assert.AreEqual(0.050, ground0.getCurrent());
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

			int steps = 100;
			for(int x = 1; x <= steps; x++)
				sim.update(x);
			
			Assert.AreEqual(0.050, ground0.getCurrent());
			Assert.AreEqual(0.005, ground1.getCurrent());
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

			int steps = 100;
			for(int x = 1; x <= steps; x++)
				sim.update(x);

			Assert.AreEqual(5.0, res0.getVoltageDiff());
			Assert.AreEqual(5.0, res1.getVoltageDiff());
			Assert.AreEqual(2.5, res2.getVoltageDiff());
			Assert.AreEqual(2.5, res3.getVoltageDiff());
			Assert.AreEqual(2.5, res4.getVoltageDiff());
			Assert.AreEqual(2.5, res5.getVoltageDiff());

			Assert.AreEqual(5.0, out0.getVoltageDiff());
			Assert.AreEqual(7.5, out1.getVoltageDiff());
			Assert.AreEqual(5.0, out2.getVoltageDiff());
			Assert.AreEqual(2.5, out3.getVoltageDiff());
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

			int steps = 100;
			for(int x = 1; x <= steps; x++)
				sim.update(x);

			Assert.AreEqual(0.025, Math.Round(volt0.getCurrent(), 8));

			Assert.AreEqual(0.01666667, Math.Round(res0.getCurrent(), 8));
			Assert.AreEqual(0.00833333, Math.Round(res1.getCurrent(), 8));
			Assert.AreEqual(0.01666667, Math.Round(res2.getCurrent(), 8));
			Assert.AreEqual(0.00833333, Math.Round(resX.getCurrent(), 8));

			Assert.AreEqual(0, wire0.getCurrent());
		}

	}
}
