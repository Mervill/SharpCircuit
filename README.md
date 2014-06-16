FalstadCircuitsSharp
=======

This is a C# port of Paul Falstad's excellent [circuit simulator](http://www.falstad.com/circuit/) applet. 

The library is in a very early stage. I've finished a first pass on all the original code, but there's still lots of work to be done so **expect changes to the core api**. The main tasks are to finish removing UI functions and to organize and simplify the front-facing api.

Most of the circuit elements are untested, but initial testing suggests that the majority of the circuits work exactly as they did in the applet. But until further notice, your mileage may vary.

Licence: MIT/Boost C++

## ToDo

- Go back and make sure all publicly editable variables have proper checks.
- Reorganize leads & give them semantic names.
- Remove the -Elm suffix from circuit elements?
- Add new serialization system (JSON).
- Test everything.
- Fix Scope.cs object and ScopeElm.cs
- Fix momentary switches.
- Externalize deltatime
- LogicInputElm
- Develop testing strategy. 

**API notes**

- Create verbose properties for element leads where plausible, for example a transistor should have 3 properties: base, collector and emitter, that can be used to simplify connecting elements.

```csharp

// Desired API usage
acSource.Attach<Resistor>().Attach<Inductor>().WireTo(acSource);

// ... would create the circuit described in the first example below

// Create
sim.Create<T>(params args) where T : CircuitElm

// Create a new T and connect to a specific lead
elm.Attach<T>([int lead / string lead]);
elm.Attach<T>([int lead / string lead],params args);
// Attach(Type typ, ...)

elm.Attach([int lead / string lead],CircuitElm elm); // Add elm to the sim if it doesn't exist yet

// WireTo should the same thing as Attach(), but use a wire in between the two leads.
elm.WireTo<T>([int lead / string lead]);
elm.WireTo<T>([int lead / string lead],params args);
// WireTo(Type typ, ...)

// If attach or WireTo are called on an element, use the first open node,
// otherwise Attach and WireTo can be called on specific leads

elm.Attach<T>(...);
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
- Diode
- Transistor (NPN/PNP)

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
	
	public ACVoltageElm acSource;
	public ResistorElm resistorA;
	public InductorElm inductor;
	public WireElm wire;
	
	void Init(){
		
		sim = new CirSim();
		
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

using Circuits;

public class Example {
	
    //
	// Half-Wave Rectifier
	// This circuit uses a diode, a device that conducts 
	// current in only one direction. It takes AC input 
	// and "rectifies" it so that the negative portion of the 
	// output is removed.
	// 
    
	CirSim sim;
	
	public ACVoltageElm acSource;
	public ResistorElm resistorA;
	public DiodeElm diode;
	public WireElm wire;
	
	void Init(){
		
		sim = new CirSim();
		
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

using Circuits;

public class Example {
	
    //
	// NPN Transistor
	// Simple example of an NPN Transistor.
	//
	
	CirSim sim;
	
	public NPNTransistorElm transistor;
	public VarRailElm baseVoltage;
	public VarRailElm collectorVoltage;
	public GroundElm ground;
	
	void Init(){
		
		sim = new CirSim();
		
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

	void Tick(){
		sim.updateCircuit();
		if(sim.stoppedCheck){
			Console.WriteLine(sim.stopMessage);
		}
	}
}
```

## Licence

The original applet source code [[download](http://www.falstad.com/circuit/src.zip)] is licensed under Paul Falstad's [Applet Source Licence](http://www.falstad.com/licensing.html). The new API and other improvements are licensed under the Boost Software License.

```
CirSim.java (c) 2010 by Paul Falstad - java@falstad.com
http://www.falstad.com/circuit/

Falstad.com Applet Source Licence.
http://www.falstad.com/licensing.html

You have permission to use these applets in a classroom setting or take screenshots 
as long as the applets are unmodified. Modification or redistribution for non-commercial 
purposes is allowed, as long as you credit me (Paul Falstad) and provide a link to my page 
(the page you found the applet(s) on, or http://www.falstad.com/mathphysics.html). Contact 
me for any other uses. The source code for each applet is generally available on that applet's 
web page, but some of the applets use third-party source code that has restrictions.

THIS SOFTWARE IS PROVIDED ``AS IS'' AND WITHOUT ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, 
WITHOUT LIMITATION, THE IMPLIED WARRANTIES OF MERCHANTIBILITY AND FITNESS FOR A PARTICULAR PURPOSE.
```

```
FalstadCircuitsSharp (c) 2014 Riley 'Mervill' Godard - mervills.email@gmail.com
https://github.com/Mervill/FalstadCircuitsSharp
http://transistorcollective.net/

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
