FalstadCircuitsSharp
=======

This is a C# port of Paul Falstad's excellent [circuit simulator](http://www.falstad.com/circuit/) applet. 

The library is in a very early stage. I've finished a first pass on all the original code, but there's still lots of work to be done so **expect changes to the core api**. The main tasks are to finish removing UI functions and to organize and simplify the front-facing api.

Most of the circuit elements are untested, but initial testing suggests that the majority of the circuits work exactly as they did in the applet. But until further notice, your mileage may vary.

Licence: MIT/Boost C++

## ToDo

- Access modifiers are a mess, determine what functions need to be public and what should be made protected or private.
- Same thing goes for properties, look at the old option screen code for direction on what variables should be exposed as properties.
- Remove dependants on the Point class, the library itself should not care about the spacial relationship between elements and their lead points.
- Ensure nothing relies on static reference objects / singletons.
- Finish removing all the vestigial rendering code so we can use it as a library.
- The API is an absolute mess, improve usability! There needs to be an easy way to connect two elements.

**API notes**

- Create verbose properties for element leads where plausible, for example a transistor should have 3 properties: base, collector and emitter, that can be used to simplify connecting elements.

```csharp

// Desired API usage
acSource.Attach<Resistor>().Attach<Inductor>().WireTo(acSource);

// ... would create the circuit described in the first example below

// Create
sim.Create<T>(params args) where T : CircuitElm

elm.Attach<T>(); // Create a new T and attach to first open lead
elm.Attach<T>(params args); // Create a new T and pass args to the constructor
elm.Attach<T>([int lead / string lead]); // Create a new T and connect to a specific lead
elm.Attach<T>([int lead / string lead],params args); // combined

// Also do Attach(Type typ, ...) versions

elm.Attach([int lead / string lead],CircuitElm elm); // Add elm to the sim if it doesn't exist yet

// WireTo should the same thing as Attach(), but use a wire in between the two leads
elm.WireTo<T>([int lead / string lead],params args);

// If attach or WireTo are called on an element, use the first open node,
// otherwise Attach and WireTo can be called on specific leads

elm.Attach<T>();
elm.lead.Attach(); // Named lead
elm.GetLead(0).Attach();
elm.GetLead("base").Attach();

// Perhaps??
elm["base"].Attach()
elm[0].Attach()

```

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

using Circuits;

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

		// Give these two elements absolute positions.
		elements.Add(ACSource = new ACVoltageElm(50,50,sim));
		elements.Add(Inductor = new InductorElm(60,50,sim));

		// Wire these elements relative to the above elements.
		elements.Add(Resistor = new ResistorElm(0,0,sim){
			point1 = ACSource.point1,
			point2 = Inductor.point1,
			resistance = 100
		});
		
		elements.Add(wire = new WireElm(0,0,sim){
			point1 = Inductor.point2,
			point2 = ACSource.point2
		});
		
		foreach(CircuitElm elm in elements){
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

## Licence

The applet source code [[download](http://www.falstad.com/circuit/src.zip)] does not specify a particular licence. Since the code is freely downloadable from the website I've decided to licence the code under the Boost variant of the MIT Licence.

```
FalstadCircuitsSharp (c) 2014 Riley 'Mervill' Godard

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
