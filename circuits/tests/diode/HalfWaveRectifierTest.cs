using System;
using System.Collections;
using System.Collections.Generic;

namespace Circuits.Tests {

	public class HalfWaveRectifierTest : CircuitTest {

		//
		// Half-Wave Rectifier
		// This circuit uses a diode, a device that conducts 
		// current in only one direction. It takes AC input 
		// and "rectifies" it so that the negative portion of the 
		// output is removed.
		// 

		public ACVoltageElm acSource;
		public ResistorElm resistorA;
		public DiodeElm diode;
		public WireElm wire;

		public HalfWaveRectifierTest() {

			acSource = new ACVoltageElm(sim);
			resistorA = new ResistorElm(sim){
				resistance = 640
			};
			diode = new DiodeElm(sim);
			wire = new WireElm(sim);
			
			acSource
				.Next(diode)
				.Next(resistorA)
				.Next(wire)
				.Next(acSource);
			
			sim.needAnalyze();
		}

	}


}