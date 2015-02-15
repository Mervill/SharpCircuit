using System;
using System.Linq;
using System.Text;
using System.Collections;
using System.Collections.Generic;

using ServiceStack.Text;

namespace SharpCircuit {
	
	class Program {

		static void Main(string[] args) {

			CirSim sim = new CirSim();
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
			for(int x = 1; x <= steps; x++) {
				sim.update(x);
				string utime = CircuitElement.getUnitText(sim.time, "s");
				Debug.LogF(" [{0,5}, {1,8}]", x, utime);
			}
			Debug.Log(Math.Round(sim.time, 4));

			Debug.Log(Math.Round(baseWire.getCurrent(), 8));
			Debug.Log(Math.Round(collectorWire.getCurrent(), 8));
			Debug.Log(Math.Round(emitterWire.getCurrent(), 8));

			//string js_out = JsonSerializer.SerializeToString(sim);
			//System.IO.File.WriteAllText("./out.json", js_out);

			string js_in = System.IO.File.ReadAllText("./out.json");
			sim = JsonSerializer.DeserializeFromString<CirSim>(js_in);
			sim.needAnalyze();
			sim.update(1);

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