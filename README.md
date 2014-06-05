FalstadCircutsSharp
=======

This is a C# port of Paul Falstad's excellent [circuit simulator](http://www.falstad.com/circuit/) applet. 

The library is in a very early stage. I've finished a first pass on all the original code, but there's still lots of work to be done so **expect changes to the core api**. The main tasks are to finish removing UI functions and to organize and simplify the front-facing api.

Most of the circuit elements are untested, but initial testing suggests that the majority of the circuits work exactly as they did in the applet. But until further notice, your mileage may vary.

Licence: MIT/Boost C++

## ToDo

- Access modifiers are a mess, determine what functions need to be public and what should be made protected or private.
- Same thing goes for properties, look at the old option screen code for direction on what variables should be exposed as properties.
- Finish removing all the vestigial rendering code so we can use it as a library.
- The API is an absolute mess, improve usability! There needs to be an easy way to connect two elements.

## Tested Elements

The following elements have been tested:

- Wire
- ACVoltage
- Resistor
- Inductor
- VarRail (single-terminal DC voltage source)

## Examples

```csharp
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Circuts;

public class Example {
	
    //
	// AC voltage source connected to a resistor
	// and an inductor. The current and delta voltage
	// of the inductor will oscillate between positive
	// and negative as the AC voltage cycles the direction
	// of the current.
	// 
    
	CirSim sim;
	
	ACVoltageElm 	ACSource;
	ResistorElm 	Resistor;
	InductorElm 	Inductor;
	WireElm 		wire;
	
	void Init(){
		
		sim = new CirSim();
		
		List<CircuitElm> elements = new List<CircuitElm>();
		
		elements.Add(ACSource = new ACVoltageElm(5,5,sim){
			x2 = 5,
			y2 = 10
		});
		
		elements.Add(Resistor = new ResistorElm(5,5,sim){
			x2 = 10,
			y2 = 5,
			resistance = 180
		});

		elements.Add(Inductor = new InductorElm(10,5,sim){
			x2 = 10,
			y2 = 10
		});
		
		elements.Add(wire = new WireElm(10,10,sim){
			x2 = 5,
			y2 = 10
		});
		
		foreach(CircuitElm elm in elements){
			elm.setPoints();
			sim.elmList.Add(elm);
		}

		sim.analyzeFlag = true;
		sim.stoppedCheck = false;
	}

	void Tick(){
		sim.updateCircuit();
		if(sim.stoppedCheck){
			Console.WriteLine(sim.stopMessage);
		}
	}
}
```

```csharp
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Circuts;

public class Example {
	
	//
	// Trivial example of Ohm's law. Two resistors are
	// connected to a single-terminal voltage source and
	// then to ground. The current in scope2 should be
	// 10 times larger then then current in scope1.
	//
	// You can use element.getVoltageText() to get 
	// a nicely formatted current output.
	//
	
	CirSim sim;
	
	VarRailElm 		VoltageSource;
	ResistorElm 	ResistorA;
	ResistorElm 	ResistorB;
	
	Wire scope1;
	Wire scope2;
	
	void Init(){
		
		sim = new CirSim();
		
		List<CircuitElm> elements = new List<CircuitElm>();
		
		elements.Add(VoltageSource = new VarRailElm(0,5,sim){
			x2 = 0,
			y2 = 0,
			slider = 100
		});
		
		elements.Add(new WireElm(0,5,sim){
			x2 = 0,
			y2 = 10
		});
		
		elements.Add(ResistorA = new ResistorElm(0,10,sim){
			x2 = 0,
			y2 = 15,
			resistance = 100
		});
		
		elements.Add(scope1 = new WireElm(0,15,sim){
			x2 = 0,
			y2 = 20
		});
		
		elements.Add(new GroundElm(0,20,sim){
			x2 = 0,
			y2 = 25
		});

		wires.Add(new WireElm(0,5,sim){
			x2 = 5,
			y2 = 10
		});

		elements.Add(ResistorB = new ResistorElm(5,10,sim){
			x2 = 5,
			y2 = 15,
			resistorB.resistance  = 1000
		});
		
		elements.Add(scope2 = new WireElm(5,15,sim){
			x2 = 5,
			y2 = 20
		});
		
		elements.Add(new GroundElm(5,20,sim){
			x2 = 5,
			y2 = 25
		});
		
		foreach(CircuitElm elm in elements){
			elm.setPoints();
			sim.elmList.Add(elm);
		}

		sim.analyzeFlag = true;
		sim.stoppedCheck = false;
	}

	void Tick(){
		sim.updateCircuit();
		if(sim.stoppedCheck){
			Debug.LogWarning(sim.stopMessage);
		}
	}
}
```

## Licence

The applet source code [[download](http://www.falstad.com/circuit/src.zip)] does not spesify a paticular licence. Since the code is freely downloadable from the website I've decided to licence the code under the Boost variant of the MIT Licence.

```
FalstadCircutsSharp (c) 2014 Riley 'Mervill' Godard

Java -> C# language conversion project based almost entirely on:

CirSim.java (c) 2010 by Paul Falstad
http://www.falstad.com/circuit/

Boost Software License - Version 1.0 - August 17, 2003
 
Permission is hereby granted, free of charge, to any person or organization
obtaining a copy of the software and accompanying documentation covered by
this license (the "Software") to use, reproduce, display, distribute,
execute, and transmit the Software, and to prepare [[derivative work]]s of the
Software, and to permit third-parties to whom the Software is furnished to
do so, all subject to the following:
 
The copyright notices in the Software and this entire statement, including
the above license grant, this restriction and the following disclaimer,
must be included in all copies of the Software, in whole or in part, and
all derivative works of the Software, unless such copies or derivative
works are solely in the form of machine-executable object code generated by
a source language processor.
 
THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE, TITLE AND NON-INFRINGEMENT. IN NO EVENT
SHALL THE COPYRIGHT HOLDERS OR ANYONE DISTRIBUTING THE SOFTWARE BE LIABLE
FOR ANY DAMAGES OR OTHER LIABILITY, WHETHER IN CONTRACT, TORT OR OTHERWISE,
ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
DEALINGS IN THE SOFTWARE.
```