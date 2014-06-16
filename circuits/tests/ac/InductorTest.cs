using System;
using System.Collections;
using System.Collections.Generic;

namespace Circuits.Tests {

	public class InductorTest : CircuitTest {

		//
		// AC voltage source connected to a resistor
		// and an inductor. The current and delta voltage
		// of the inductor will oscillate between positive
		// and negative as the AC voltage cycles the direction
		// of the current.
		// 

		public ACVoltageElm acSource;
		public ResistorElm resistorA;
		public InductorElm inductor;
		public WireElm wire;

		public InductorTest() {

			acSource = new ACVoltageElm(sim);
			resistorA = new ResistorElm(sim){
				resistance = 180
			};
			inductor = new InductorElm(sim);
			wire = new WireElm(sim);
			
			acSource
				.Next(resistorA)
				.Next(inductor)
				.Next(wire)
				.Next(acSource);
			
			sim.needAnalyze();
		}

	}


}