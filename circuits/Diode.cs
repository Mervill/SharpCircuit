using System;
using System.Collections;
using System.Collections.Generic;

namespace Circuits {

	public class Diode {

		private int[] nodes;
		private CirSim sim;

		public double leakage = 1e-14; // was 1e-9;
		private double vt, vdcoef, fwdrop, zvoltage, zoffset;
		private double lastvoltdiff;
		private double vcrit;

		public Diode(CirSim s){
			sim = s;
			nodes = new int[2];
		}

		public void setup(double fw, double zv){
			fwdrop = fw;
			zvoltage = zv;
			vdcoef = Math.Log(1 / leakage + 1) / fwdrop;
			vt = 1 / vdcoef;
			// critical voltage for limiting; current is vt/sqrt(2) at
			// this voltage
			vcrit = vt * Math.Log(vt / (Math.Sqrt(2) * leakage));
			if(zvoltage == 0){
				zoffset = 0;
			}else{
				// calculate offset which will give us 5mA at zvoltage
				double i = -0.005;
				zoffset = zvoltage - Math.Log(-(1 + i / leakage)) / vdcoef;
			}
		}

		public void reset(){
			lastvoltdiff = 0;
		}

		public double limitStep(double vnew, double vold){
			double arg;
			// check new voltage; has current changed by factor of e^2?
			if(vnew > vcrit && Math.Abs(vnew - vold) > (vt + vt)){
				if(vold > 0){
					arg = 1 + (vnew - vold) / vt;
					if(arg > 0){
						// adjust vnew so that the current is the same
						// as in linearized model from previous iteration.
						// current at vnew = old current * arg
						vnew = vold + vt * Math.Log(arg);
						// current at v0 = 1uA
						double v0 = Math.Log(1e-6 / leakage) * vt;
						vnew = Math.Max(v0, vnew);
					}else{
						vnew = vcrit;
					}
				}else{
					// adjust vnew so that the current is the same
					// as in linearized model from previous iteration.
					// (1/vt = slope of load line)
					vnew = vt * Math.Log(vnew / vt);
				}
				sim.converged = false;
				// System.out.println(vnew + " " + oo + " " + vold);
			}else if(vnew < 0 && zoffset != 0){
				// for Zener breakdown, use the same logic but translate the values
				vnew = -vnew - zoffset;
				vold = -vold - zoffset;

				if(vnew > vcrit && Math.Abs(vnew - vold) > (vt + vt)){
					if(vold > 0){
						arg = 1 + (vnew - vold) / vt;
						if(arg > 0){
							vnew = vold + vt * Math.Log(arg);
							double v0 = Math.Log(1e-6 / leakage) * vt;
							vnew = Math.Max(v0, vnew);
							// System.out.println(oo + " " + vnew);
						}else{
							vnew = vcrit;
						}
					}else{
						vnew = vt * Math.Log(vnew / vt);
					}
					sim.converged = false;
				}
				vnew = -(vnew + zoffset);
			}
			return vnew;
		}

		public void stamp(int n0, int n1){
			nodes[0] = n0;
			nodes[1] = n1;
			sim.stampNonLinear(nodes[0]);
			sim.stampNonLinear(nodes[1]);
		}

		public void doStep(double voltdiff){
			// used to have .1 here, but needed .01 for peak detector
			if(Math.Abs(voltdiff - lastvoltdiff) > 0.01)
				sim.converged = false;
			
			voltdiff = limitStep(voltdiff, lastvoltdiff);
			lastvoltdiff = voltdiff;

			if(voltdiff >= 0 || zvoltage == 0){
				// regular diode or forward-biased zener
				double eval = Math.Exp(voltdiff * vdcoef);
				// make diode linear with negative voltages; aids convergence
				if(voltdiff < 0)
					eval = 1;
				
				double geq = vdcoef * leakage * eval;
				double nc = (eval - 1) * leakage - geq * voltdiff;
				sim.stampConductance(nodes[0], nodes[1], geq);
				sim.stampCurrentSource(nodes[0], nodes[1], nc);
			}else{
				// Zener diode
				// I(Vd) = Is * (exp[Vd*C] - exp[(-Vd-Vz)*C] - 1 )
				// geq is I'(Vd) nc is I(Vd) + I'(Vd)*(-Vd)
				double geq = leakage * vdcoef * (Math.Exp(voltdiff * vdcoef) + Math.Exp((-voltdiff - zoffset) * vdcoef));
				double nc = leakage * (Math.Exp(voltdiff * vdcoef) - Math.Exp((-voltdiff - zoffset) * vdcoef) - 1) + geq * (-voltdiff);
				sim.stampConductance(nodes[0], nodes[1], geq);
				sim.stampCurrentSource(nodes[0], nodes[1], nc);
			}
		}

		public double calculateCurrent(double voltdiff){
			if(voltdiff >= 0 || zvoltage == 0)
				return leakage * (Math.Exp(voltdiff * vdcoef) - 1);
			
			return leakage * (Math.Exp(voltdiff * vdcoef) - Math.Exp((-voltdiff - zoffset) * vdcoef) - 1);
		}
	}
}