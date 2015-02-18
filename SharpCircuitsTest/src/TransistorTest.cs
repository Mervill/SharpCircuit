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

			sim.Connect(baseVoltage.leadOut, baseWire.leadIn);
			sim.Connect(baseWire.leadOut, transistor.leadBase);
			
			sim.Connect(collectorVoltage.leadOut, collectorWire.leadIn);
			sim.Connect(collectorWire.leadOut, transistor.leadCollector);

			sim.Connect(ground.leadIn, emitterWire.leadIn);
			sim.Connect(emitterWire.leadOut, transistor.leadEmitter);

			for(int x = 1; x <= 100; x++)
				sim.update(sim.timeStep);

			TestUtils.Compare(baseWire.getCurrent(), 0.00158254, 8);
			TestUtils.Compare(collectorWire.getCurrent(), 0.15825359, 8);
			TestUtils.Compare(emitterWire.getCurrent(), -0.15983612, 8);
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

			sim.Connect(baseVoltage.leadOut, baseWire.leadIn);
			sim.Connect(baseWire.leadOut, transistor.leadBase);

			sim.Connect(collectorVoltage.leadOut, collectorWire.leadIn);
			sim.Connect(collectorWire.leadOut, transistor.leadCollector);

			sim.Connect(emitterVoltage.leadOut, emitterWire.leadIn);
			sim.Connect(emitterWire.leadOut, transistor.leadEmitter);

			for(int x = 1; x <= 100; x++)
				sim.update(sim.timeStep);

			TestUtils.Compare(baseWire.getCurrent(), -0.07374479, 8);
			TestUtils.Compare(collectorWire.getCurrent(), 0.00143194, 8);
			TestUtils.Compare(emitterWire.getCurrent(), 0.07231284, 8);
		}

	}
}
