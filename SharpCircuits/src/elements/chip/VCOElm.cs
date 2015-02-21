using System;
using System.Collections;
using System.Collections.Generic;

namespace SharpCircuit {

	public class VCOElm : ChipElm {
		
		public VCOElm() : base() {
			
		}

		public override String getChipName() {
			return "VCO";
		}

		public override void setupPins() {
			pins = new Pin[6];
			pins[0] = new Pin("Vi");
			pins[1] = new Pin("Vo");
			pins[1].output = true;
			pins[2] = new Pin("C");
			pins[3] = new Pin("C");
			pins[4] = new Pin("R1");
			pins[4].output = true;
			pins[5] = new Pin("R2");
			pins[5].output = true;
		}

		public override bool nonLinear() { return true; }

		public override void stamp(Circuit sim) {
			// output pin
			sim.stampVoltageSource(0, lead_node[1], pins[1].voltSource);
			// attach Vi to R1 pin so its current is proportional to Vi
			sim.stampVoltageSource(lead_node[0], lead_node[4], pins[4].voltSource, 0);
			// attach 5V to R2 pin so we get a current going
			sim.stampVoltageSource(0, lead_node[5], pins[5].voltSource, 5);
			// put resistor across cap pins to give current somewhere to go
			// in case cap is not connected
			sim.stampResistor(lead_node[2], lead_node[3], cResistance);
			sim.stampNonLinear(lead_node[2]);
			sim.stampNonLinear(lead_node[3]);
		}

		public double cResistance = 1e6;
		public double cCurrent;
		public int cDir;

		public override void step(Circuit sim) {
			double vc = lead_volt[3] - lead_volt[2];
			double vo = lead_volt[1];
			int dir = (vo < 2.5) ? 1 : -1;
			// switch direction of current through cap as we oscillate
			if (vo < 2.5 && vc > 4.5) {
				vo = 5;
				dir = -1;
			}
			if (vo > 2.5 && vc < .5) {
				vo = 0;
				dir = 1;
			}

			// generate output voltage
			sim.updateVoltageSource(0, lead_node[1], pins[1].voltSource, vo);
			// now we set the current through the cap to be equal to the
			// current through R1 and R2, so we can measure the voltage
			// across the cap
			int cur1 = sim.nodeCount + pins[4].voltSource;
			int cur2 = sim.nodeCount + pins[5].voltSource;
			sim.stampMatrix(lead_node[2], cur1, dir);
			sim.stampMatrix(lead_node[2], cur2, dir);
			sim.stampMatrix(lead_node[3], cur1, -dir);
			sim.stampMatrix(lead_node[3], cur2, -dir);
			cDir = dir;
		}

		// can't do this in calculateCurrent() because it's called before
		// we get pins[4].current and pins[5].current, which we need
		public void computeCurrent() {
			if (cResistance == 0) {
				return;
			}
			double c = cDir * (pins[4].current + pins[5].current)
					+ (lead_volt[3] - lead_volt[2]) / cResistance;
			pins[2].current = -c;
			pins[3].current = c;
			pins[0].current = -pins[4].current;
		}

		public override int getLeadCount() {
			return 6;
		}

		public override int getVoltageSourceCount() {
			return 3;
		}

	}
}