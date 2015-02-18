using System;
using System.Collections;
using System.Collections.Generic;

namespace SharpCircuit {

	public abstract class ChipElm : CircuitElement {

		protected int bits;
		protected Pin[] pins;
		protected bool lastClock;

		public ChipElm() : base() {
			if(needsBits())
				bits = (this is DecadeElm) ? 10 : 4;
			setupPins();
		}

		public virtual void setupPins() { }
		public virtual void execute(Circuit sim) { }

		public virtual bool needsBits() {
			return false;
		}

		public override void setVoltageSource(int j, int vs) {
			for(int i = 0; i != getLeadCount(); i++) {
				Pin p = pins[i];
				if(p.output && j-- == 0)
					p.voltSource = vs;
			}
		}

		public override void stamp(Circuit sim) {
			for(int i = 0; i != getLeadCount(); i++) {
				Pin p = pins[i];
				if(p.output)
					sim.stampVoltageSource(0, lead_node[i], p.voltSource);
			}
		}

		public override void step(Circuit sim) {
			for(int i = 0; i != getLeadCount(); i++) {
				Pin p = pins[i];
				if(!p.output)
					p.value = lead_volt[i] > 2.5;
			}

			execute(sim);

			for(int i = 0; i != getLeadCount(); i++) {
				Pin p = pins[i];
				if(p.output)
					sim.updateVoltageSource(0, lead_node[i], p.voltSource, p.value ? 5 : 0);
			}
		}

		public override void reset() {
			for(int i = 0; i != getLeadCount(); i++) {
				pins[i].value = false;
				lead_volt[i] = 0;
			}
			lastClock = false;
		}

		public override void getInfo(String[] arr) {
			arr[0] = getChipName();
			int a = 1;
			for(int i = 0; i != getLeadCount(); i++) {

				Pin p = pins[i];
				if(arr[a] != null) {
					arr[a] += "; ";
				} else {
					arr[a] = "";
				}

				String t = "";
				if(p.lineOver) t += '\'';
				if(p.clock) t = "Clk";
				arr[a] += t + " = " + getVoltageText(lead_volt[i]);
				if(i % 2 == 1)
					a++;
			}
		}

		public override void setCurrent(int x, double c) {
			for(int i = 0; i != getLeadCount(); i++) {
				if(pins[i].output && pins[i].voltSource == x)
					pins[i].current = c;
			}
		}

		public virtual String getChipName() {
			return "chip";
		}

		public override bool leadsAreConnected(int n1, int n2) {
			return false;
		}

		public override bool leadIsGround(int n1) {
			return pins[n1].output;
		}

		public class Pin {

			public Pin(String nm) {
				name = nm;
			}

			public string name;

			public int voltSource;
			public bool lineOver, clock, output, value;
			public double current;
		}

	}
}