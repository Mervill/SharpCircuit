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
			
			var npn0 = sim.Create<NPNTransistorElm>();

			var baseVoltage = sim.Create<VoltageInputElm>(VoltageElm.WaveType.DC);
			baseVoltage.maxVoltage = 0.7025;

			var collectorVoltage = sim.Create<VoltageInputElm>(VoltageElm.WaveType.DC);
			collectorVoltage.maxVoltage = 2;

			var ground = sim.Create<GroundElm>();

			var baseWire = sim.Create<WireElm>();
			var collectorWire = sim.Create<WireElm>();
			var emitterWire = sim.Create<WireElm>();

			sim.Connect(baseVoltage.leadVoltage, baseWire.leadIn);
			sim.Connect(baseWire.leadOut, npn0.leadBase);
			
			sim.Connect(collectorVoltage.leadVoltage, collectorWire.leadIn);
			sim.Connect(collectorWire.leadOut, npn0.leadCollector);

			sim.Connect(ground.leadIn, emitterWire.leadIn);
			sim.Connect(emitterWire.leadOut, npn0.leadEmitter);

			sim.doTicks(100);

			TestUtils.Compare(baseWire.getCurrent(), 0.00158254, 8);
			TestUtils.Compare(collectorWire.getCurrent(), 0.15825359, 8);
			TestUtils.Compare(emitterWire.getCurrent(), -0.15983612, 8);
		}

		[Test]
		public void PNPTransistorTest() {
			Circuit sim = new Circuit();
			
			var pnp0 = sim.Create<PNPTransistorElm>();

			var baseVoltage = sim.Create<VoltageInputElm>(VoltageElm.WaveType.DC);
			baseVoltage.maxVoltage = 1.3;

			var collectorVoltage = sim.Create<VoltageInputElm>(VoltageElm.WaveType.DC);
			collectorVoltage.maxVoltage = 2;

			var emitterVoltage = sim.Create<VoltageInputElm>(VoltageElm.WaveType.DC);
			emitterVoltage.maxVoltage = 2;

			var baseWire = sim.Create<WireElm>();
			var collectorWire = sim.Create<WireElm>();
			var emitterWire = sim.Create<WireElm>();

			sim.Connect(baseVoltage.leadVoltage, baseWire.leadIn);
			sim.Connect(baseWire.leadOut, pnp0.leadBase);

			sim.Connect(collectorVoltage.leadVoltage, collectorWire.leadIn);
			sim.Connect(collectorWire.leadOut, pnp0.leadCollector);

			sim.Connect(emitterVoltage.leadVoltage, emitterWire.leadIn);
			sim.Connect(emitterWire.leadOut, pnp0.leadEmitter);

			sim.doTicks(100);

			TestUtils.Compare(baseWire.getCurrent(), -0.07374479, 8);
			TestUtils.Compare(collectorWire.getCurrent(), 0.00143194, 8);
			TestUtils.Compare(emitterWire.getCurrent(), 0.07231284, 8);
		}

		[TestCase(true)]
		[TestCase(false)]
		public void SwitchTest(bool In0) {
			Circuit sim = new Circuit();

			var volt0 = sim.Create<DCVoltageElm>();

			var res0 = sim.Create<ResistorElm>(10000);
			var res1 = sim.Create<ResistorElm>(300);

			var switch0 = sim.Create<SwitchElm>();

			var npn0 = sim.Create<NPNTransistorElm>();

			sim.Connect(volt0.leadPos, res0.leadIn);
			sim.Connect(res0.leadOut, volt0.leadNeg);

			/*sim.Connect(volt0.leadPos, res0.leadIn);
			sim.Connect(volt0.leadPos, res1.leadIn);

			sim.Connect(switch0, 0, res0, 1);
			sim.Connect(switch0, 1, npn0, 0);

			sim.Connect(res1.leadOut, npn0.leadCollector);
			sim.Connect(npn0.leadEmitter, volt0.leadNeg);*/

			//if(In0) switch0.toggle();

			sim.doTicks(100);
			if(sim.errorMessage != null)
				Debug.Log(sim.errorMessage);

			//Debug.Log(res0.getVoltageDiff());
			//Debug.Log(res0.getCurrent());
			//Debug.Log(res1.getVoltageDiff());
			//Debug.Log(res1.getCurrent());

			//Debug.Log(volt0.getLeadVoltage(1));

			//Debug.Log();
			//Debug.Log(res0.getLeadVoltage(0));
			//Debug.Log(res0.getLeadVoltage(1));

			//Debug.Log();
			//Debug.Log(res1.getLeadVoltage(0));
			//Debug.Log(res1.getLeadVoltage(1));

			//Debug.Log();
			//Debug.Log(switch0.getLeadVoltage(0));
			//Debug.Log(switch0.getLeadVoltage(1));
			Assert.Ignore();
		}

		[Test]
		public void CurrentSourceRampTest() {
			Assert.Ignore("Not Implemented!");
		}

		[TestCase(true,  5.2E-10)]
		[TestCase(false, 0.014121382555)]
		public void DarlingtonPairTest(bool In0, double i0) {
			Circuit sim = new Circuit();

			var volt0 = sim.Create<VoltageInputElm>();

			var res0 = sim.Create<ResistorElm>(2000000);
			var res1 = sim.Create<ResistorElm>(300);

			var switch0 = sim.Create<SwitchElm>();

			var npn0 = sim.Create<NPNTransistorElm>();
			var npn1 = sim.Create<NPNTransistorElm>();

			var groun0 = sim.Create<GroundElm>();

			sim.Connect(volt0.leadVoltage, res0.leadIn);
			sim.Connect(volt0.leadVoltage, res1.leadIn);

			sim.Connect(switch0, 0, res0, 1);

			sim.Connect(npn0, 0, switch0, 1);
			sim.Connect(npn0.leadCollector, res1.leadOut);
			sim.Connect(npn0.leadEmitter, npn1.leadBase);

			sim.Connect(npn1.leadCollector, res1.leadOut);
			sim.Connect(npn1.leadEmitter, groun0.leadIn);

			if(In0) switch0.toggle();

			sim.doTicks(100);

			Assert.AreEqual(i0, Math.Round(groun0.getCurrent(), 12));
		}

	}
}
