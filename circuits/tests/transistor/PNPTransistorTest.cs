using System;
using System.Collections;
using System.Collections.Generic;

namespace Circuits.Tests {

	public class PNPTransistorTest : CircuitTest {

		public PNPTransistorElm transistor;
		public VarRailElm baseVoltage;
		public VarRailElm collectorVoltage;
		public VarRailElm emitterVoltage;

		public PNPTransistorTest() {

			baseVoltage = new VarRailElm(sim);
			baseVoltage.maxVoltage = 1.3;
			
			collectorVoltage = new VarRailElm(sim);
			collectorVoltage.maxVoltage = 0;

			emitterVoltage = new VarRailElm(sim);
			emitterVoltage.maxVoltage = 2;

			transistor = new PNPTransistorElm(sim);
			
			baseVoltage.Attach(0,transistor.leadBase);
			collectorVoltage.Attach(0,transistor.leadCollector);
			emitterVoltage.Attach(0,transistor.leadEmitter);
			
			sim.needAnalyze();
		}

	}


}