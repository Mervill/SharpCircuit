using System;
using System.Collections.Generic;
using System.Linq;

using SharpCircuit;
using ServiceStack.Text;
using NUnit.Framework;

namespace SharpCircuitTest {

	[TestFixture]
	public class OpAmpTest {

		[Test]
		public void SimpleOpAmpTest() {
			Circuit sim = new Circuit();

			var volt0 = sim.Create<VoltageInput>(Voltage.WaveType.DC);
			volt0.maxVoltage = 3;

			var volt1 = sim.Create<VoltageInput>(Voltage.WaveType.DC);
			volt1.maxVoltage = 4;

			var opAmp0 = sim.Create<OpAmp>();
			var analogOut0 = sim.Create<Output>();

			sim.Connect(volt0.leadPos, opAmp0.leadNeg);
			sim.Connect(volt1.leadPos, opAmp0.leadPos);
			sim.Connect(opAmp0.leadOut, analogOut0.leadIn);

			for(int x = 1; x <= 100; x++)
				sim.doTick();
			
			TestUtils.Compare(analogOut0.getVoltageDelta(), 15, 2);
		}

		[Test]
		public void OpAmpFeedbackTest() {
			Circuit sim = new Circuit();

			var volt0 = sim.Create<VoltageInput>(Voltage.WaveType.DC);
			var opAmp0 = sim.Create<OpAmp>();
			var analogOut0 = sim.Create<Output>();

			sim.Connect(opAmp0.leadOut, opAmp0.leadNeg);
			sim.Connect(volt0.leadPos, opAmp0.leadPos);
			sim.Connect(analogOut0.leadIn, opAmp0.leadOut);

			for(int x = 1; x <= 100; x++)
				sim.doTick();

			TestUtils.Compare(analogOut0.getVoltageDelta(), 5, 2);
		}

		[Test]
		public void GyratorTest() {
			Circuit sim = new Circuit();

			var square0 = sim.Create<VoltageInput>(Voltage.WaveType.SQUARE);
			square0.frequency = 20;

			var res0 = sim.Create<Resistor>(1000);
			var res1 = sim.Create<Resistor>(20000);
			var cap0 = sim.Create<CapacitorElm>(2.5E-7);
			var opAmp0 = sim.Create<OpAmp>();
			var grnd0 = sim.Create<Ground>();

			sim.Connect(square0.leadPos, res0.leadIn);
			sim.Connect(opAmp0.leadNeg, res0.leadOut);
			sim.Connect(opAmp0.leadOut, opAmp0.leadNeg);

			sim.Connect(square0.leadPos, cap0.leadIn);
			sim.Connect(opAmp0.leadPos, cap0.leadOut);
			sim.Connect(cap0.leadOut, res1.leadIn);
			sim.Connect(res1.leadOut, grnd0.leadIn);

			var square1 = sim.Create<VoltageInput>(Voltage.WaveType.SQUARE);
			square1.frequency = 20;

			var res3 = sim.Create<Resistor>(1000);
			var induct0 = sim.Create<InductorElm>(5);
			var grnd1 = sim.Create<Ground>();

			sim.Connect(square1.leadPos, res3.leadIn);
			sim.Connect(res3.leadOut, induct0.leadIn);
			sim.Connect(induct0.leadOut, grnd0.leadIn);

			var scope0 = sim.Watch(square0);
			var scope1 = sim.Watch(square1);

			double cycleTime = 1 / square0.frequency;
			double quarterCycleTime = cycleTime / 4;
			int steps = (int)(cycleTime / sim.timeStep);
			sim.doTicks(steps);

			for(int x = 0; x < scope0.Count; x++) {
				Assert.AreEqual(scope0[x].voltage, scope1[x].voltage);
				Assert.AreEqual(0, Math.Round(scope0[x].current - scope1[x].current, 3));
			}

		}

		[Test]
		public void CapacitanceMultiplierTest() {
			Circuit sim = new Circuit();

			var square0 = sim.Create<VoltageInput>(Voltage.WaveType.SQUARE);
			square0.frequency = 30;

			var res0 = sim.Create<Resistor>(100000);
			var res1 = sim.Create<Resistor>(1000);
			var cap0 = sim.Create<CapacitorElm>(1E-7);
			var opAmp0 = sim.Create<OpAmp>();
			var grnd0 = sim.Create<Ground>();

			sim.Connect(square0.leadPos, res0.leadIn);
			sim.Connect(square0.leadPos, res1.leadIn);
			
			sim.Connect(opAmp0.leadNeg, res1.leadOut);
			sim.Connect(opAmp0.leadNeg, opAmp0.leadOut);

			sim.Connect(opAmp0.leadPos, res0.leadOut);
			sim.Connect(opAmp0.leadPos, cap0.leadIn);

			sim.Connect(cap0.leadOut, grnd0.leadIn);

			var square1 = sim.Create<VoltageInput>(Voltage.WaveType.SQUARE);
			square1.frequency = 30;

			var cap1 = sim.Create<CapacitorElm>(1E-5);
			var res2 = sim.Create<Resistor>(1000);
			var grnd1 = sim.Create<Ground>();

			sim.Connect(square1.leadPos, cap1.leadIn);
			sim.Connect(cap1.leadOut, res2.leadIn);
			sim.Connect(res2.leadOut, grnd1.leadIn);

			var scope0 = sim.Watch(cap0);
			var scope1 = sim.Watch(cap1);

			double cycleTime = 1 / square0.frequency;
			double quarterCycleTime = cycleTime / 4;
			int steps = (int)(cycleTime / sim.timeStep);
			sim.doTicks(steps);

			/*Assert.AreEqual( 4.06, Math.Round(scope0.Max((f) => f.voltage), 2));
			Assert.AreEqual(-3.29, Math.Round(scope0.Min((f) => f.voltage), 2));
			Assert.AreEqual( 4.06, Math.Round(scope1.Max((f) => f.voltage), 2));
			Assert.AreEqual(-3.29, Math.Round(scope1.Min((f) => f.voltage), 2));

			Assert.AreEqual( 0.004999, Math.Round(scope0.Max((f) => f.current) * 100, 6));
			Assert.AreEqual(-0.009054, Math.Round(scope0.Min((f) => f.current) * 100, 6));
			Assert.AreEqual( 0.004999, Math.Round(scope1.Max((f) => f.current), 6));
			Assert.AreEqual(-0.009054, Math.Round(scope1.Min((f) => f.current), 6));*/

			for(int x = 0; x < scope0.Count; x++) {
				Assert.AreEqual(Math.Round(scope0[x].voltage, 6), Math.Round(scope1[x].voltage, 6));
				Assert.AreEqual(Math.Round(scope0[x].current * 100, 6), Math.Round(scope1[x].current, 6));
			}

			scope0.Clear();
			scope1.Clear();
			sim.doTicks(steps);

			/*Assert.AreEqual( 3.43, Math.Round(scope0.Max((f) => f.voltage), 2));
			Assert.AreEqual(-3.41, Math.Round(scope0.Min((f) => f.voltage), 2));
			Assert.AreEqual( 3.43, Math.Round(scope1.Max((f) => f.voltage), 2));
			Assert.AreEqual(-3.41, Math.Round(scope1.Min((f) => f.voltage), 2));

			Assert.AreEqual( 0.008287, Math.Round(scope0.Max((f) => f.current) * 100, 6));
			Assert.AreEqual(-0.008432, Math.Round(scope0.Min((f) => f.current) * 100, 6));
			Assert.AreEqual( 0.008287, Math.Round(scope1.Max((f) => f.current), 6));
			Assert.AreEqual(-0.008432, Math.Round(scope1.Min((f) => f.current), 6));*/

			for(int x = 0; x < scope0.Count; x++) {
				Assert.AreEqual(Math.Round(scope0[x].voltage, 6), Math.Round(scope1[x].voltage, 6));
				Assert.AreEqual(Math.Round(scope0[x].current * 100, 6), Math.Round(scope1[x].current, 6));
			}

		}

	}
}
