using System;
using System.Linq;
using System.Text;
using System.Collections;
using System.Collections.Generic;

using ServiceStack.Text;

namespace SharpCircuit {
	
	class Program {

		static void Main(string[] args) {

			// when speed = 172, {100 steps} => [T0.001]

			Circuit sim = new Circuit();
			sim.timeStep = 1E-14;

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

			int steps = 5;
			for(int x = 1; x <= steps; x++) {
				sim.update(x);
				string utime = CircuitElement.getUnitText(sim.time, "s");
				Debug.LogF(" [{0,5}, {1,8}]", x, utime);
			}
				

			// A/C Voltage Source
			{
				double voltageHigh = sourceScope.Max((f) => f.voltage);
				int voltageHighNdx = sourceScope.FindIndex((f) => f.voltage == voltageHigh);
				Debug.Log(Math.Round(voltageHigh, 4), "voltageHigh");

				double voltageLow = sourceScope.Min((f) => f.voltage);
				int voltageLowNdx = sourceScope.FindIndex((f) => f.voltage == voltageLow);
				Debug.Log(Math.Round(voltageLow, 4), "voltageLow");

				//Assert.AreEqual(208 * 2, voltageLowNdx - voltageHighNdx);
				//Assert.AreEqual(208 * 3, voltageLowNdx);

				double currentHigh = sourceScope.Max((f) => f.current);
				int currentHighNdx = sourceScope.FindIndex((f) => f.current == currentHigh);
				Debug.Log(currentHighNdx, "currentHighNdx");

				double currentLow = sourceScope.Min((f) => f.current);
				int currentLowNdx = sourceScope.FindIndex((f) => f.current == currentLow);
				Debug.Log(Math.Round(currentLow, 4), "currentLow");
			}

			// Resistor
			{
				double voltageHigh = resScope.Max((f) => f.voltage);
				int voltageHighNdx = resScope.FindIndex((f) => f.voltage == voltageHigh);
				Debug.Log(Math.Round(voltageHigh, 4), "voltageHigh");

				double voltageLow = resScope.Min((f) => f.voltage);
				int voltageLowNdx = resScope.FindIndex((f) => f.voltage == voltageLow);
				Debug.Log(Math.Round(voltageLow, 4), "voltageLow");

				double currentLow = resScope.Min((f) => f.current);
				int currentLowNdx = resScope.FindIndex((f) => f.current == currentLow);
				Debug.Log(Math.Round(currentLow, 4), "currentLow");

				double currentHigh = resScope.Max((f) => f.current);
				int currentHighNdx = resScope.FindIndex((f) => f.current == currentHigh);
				Debug.Log(currentHighNdx, "currentHighNdx");
			}

			Debug.Log();
			Debug.Log(sim.speed);
			Debug.Log(sim.time, CircuitElement.getUnitText(sim.time, "s"));
			//Debug.Log(CircuitElement.getUnitText(sim.timeStep, "s"));
			
			Console.WriteLine("program complete");
			Console.ReadLine();
		}

	}
}

public static class Debug {

	public static void Log(params object[] objs) {
		StringBuilder sb = new StringBuilder();
		foreach(object o in objs)
			sb.Append(o.ToString()).Append(" ");
		Console.WriteLine(sb.ToString());
	}

	public static void LogF(string format, params object[] objs) {
		Console.WriteLine(string.Format(format, objs));
	}

}