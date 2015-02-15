using System;
using System.Collections.Generic;
using System.Linq;

using SharpCircuit;
using NUnit.Framework;

namespace SharpCircuitTest {

	[TestFixture]
	public class TransistorTest {

		[Test]
		public void NPNTransistorTest() {
			Circuit sim = new Circuit();
			var transistor = sim.Create<NPNTransistorElm>();

			var baseVoltage = sim.Create<RailElm>();
			baseVoltage.maxVoltage = 0.7025;

			var collectorVoltage = sim.Create<RailElm>();
			collectorVoltage.maxVoltage = 2;

			var ground = sim.Create<GroundElm>();

			var baseWire = sim.Create<WireElm>();
			var collectorWire = sim.Create<WireElm>();
			var emitterWire = sim.Create<WireElm>();

			sim.Connect(baseVoltage, 0, baseWire, 0);
			sim.Connect(baseWire, 1, transistor, 0);
			
			sim.Connect(collectorVoltage, 0, collectorWire, 0);
			sim.Connect(collectorWire, 1, transistor, 1);

			sim.Connect(ground, 0, emitterWire, 0);
			sim.Connect(emitterWire, 1, transistor, 2);

			int steps = 1000;
			for(int x = 1; x <= steps; x++)
				sim.update(x);

			Assert.AreEqual(0.01, Math.Round(sim.time, 4));

			Assert.AreEqual( 0.00158254, Math.Round(baseWire.getCurrent(), 8));
			Assert.AreEqual( 0.15825359, Math.Round(collectorWire.getCurrent(), 8));
			Assert.AreEqual(-0.15983612, Math.Round(emitterWire.getCurrent(), 8));
		}

		[Test]
		public void PNPTransistorTest() {
			Circuit sim = new Circuit();
			var transistor = sim.Create<PNPTransistorElm>();

			var baseVoltage = sim.Create<RailElm>();
			baseVoltage.maxVoltage = 1.3;

			var collectorVoltage = sim.Create<RailElm>();
			collectorVoltage.maxVoltage = 2;

			var emitterVoltage = sim.Create<RailElm>();
			emitterVoltage.maxVoltage = 2;

			var baseWire = sim.Create<WireElm>();
			var collectorWire = sim.Create<WireElm>();
			var emitterWire = sim.Create<WireElm>();

			sim.Connect(baseVoltage, 0, baseWire, 0);
			sim.Connect(baseWire, 1, transistor, 0);

			sim.Connect(collectorVoltage, 0, collectorWire, 0);
			sim.Connect(collectorWire, 1, transistor, 1);

			sim.Connect(emitterVoltage, 0, emitterWire, 0);
			sim.Connect(emitterWire, 1, transistor, 2);

			int steps = 1000;
			for(int x = 1; x <= steps; x++)
				sim.update(x);

			Assert.AreEqual(0.01, Math.Round(sim.time, 4));

			Assert.AreEqual(-0.07374479, Math.Round(baseWire.getCurrent(), 8));
			Assert.AreEqual( 0.00143194, Math.Round(collectorWire.getCurrent(), 8));
			Assert.AreEqual( 0.07231285, Math.Round(emitterWire.getCurrent(), 8));
		}

	}
}
