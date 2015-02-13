using System;
using System.Collections.Generic;
using System.Linq;

using SharpCircuit;
using NUnit.Framework;

namespace SharpCircuitTest {
	
	[TestFixture]
	public class DiodeTest {

		[Test]
		public void SimpleDiodeTest() {
			Assert.Fail();
		}

		[Test]
		public void HalfWaveRectifierTest(){

			CirSim sim = new CirSim();
			sim.speed = 200;

			VoltageElm source0 = sim.Create<VoltageElm>(VoltageElm.WaveType.AC);
			DiodeElm diode0 = sim.Create<DiodeElm>();
			ResistorElm res0 = sim.Create<ResistorElm>(640);
			WireElm wire0 = sim.Create<WireElm>();

			sim.Connect(source0, 1, diode0, 0);
			sim.Connect(diode0, 1, res0, 0);
			sim.Connect(res0, 1, wire0, 0);
			sim.Connect(wire0, 1, source0, 0);

			var sourceScope = sim.Watch(source0);
			var resScope = sim.Watch(res0);

			int steps = 208 * 4;
			for(int x = 1; x <= steps; x++)
				sim.update(x);

			// A/C Voltage Source
			{
				double voltageHigh = sourceScope.Max((f) => f.voltage);
				int voltageHighNdx = sourceScope.FindIndex((f) => f.voltage == voltageHigh);
				Assert.AreEqual(source0.dutyCycle, Math.Round(voltageHigh, 4), "Didn't maintain duty cycle (+).");
			
				double voltageLow = sourceScope.Min((f) => f.voltage);
				int voltageLowNdx = sourceScope.FindIndex((f) => f.voltage == voltageLow);
				Assert.AreEqual(-source0.dutyCycle, Math.Round(voltageLow, 4), "Didn't maintain duty cycle (-).");

				Assert.AreEqual(208 * 2, voltageLowNdx - voltageHighNdx);
				Assert.AreEqual(208 * 3, voltageLowNdx);
			
				double currentHigh = sourceScope.Max((f) => f.current);
				int currentHighNdx = sourceScope.FindIndex((f) => f.current == currentHigh);
				Assert.AreEqual(voltageHighNdx, currentHighNdx, "Voltage and current do not reach maximum value on the same frame.");
			
				double currentLow = sourceScope.Min((f) => f.current);
				int currentLowNdx = sourceScope.FindIndex((f) => f.current == currentLow);
				Assert.AreEqual(0, Math.Round(currentLow, 4), "Current drops below zero.");
			}

			// Resistor
			{
				double voltageHigh = resScope.Max((f) => f.voltage);
				int voltageHighNdx = resScope.FindIndex((f) => f.voltage == voltageHigh);
				Assert.AreEqual(4.319, Math.Round(voltageHigh, 4), "Didn't caclulate correct resistance.");

				double voltageLow = resScope.Min((f) => f.voltage);
				int voltageLowNdx = resScope.FindIndex((f) => f.voltage == voltageLow);
				Assert.AreEqual(0, Math.Round(voltageLow, 4), "Voltage drops below zero.");

				double currentLow = resScope.Min((f) => f.current);
				int currentLowNdx = resScope.FindIndex((f) => f.current == currentLow);
				Assert.AreEqual(0, Math.Round(currentLow, 4), "Current drops below zero.");

				double currentHigh = resScope.Max((f) => f.current);
				int currentHighNdx = resScope.FindIndex((f) => f.current == currentHigh);
				Assert.AreEqual(voltageHighNdx, currentHighNdx, "Voltage and current do not reach max value on the same frame.");
			}


		}

		[Test]
		public void FullWaveRectifierTest(){
			Assert.Fail();
		}

	}
}
