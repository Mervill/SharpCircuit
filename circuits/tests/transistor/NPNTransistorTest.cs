using System;
using System.Collections;
using System.Collections.Generic;

namespace Circuits.Tests {

	public class NPNTransistorTest : CircuitTest {

		public NPNTransistorElm transistor;
		public VarRailElm baseVoltage;
		public VarRailElm collectorVoltage;
		public GroundElm ground;

		public NPNTransistorTest() {

			baseVoltage = new VarRailElm(sim);
			baseVoltage.maxVoltage = 0.7025;
			
			collectorVoltage = new VarRailElm(sim);
			collectorVoltage.maxVoltage = 2;
			
			ground = new GroundElm(sim);
			
			transistor = new NPNTransistorElm(sim);
			
			baseVoltage.Attach(0,transistor.leadBase);
			collectorVoltage.Attach(0,transistor.leadCollector);
			ground.Attach(0,transistor.leadEmitter);
			
			sim.needAnalyze();
		}

	}


}