using System;
using System.Collections.Generic;
using System.Linq;

using SharpCircuit;
using NUnit.Framework;

namespace SharpCircuitTest {

	[TestFixture]
	public class DCVoltageTest {

		[Test]
		public void CapacitorTest() {
			Circuit sim = new Circuit();

			var volt0 = sim.Create<DCVoltageElm>();
			var cap0 = sim.Create<CapacitorElm>(2E-4);
			var res0 = sim.Create<ResistorElm>();

			var switch0 = sim.Create<Switch2Elm>();

			/*sim.Connect(volt0, 1, switch0, 1);
			sim.Connect(switch0, 1, switch0, 1);
			sim.Connect(switch0, 2, res0, 1);
			sim.Connect(cap0, 1, res0, 0);
			sim.Connect(res0, 1, volt0, 0);*/

			// leadNeg 0
			// leadPos 1

			sim.Connect(volt0, 1, switch0, 1);
			sim.Connect(switch0, 0, cap0, 0);
			sim.Connect(cap0, 1, res0, 0);
			sim.Connect(res0, 1, volt0, 0);
			sim.Connect(switch0, 2, volt0, 0);

			switch0.setPosition(0);

			var capScope0 = sim.Watch(cap0);

			for(int x = 1; x <= 28000; x++)
				sim.update(sim.timeStep);

			Debug.LogF("{0} [{1}]", sim.time, CircuitElement.getUnitText(sim.time, "s"));
			{
				double voltageHigh = capScope0.Max((f) => f.voltage);
				int voltageHighNdx = capScope0.FindIndex((f) => f.voltage == voltageHigh);

				Debug.Log("voltageHigh", voltageHigh, voltageHighNdx);

				double voltageLow = capScope0.Min((f) => f.voltage);
				int voltageLowNdx = capScope0.FindIndex((f) => f.voltage == voltageLow);

				Debug.Log("voltageLow ", voltageLow, voltageLowNdx);

				double currentHigh = capScope0.Max((f) => f.current);
				int currentHighNdx = capScope0.FindIndex((f) => f.current == currentHigh);
				Debug.Log("currentHigh", currentHigh, currentHighNdx);

				double currentLow = capScope0.Min((f) => f.current);
				int currentLowNdx = capScope0.FindIndex((f) => f.current == currentLow);

				Debug.Log("currentLow ", currentLow, currentLowNdx);

				Assert.AreEqual(27999, voltageHighNdx);
				Assert.AreEqual(    0, voltageLowNdx);
				
				Assert.AreEqual(    0, currentHighNdx);
				Assert.AreEqual(27999, currentLowNdx);
			}

			switch0.setPosition(1);
			sim.analyze();
			capScope0.Clear();

			for(int x = 1; x <= 28000; x++)
				sim.update(sim.timeStep);

			Debug.Log();

			Debug.LogF("{0} [{1}]", sim.time, CircuitElement.getUnitText(sim.time, "s"));
			{
				double voltageHigh = capScope0.Max((f) => f.voltage);
				int voltageHighNdx = capScope0.FindIndex((f) => f.voltage == voltageHigh);

				Debug.Log("voltageHigh ", voltageHigh, voltageHighNdx);

				double voltageLow = capScope0.Min((f) => f.voltage);
				int voltageLowNdx = capScope0.FindIndex((f) => f.voltage == voltageLow);

				Debug.Log("voltageLow  ", voltageLow, voltageLowNdx);

				double currentHigh = capScope0.Max((f) => f.current);
				int currentHighNdx = capScope0.FindIndex((f) => f.current == currentHigh);
				Debug.Log("currentHigh", currentHigh, currentHighNdx);

				double currentLow = capScope0.Min((f) => f.current);
				int currentLowNdx = capScope0.FindIndex((f) => f.current == currentLow);

				Debug.Log("currentLow ", currentLow, currentLowNdx);

				Assert.AreEqual(voltageHighNdx, currentLowNdx);
				Assert.AreEqual(voltageLowNdx, currentHighNdx);

				Assert.AreEqual(    0, voltageHighNdx);
				Assert.AreEqual(27999, voltageLowNdx);

				Assert.AreEqual(27999, currentHighNdx);
				Assert.AreEqual(    0, currentLowNdx);
			}

		}

	}
}
