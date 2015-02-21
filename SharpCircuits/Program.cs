using System;
using System.Linq;
using System.Text;
using System.Collections;
using System.Collections.Generic;

using ServiceStack.Text;

namespace SharpCircuit {
	
	class Program {

		public static double Round(double val, int places) {
			if(places < 0) throw new ArgumentException("places");
			return Math.Round(val - (0.5 / Math.Pow(10, places)), places);
		}

		static void Main(string[] args) {

			// when speed = 172, {100 steps} => [T0.001]

			Circuit sim = new Circuit();
			
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

			double cycleTime = 1 / source0.frequency;
			double quarterCycleTime = cycleTime / 4;
			int steps = (int)(cycleTime / sim.timeStep);
			for(int x = 1; x <= steps; x++) {
				sim.doTick();
				string utime = SIUnits.Normalize(sim.time, "s");
				Debug.LogF(" [{0,4}, {1}]", x, utime);
			}

			Debug.Log();

			int double_acc = 10;

			Debug.Log("A/C Voltage Source");
			{
				double voltageHigh = sourceScope.Max((f) => f.voltage);
				int voltageHighNdx = sourceScope.FindIndex((f) => f.voltage == voltageHigh);
				Debug.LogF("voltageHigh {0,-12} ({1})", Math.Round(voltageHigh, double_acc), voltageHigh);

				double voltageLow = sourceScope.Min((f) => f.voltage);
				int voltageLowNdx = sourceScope.FindIndex((f) => f.voltage == voltageLow);
				Debug.LogF("voltageLow  {0,-12} ({1})", Math.Round(voltageLow, double_acc), voltageLow);

				double currentLow = sourceScope.Min((f) => f.current);
				int currentLowNdx = sourceScope.FindIndex((f) => f.current == currentLow);
				Debug.LogF("currentLow  {0,-12} ({1})", Math.Round(currentLow, double_acc), currentLow);

				double currentHigh = sourceScope.Max((f) => f.current);
				int currentHighNdx = sourceScope.FindIndex((f) => f.current == currentHigh);
				Debug.LogF("currentHigh {0,-12} ({1})", Math.Round(currentHigh, double_acc), currentHigh);
			}

			Debug.Log();

			Debug.Log("Resistor");
			{
				double voltageHigh = resScope.Max((f) => f.voltage);
				int voltageHighNdx = resScope.FindIndex((f) => f.voltage == voltageHigh);
				Debug.LogF("voltageHigh {0,-12} ({1})", Math.Round(voltageHigh, double_acc), voltageHigh);

				double voltageLow = resScope.Min((f) => f.voltage);
				int voltageLowNdx = resScope.FindIndex((f) => f.voltage == voltageLow);
				Debug.LogF("voltageLow  {0,-12} ({1})", Math.Round(voltageLow, double_acc), voltageLow);

				double currentLow = resScope.Min((f) => f.current);
				int currentLowNdx = resScope.FindIndex((f) => f.current == currentLow);
				Debug.LogF("currentLow  {0,-12} ({1})", Math.Round(currentLow, double_acc), currentLow);

				double currentHigh = resScope.Max((f) => f.current);
				int currentHighNdx = resScope.FindIndex((f) => f.current == currentHigh);
				Debug.LogF("currentHigh {0,-12} ({1})", Math.Round(currentHigh, double_acc), currentHigh);
			}

			Debug.Log();

			/*JsConfig.ExcludeTypes.Add(typeof(Circuit.Lead));
			System.IO.File.WriteAllText("./out.json", JsonSerializer.SerializeToString(sim));

			string json = System.IO.File.ReadAllText("./out.json");
			Circuit sim2 = JsonSerializer.DeserializeFromString<Circuit>(json);
			sim2.resetTime();
			sim2.analyze();

			sourceScope = sim2.Watch(sim2.getElm(0));
			
			for(int x = 1; x <= steps; x++) {
				sim2.ticks();
				string utime = SIUnits.Normalize(sim2.time, "s");
				Debug.LogF(" [{0,4}, {1}]", x, utime);
			}

			double voltageHigh0 = sourceScope.Max((f) => f.voltage);
			int voltageHighNdx0 = sourceScope.FindIndex((f) => f.voltage == voltageHigh0);
			Debug.LogF("voltageHigh {0,-12} ({1})", Math.Round(voltageHigh0, double_acc), voltageHigh0);*/

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