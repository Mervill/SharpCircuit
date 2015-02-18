using System;
using System.Collections.Generic;
using System.Linq;

using SharpCircuit;
using ServiceStack.Text;
using NUnit.Framework;

namespace SharpCircuitTest {
	
	[TestFixture]
	public class DiodeTest {

		[Test]
		public void SimpleDiodeTest() {
			Circuit sim = new Circuit();

			var voltage0 = sim.Create<ACRailElm>();
			var diode = sim.Create<DiodeElm>();
			var ground = sim.Create<GroundElm>();

			sim.Connect(voltage0.leadOut, diode.leadIn);
			sim.Connect(diode.leadOut, ground.leadIn);

			var diodeScope = sim.Watch(diode);

			double cycleTime = 1 / voltage0.frequency;
			double quarterCycleTime = cycleTime / 4;

			int steps = (int)(cycleTime / sim.timeStep);
			for(int x = 1; x <= steps; x++)
				sim.update(sim.timeStep);

			double voltageHigh = diodeScope.Max((f) => f.voltage);
			int voltageHighNdx = diodeScope.FindIndex((f) => f.voltage == voltageHigh);

			TestUtils.Compare(voltageHigh, voltage0.dutyCycle, 4);
			TestUtils.Compare(diodeScope[voltageHighNdx].time, quarterCycleTime, 4);

			double voltageLow = diodeScope.Min((f) => f.voltage);
			int voltageLowNdx = diodeScope.FindIndex((f) => f.voltage == voltageLow);

			TestUtils.Compare(voltageLow, -voltage0.dutyCycle, 4);
			TestUtils.Compare(diodeScope[voltageLowNdx].time, quarterCycleTime * 3, 4);

			double currentHigh = diodeScope.Max((f) => f.current);
			int currentHighNdx = diodeScope.FindIndex((f) => f.current == currentHigh);
			TestUtils.Compare(diodeScope[voltageHighNdx].time, diodeScope[currentHighNdx].time, 5);

			double currentLow = diodeScope.Min((f) => f.current);
			int currentLowNdx = diodeScope.FindIndex((f) => f.current == currentLow);

			TestUtils.Compare(currentLow, 0, 8);
		}

		[Test]
		public void HalfWaveRectifierTest(){

			/*string nm = TestContext.CurrentContext.Test.Name;
			string js = System.IO.File.ReadAllText(string.Format("./{0}.json", nm));
			Circuit sim = JsonSerializer.DeserializeFromString<Circuit>(js);
			sim.needAnalyze();

			var source0 = sim.getElm(0) as VoltageElm;
			var sourceScope = sim.Watch(sim.getElm(0));
			var resScope = sim.Watch(sim.getElm(2));*/

			Circuit sim = new Circuit();
			
			VoltageElm voltage0 = sim.Create<VoltageElm>(VoltageElm.WaveType.AC);
			DiodeElm diode0 = sim.Create<DiodeElm>();
			ResistorElm res0 = sim.Create<ResistorElm>(640);
			WireElm wire0 = sim.Create<WireElm>();

			sim.Connect(voltage0, 1, diode0, 0);
			sim.Connect(diode0, 1, res0, 0);
			sim.Connect(res0, 1, wire0, 0);
			sim.Connect(wire0, 1, voltage0, 0);

			var voltScope = sim.Watch(voltage0);
			var resScope = sim.Watch(res0);

			double cycleTime = 1 / voltage0.frequency;
			double quarterCycleTime = cycleTime / 4;

			int steps = (int)(cycleTime / sim.timeStep);
			for(int x = 1; x <= steps; x++)
				sim.update(sim.timeStep);

			// A/C Voltage Source
			{
				double voltageHigh = voltScope.Max((f) => f.voltage);
				int voltageHighNdx = voltScope.FindIndex((f) => f.voltage == voltageHigh);

				TestUtils.Compare(voltageHigh, voltage0.dutyCycle, 4);
				TestUtils.Compare(voltScope[voltageHighNdx].time, quarterCycleTime, 4);

				double voltageLow = voltScope.Min((f) => f.voltage);
				int voltageLowNdx = voltScope.FindIndex((f) => f.voltage == voltageLow);

				TestUtils.Compare(voltageLow, -voltage0.dutyCycle, 4);
				TestUtils.Compare(voltScope[voltageLowNdx].time, quarterCycleTime * 3, 4);
			}

			// Resistor
			{
				double voltageHigh = resScope.Max((f) => f.voltage);
				int voltageHighNdx = resScope.FindIndex((f) => f.voltage == voltageHigh);

				TestUtils.Compare(resScope[voltageHighNdx].time, quarterCycleTime, 4);

				double voltageLow = resScope.Min((f) => f.voltage);
				int voltageLowNdx = resScope.FindIndex((f) => f.voltage == voltageLow);

				TestUtils.Compare(voltageLow, 0, 8);
			}

			/*string js = JsonSerializer.SerializeToString(sim);
			string nm = TestContext.CurrentContext.Test.Name;
			Debug.Log(nm);
			Debug.Log(js);
			System.IO.File.WriteAllText(string.Format("./{0}.json", nm), js);*/
		}

		[Test]
		public void FullWaveRectifierTest(){
			Assert.Fail();
		}

	}
}
