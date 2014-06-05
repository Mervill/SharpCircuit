using System;
using System.Collections;
using System.Collections.Generic;

namespace Circuts {

	public class VarRailElm : RailElm {
		public double slider;

		public VarRailElm(int xx, int yy, CirSim s) : base(xx, yy, WF_VAR, s) {
			frequency = maxVoltage;
			waveform = WF_VAR;
		}

		public override double getVoltage() {
			frequency = slider * (maxVoltage - bias) / 100.0 + bias;
			return frequency;
		}

		/*public EditInfo getEditInfo(int n) {
			if (n == 0) {
				return new EditInfo("Min Voltage", bias, -20, 20);
			}
			if (n == 1) {
				return new EditInfo("Max Voltage", maxVoltage, -20, 20);
			}
			if (n == 2) {
				EditInfo ei = new EditInfo("Slider Text", 0, -1, -1);
				ei.text = sliderText;
				return ei;
			}
			return null;
		}

		public void setEditValue(int n, EditInfo ei) {
			if (n == 0) {
				bias = ei.value;
			}
			if (n == 1) {
				maxVoltage = ei.value;
			}
			if (n == 2) {
				sliderText = ei.textf.getText();
				label.setText(sliderText);
			}
		}*/

	}
}