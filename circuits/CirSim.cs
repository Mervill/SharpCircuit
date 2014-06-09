// CirSim.java (c) 2010 by Paul Falstad
// For information about the theory behind this, see Electronic Circuit & System Simulation Methods by Pillage
// http://www.falstad.com/circuit/

using System;
using System.Collections;
using System.Collections.Generic;

namespace Circuits {

	public class CirSim {

		public readonly static String muString = "u";
		public readonly static String ohmString = "ohm";

		System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();

		public Random random;

		public bool stopped = false;
		public double time{ get; private set; }
		public double timeStep = 5.0E-6;

		public bool conventionalCurrent = true;
		public double speed = Math.Log(10 * 14.3) * 24 + 61.5; // 14.3
		public double currentSpeed = 55;
		public double powerBrightness = 50;

		public List<CircuitElement> elements = new List<CircuitElement>();
		public List<CircuitNode> nodeList = new List<CircuitNode>();
		public CircuitElement[] voltageSources;
		public int voltageSourceCount{ get; private set; }

		private bool analyzeFlag;
		public bool dumpMatrix;
		public String stopMessage{ get; private set; }
		public CircuitElement stopElm{ get; private set; }

		public int scopeCount;
		public Scope[] scopes;
		public string[] info;

		private double[][] circuitMatrix; 
		private double[] circuitRightSide; 
		private double[][] origMatrix;
		private double[] origRightSide;
		private RowInfo[] circuitRowInfo;
		private int[] circuitPermute;
		private bool circuitNonLinear, circuitNeedsMap;
		private int circuitMatrixSize, circuitMatrixFullSize;

		private long lastTime, lastFrameTime, lastIterTime, secTime;
		public int frames{ get; private set; }
		public int steps{ get; private set; }

		public bool converged;
		public int subIterations;

		public CirSim(){
			watch.Start();
		}

		public int getrand(int x) {
			int q = random.Next();
			if (q < 0) {
				q = -q;
			}
			return q % x;
		}

		public void updateCircuit() {

			if (analyzeFlag) {
				analyzeCircuit();
				analyzeFlag = false;
			}

			if (!stopped) {
				try {
					runCircuit();
				} catch (Exception) {
					analyzeFlag = true;
					return;
				}
			}

			if (!stopped) {
				long sysTime = watch.ElapsedMilliseconds;
				if (lastTime != 0) {
					int inc = (int) (sysTime - lastTime);
					double c = currentSpeed;
					c = Math.Exp(c / 3.5 - 14.2);
					CircuitElement.currentMult = 1.7 * inc * c;
					if (!conventionalCurrent) {
						CircuitElement.currentMult = -CircuitElement.currentMult;
					}
				}
				if (sysTime - secTime >= 1000) {
					//framerate = frames;
					//steprate = steps;
					frames = 0;
					steps = 0;
					secTime = sysTime;
				}
				lastTime = sysTime;
			} else {
				lastTime = 0;
			}

			CircuitElement.powerMult = Math.Exp(powerBrightness / 4.762 - 7);

			int i = 0;
			int badnodes = 0;
			// find bad connections, nodes not connected to other elements which
			// intersect other elements' bounding boxes
			// debugged by hausen: nullPointerException
			if (nodeList != null) {
				for (i = 0; i != nodeList.Count; i++) {
					CircuitNode cn = getCircuitNode(i);
					if (!cn.@internal && cn.links.Count == 1) {
						int bb = 0, j;
						ElementLead cnl = cn.links[0];
						for (j = 0; j != elements.Count; j++) { // TODO: (hausen) see if this change does not break stuff
							CircuitElement ce = getElm(j);
							if (cnl.element != ce) { //&& getElm(j).boundingBox.contains(cn.x, cn.y) // && (getElm(j).x == cn.x && getElm(j).y == cn.y)
								bb++;
							}
						}
						if (bb > 0) {
							badnodes++;
						}
					}
				}
			}


			if (stopMessage == null) {
				info = new String[10];
				info[0] = "time = " + CircuitElement.getUnitText(time, "s");

				for (i = 0; info[i] != null; i++) {
					;
				}
				if (badnodes > 0) {
					info[i++] = badnodes + ((badnodes == 1) ? " bad connection" : " bad connections");
				}
			}

			frames++;

			if (!stopped && circuitMatrix != null) {
				// Limit to 50 fps (thanks to Jurgen Klotzer for this)
				long delay = 1000 / 50 - (watch.ElapsedMilliseconds - lastFrameTime);
				// realg.drawString("delay: " + delay, 10, 90);
				if (delay > 0) {
					try {
						//Thread.sleep(delay);
					} catch (Exception) {
					}
				}
			}
			lastFrameTime = lastTime;
		}

		/*public void toggleSwitch(int n) {
			int i;
			for (i = 0; i != elmList.Count; i++) {
				CircuitElm ce = getElm(i);
				if (ce is SwitchElm) {
					n--;
					if (n == 0) {
						((SwitchElm) ce).toggle();
						analyzeFlag = true;
						return;
					}
				}
			}
		}

		public bool doSwitch(int x, int y) {
			if (mouseElm == null || !(mouseElm is SwitchElm)) {
				return false;
			}
			SwitchElm se = (SwitchElm) mouseElm;
			se.toggle();
			if (se.momentary) {
				heldSwitchElm = se;
			}
			needAnalyze();
			return true;
		}*/

		public void needAnalyze() {
			analyzeFlag = true;
		}

		private void analyzeCircuit() {

			if (elements.Count == 0)
				return;

			stopMessage = null;
			stopElm = null;
			int i, lead_index;
			int vscount = 0;
			nodeList = new List<CircuitNode>();
			bool gotGround = false;
			bool gotRail = false;
			CircuitElement volt = null;

			#region Look for Voltage or Ground element
			for (i = 0; i != elements.Count; i++) {
				CircuitElement ce = getElm(i);
				if (ce is GroundElm) {
					gotGround = true;
					break;
				}

				if (ce is RailElm)
					gotRail = true;

				if (volt == null && ce is VoltageElm)
					volt = ce;
				
			}


			if(!gotGround && volt != null && !gotRail){
				// If no ground, and no rails, then the voltage elm's first terminal is ground.
				ElementLead pt = volt.getLead(0);
				CircuitNode node = pt.node;
				nodeList.Add(node);
			}else{
				// Otherwise allocate extra node for ground.
				CircuitNode node = new CircuitNode();
				nodeList.Add(node);
			}
			#endregion

			#region Nodes and Voltage Sources
			for (i = 0; i != elements.Count; i++) {
				CircuitElement current_elm = getElm(i);
				int num_leads = current_elm.getLeadCount();

				// Populate the node list
				for (lead_index = 0; lead_index != num_leads; lead_index++) {

					ElementLead current_lead = current_elm.getLead(lead_index);

					int node_index = nodeList.IndexOf(current_lead.node);

					if (node_index == -1) {
						current_elm.setNode(lead_index,nodeList.Count);
						nodeList.Add(current_lead.node);
					} else {

						current_elm.setNode(lead_index, node_index);

						// if it's the ground node, make sure the node voltage is 0,
						// cause it may not get set later
						if (node_index == 0)
							current_elm.setNodeVoltage(lead_index, 0);
						
					}
				}

				int internal_nodes = current_elm.getInternalNodeCount();
				for (int internal_lead_index = 0; internal_lead_index != internal_nodes; internal_lead_index++) {
					CircuitNode newNode = new CircuitNode();
					//newNode.x = newNode.y = -1;
					newNode.@internal = true;

					ElementLead newLead = new ElementLead(current_elm,num_leads + internal_lead_index);
					newNode.links.Add(newLead); // Add the lead to the new node.

					current_elm.setNode(newLead.index, nodeList.Count);	// Associate the lead with the new node.
					nodeList.Add(newNode); 								// Add the new node to the node list.
				}

				int ivs = current_elm.getVoltageSourceCount();
				vscount += ivs;
			}

			voltageSources = new CircuitElement[vscount];
			vscount = 0;
			voltageSourceCount = vscount;
			#endregion

			// == Determine if circuit is nonlinear
			circuitNonLinear = false;
			for (i = 0; i != elements.Count; i++) {
				CircuitElement ce = getElm(i);
				if (ce.nonLinear())
					circuitNonLinear = true;
				
				int ivs = ce.getVoltageSourceCount();
				for (lead_index = 0; lead_index != ivs; lead_index++) {
					voltageSources[vscount] = ce;
					ce.setVoltageSource(lead_index, vscount++);
				}
			}

			#region Matrix setup
			int matrixSize = nodeList.Count - 1 + vscount;
			circuitMatrix = new double[matrixSize][]; //matrixSize
			for (int z = 0; z < matrixSize; z++)
				circuitMatrix[z] = new double[matrixSize];

			origMatrix = new double[matrixSize][];
			for (int z = 0; z < matrixSize; z++)
				origMatrix[z] = new double[matrixSize];

			circuitRowInfo = new RowInfo[matrixSize];
			for (i = 0; i != matrixSize; i++)
				circuitRowInfo[i] = new RowInfo();

			circuitRightSide = new double[matrixSize];
			origRightSide = new double[matrixSize];
			circuitPermute = new int[matrixSize];
			circuitMatrixSize = circuitMatrixFullSize = matrixSize;

			circuitNeedsMap = false;
			#endregion

			// Stamp linear circuit elements.
			for (i = 0; i != elements.Count; i++) {
				CircuitElement ce = getElm(i);
				ce.stamp();
			}

			#region Determine nodes that are unconnected
			bool[] closure = new bool[nodeList.Count];
			bool changed = true;
			closure[0] = true;
			while (changed) {
				changed = false;
				for (i = 0; i != elements.Count; i++) {
					CircuitElement ce = getElm(i);
					// loop through all ce's nodes to see if they are connected
					// to other nodes not in closure
					for (lead_index = 0; lead_index < ce.getLeadCount(); lead_index++) {
						if (!closure[ce.getNode(lead_index)]) {
							if (ce.hasGroundConnection(lead_index))
								closure[ce.getNode(lead_index)] = changed = true;
							
							continue;
						}
						int k;
						for (k = 0; k != ce.getLeadCount(); k++) {
							if (lead_index == k)
								continue;
							
							int kn = ce.getNode(k);
							if (ce.getConnection(lead_index, k) && !closure[kn]) {
								closure[kn] = true;
								changed = true;
							}
						}
					}
				}

				if(changed)
					continue;

				// connect unconnected nodes
				for (i = 0; i != nodeList.Count; i++) {
					if (!closure[i] && !getCircuitNode(i).@internal) {
						//System.out.println("node " + i + " unconnected");
						stampResistor(0, i, 1E8);
						closure[i] = true;
						changed = true;
						break;
					}
				}
			}

			#endregion

			#region Sanity checks
			for (i = 0; i != elements.Count; i++) {
				CircuitElement ce = getElm(i);

				// look for inductors with no current path
				if (ce is InductorElm) {
					FindPathInfo fpi = new FindPathInfo(this,FindPathInfo.PathType.INDUCT, ce, ce.getNode(1));
					// first try findPath with maximum depth of 5, to avoid slowdowns
					if (!fpi.findPath(ce.getNode(0), 5) && !fpi.findPath(ce.getNode(0))) {
						//System.out.println(ce + " no path");
						ce.reset();
					}
				}

				// look for current sources with no current path
				if (ce is CurrentElm) {
					FindPathInfo fpi = new FindPathInfo(this,FindPathInfo.PathType.INDUCT, ce,ce.getNode(1));
					if (!fpi.findPath(ce.getNode(0))) {
						stop("No path for current source!", ce);
						return;
					}
				}

				// look for voltage source loops
				if ((ce is VoltageElm && ce.getLeadCount() == 2) || ce is WireElm) {
					FindPathInfo fpi = new FindPathInfo(this,FindPathInfo.PathType.VOLTAGE, ce,ce.getNode(1));
					if (fpi.findPath(ce.getNode(0))) {
						stop("Voltage source/wire loop with no resistance!", ce);
						return;
					}
				}

				// look for shorted caps, or caps w/ voltage but no R
				if (ce is CapacitorElm) {
					FindPathInfo fpi = new FindPathInfo(this,FindPathInfo.PathType.SHORT, ce, ce.getNode(1));
					if (fpi.findPath(ce.getNode(0))) {
						//System.out.println(ce + " shorted");
						ce.reset();
					} else {
						fpi = new FindPathInfo(this,FindPathInfo.PathType.CAP_V, ce, ce.getNode(1));
						if (fpi.findPath(ce.getNode(0))) {
							stop("Capacitor loop with no resistance!", ce);
							return;
						}
					}
				}
			}
			#endregion

			#region Simplify the Matrix
			for (i = 0; i != matrixSize; i++) {
				int qm = -1, qp = -1;
				double qv = 0;
				RowInfo re = circuitRowInfo[i];
				// System.out.println("row " + i + " " + re.lsChanges + " " + re.rsChanges + " " + re.dropRow);
				if (re.lsChanges || re.dropRow || re.rsChanges) {
					continue;
				}
				double rsadd = 0;

				// look for rows that can be removed
				for (lead_index = 0; lead_index != matrixSize; lead_index++) {
					double q = circuitMatrix[i][lead_index];
					if (circuitRowInfo[lead_index].type == RowInfo.ROW_CONST) {
						// keep a running total of const values that have been removed already
						rsadd -= circuitRowInfo[lead_index].value * q;
						continue;
					}
					if (q == 0) {
						continue;
					}
					if (qp == -1) {
						qp = lead_index;
						qv = q;
						continue;
					}
					if (qm == -1 && q == -qv) {
						qm = lead_index;
						continue;
					}
					break;
				}
				// System.out.println("line " + i + " " + qp + " " + qm + " " + j);
				/*
				 * if (qp != -1 && circuitRowInfo[qp].lsChanges) {
				 * System.out.println("lschanges"); continue; } if (qm != -1 &&
				 * circuitRowInfo[qm].lsChanges) { System.out.println("lschanges");
				 * continue; }
				 */
				if (lead_index == matrixSize) {
					if (qp == -1) {
						stop("Matrix error", null);
						return;
					}
					RowInfo elt = circuitRowInfo[qp];
					if (qm == -1) {
						// we found a row with only one nonzero entry; 
						// that value is a constant
						int k;
						for (k = 0; elt.type == RowInfo.ROW_EQUAL && k < 100; k++) {
							// follow the chain
							// System.out.println("following equal chain from " + i + " " + qp + " to " + elt.nodeEq);
							qp = elt.nodeEq;
							elt = circuitRowInfo[qp];
						}
						if (elt.type == RowInfo.ROW_EQUAL) {
							// break equal chains
							// System.out.println("Break equal chain");
							elt.type = RowInfo.ROW_NORMAL;
							continue;
						}
						if (elt.type != RowInfo.ROW_NORMAL) {
							//System.out.println("type already " + elt.type + " for " + qp + "!");
							continue;
						}
						elt.type = RowInfo.ROW_CONST;
						elt.value = (circuitRightSide[i] + rsadd) / qv;
						circuitRowInfo[i].dropRow = true;
						// System.out.println(qp + " * " + qv + " = const " + elt.value);
						i = -1; // start over from scratch
					} else if (circuitRightSide[i] + rsadd == 0) {
						// we found a row with only two nonzero entries, and one
						// is the negative of the other; the values are equal
						if (elt.type != RowInfo.ROW_NORMAL) {
							// System.out.println("swapping");
							int qq = qm;
							qm = qp;
							qp = qq;
							elt = circuitRowInfo[qp];
							if (elt.type != RowInfo.ROW_NORMAL) {
								// we should follow the chain here, but this hardly
								//  ever happens so it's not worth worrying about
								//System.out.println("swap failed");
								continue;
							}
						}
						elt.type = RowInfo.ROW_EQUAL;
						elt.nodeEq = qm;
						circuitRowInfo[i].dropRow = true;
						// System.out.println(qp + " = " + qm);
					}
				}
			}

			// == Find size of new matrix
			int nn = 0;
			for (i = 0; i != matrixSize; i++) {
				RowInfo elt = circuitRowInfo[i];
				if (elt.type == RowInfo.ROW_NORMAL) {
					elt.mapCol = nn++;
					// System.out.println("col " + i + " maps to " + elt.mapCol);
					continue;
				}
				if (elt.type == RowInfo.ROW_EQUAL) {
					RowInfo e2 = null;
					// resolve chains of equality; 100 max steps to avoid loops
					for (lead_index = 0; lead_index != 100; lead_index++) {
						e2 = circuitRowInfo[elt.nodeEq];
						if (e2.type != RowInfo.ROW_EQUAL) {
							break;
						}
						if (i == e2.nodeEq) {
							break;
						}
						elt.nodeEq = e2.nodeEq;
					}
				}
				if (elt.type == RowInfo.ROW_CONST) {
					elt.mapCol = -1;
				}
			}

			for (i = 0; i != matrixSize; i++) {
				RowInfo elt = circuitRowInfo[i];
				if (elt.type == RowInfo.ROW_EQUAL) {
					RowInfo e2 = circuitRowInfo[elt.nodeEq];
					if (e2.type == RowInfo.ROW_CONST) {
						// if something is equal to a const, it's a const
						elt.type = e2.type;
						elt.value = e2.value;
						elt.mapCol = -1;
						// System.out.println(i + " = [late]const " + elt.value);
					} else {
						elt.mapCol = e2.mapCol;
						// System.out.println(i + " maps to: " + e2.mapCol);
					}
				}
			}

			/*
			 * System.out.println("matrixSize = " + matrixSize);
			 * 
			 * for (j = 0; j != circuitMatrixSize; j++) { System.out.println(j +
			 * ": "); for (i = 0; i != circuitMatrixSize; i++)
			 * System.out.print(circuitMatrix[j][i] + " "); System.out.print("  " +
			 * circuitRightSide[j] + "\n"); } System.out.print("\n");
			 */

			// == Make the new, simplified matrix.
			int newsize = nn;
			double[][] newmatx = new double[newsize][]; // newsize
			for(int z = 0;z < newsize;z++){
				newmatx[z] = new double[newsize];
			}

			double[] newrs = new double[newsize];
			int ii = 0;
			for (i = 0; i != matrixSize; i++) {
				RowInfo rri = circuitRowInfo[i];
				if (rri.dropRow) {
					rri.mapRow = -1;
					continue;
				}
				newrs[ii] = circuitRightSide[i];
				rri.mapRow = ii;
				// System.out.println("Row " + i + " maps to " + ii);
				for (lead_index = 0; lead_index != matrixSize; lead_index++) {
					RowInfo ri = circuitRowInfo[lead_index];
					if (ri.type == RowInfo.ROW_CONST) {
						newrs[ii] -= ri.value * circuitMatrix[i][lead_index];
					} else {
						newmatx[ii][ri.mapCol] += circuitMatrix[i][lead_index];
					}
				}
				ii++;
			}
			#endregion

			#region solve

			circuitMatrix = newmatx;
			circuitRightSide = newrs;
			matrixSize = circuitMatrixSize = newsize;

			for (i = 0; i != matrixSize; i++)
				origRightSide[i] = circuitRightSide[i];
			
			for (i = 0; i != matrixSize; i++)
				for (lead_index = 0; lead_index != matrixSize; lead_index++)
					origMatrix[i][lead_index] = circuitMatrix[i][lead_index];

			circuitNeedsMap = true;

			/*
			 * System.out.println("matrixSize = " + matrixSize + " " +
			 * circuitNonLinear); for (j = 0; j != circuitMatrixSize; j++) { for (i
			 * = 0; i != circuitMatrixSize; i++)
			 * System.out.print(circuitMatrix[j][i] + " "); System.out.print("  " +
			 * circuitRightSide[j] + "\n"); } System.out.print("\n");
			 */

			// if a matrix is linear, we can do the lu_factor here instead of
			// needing to do it every frame
			if (!circuitNonLinear) {
				if (!lu_factor(circuitMatrix, circuitMatrixSize, circuitPermute)) {
					stop("Singular matrix!", null);
					return;
				}
			}
			#endregion
		}

		#region Stamp

		// control voltage source vs with voltage from n1 to n2 (must
		// also call stampVoltageSource())
		public void stampVCVS(int n1, int n2, double coef, int vs) {
			int vn = nodeList.Count + vs;
			stampMatrix(vn, n1, coef);
			stampMatrix(vn, n2, -coef);
		}

		// stamp independent voltage source #vs, from n1 to n2, amount v
		public void stampVoltageSource(int n1, int n2, int vs, double v) {
			int vn = nodeList.Count + vs;
			stampMatrix(vn, n1, -1);
			stampMatrix(vn, n2, 1);
			stampRightSide(vn, v);
			stampMatrix(n1, vn, 1);
			stampMatrix(n2, vn, -1);
		}

		// use this if the amount of voltage is going to be updated in doStep()
		public void stampVoltageSource(int n1, int n2, int vs) {
			int vn = nodeList.Count + vs;
			stampMatrix(vn, n1, -1);
			stampMatrix(vn, n2, 1);
			stampRightSide(vn);
			stampMatrix(n1, vn, 1);
			stampMatrix(n2, vn, -1);
		}

		public void stampResistor(int n1, int n2, double r) {
			double r0 = 1 / r;
			if (Double.IsNaN(r0) || Double.IsInfinity(r0)) {
				//System.out.print("bad resistance " + r + " " + r0 + "\n");
				int a = 0;
				a /= a;
			}
			stampMatrix(n1, n1, r0);
			stampMatrix(n2, n2, r0);
			stampMatrix(n1, n2, -r0);
			stampMatrix(n2, n1, -r0);
		}

		public void stampConductance(int n1, int n2, double r0) {
			stampMatrix(n1, n1, r0);
			stampMatrix(n2, n2, r0);
			stampMatrix(n1, n2, -r0);
			stampMatrix(n2, n1, -r0);
		}

		// current from cn1 to cn2 is equal to voltage from vn1 to 2, divided by g
		public void stampVCCurrentSource(int cn1, int cn2, int vn1, int vn2, double g) {
			stampMatrix(cn1, vn1, g);
			stampMatrix(cn2, vn2, g);
			stampMatrix(cn1, vn2, -g);
			stampMatrix(cn2, vn1, -g);
		}

		public void stampCurrentSource(int n1, int n2, double i) {
			stampRightSide(n1, -i);
			stampRightSide(n2, i);
		}

		// stamp a current source from n1 to n2 depending on current through vs
		public void stampCCCS(int n1, int n2, int vs, double gain) {
			int vn = nodeList.Count + vs;
			stampMatrix(n1, vn, gain);
			stampMatrix(n2, vn, -gain);
		}

		// stamp value x in row i, column j, meaning that a voltage change
		// of dv in node j will increase the current into node i by x dv.
		// (Unless i or j is a voltage source node.)
		public void stampMatrix(int i, int j, double x) {
			if (i > 0 && j > 0) {
				if (circuitNeedsMap) {
					i = circuitRowInfo[i - 1].mapRow;
					RowInfo ri = circuitRowInfo[j - 1];
					if (ri.type == RowInfo.ROW_CONST) {
						// System.out.println("Stamping constant " + i + " " + j + " " + x);
						circuitRightSide[i] -= x * ri.value;
						return;
					}
					j = ri.mapCol;
					// System.out.println("stamping " + i + " " + j + " " + x);
				} else {
					i--;
					j--;
				}
				circuitMatrix[i][j] += x;
			}
		}

		// stamp value x on the right side of row i, representing an
		// independent current source flowing into node i
		public void stampRightSide(int i, double x) {
			if (i > 0) {
				if (circuitNeedsMap) {
					i = circuitRowInfo[i - 1].mapRow;
					// System.out.println("stamping " + i + " " + x);
				} else {
					i--;
				}
				circuitRightSide[i] += x;
			}
		}

		// indicate that the value on the right side of row i changes in doStep()
		public void stampRightSide(int i) {
			// System.out.println("rschanges true " + (i-1));
			if (i > 0) {
				circuitRowInfo[i - 1].rsChanges = true;
			}
		}

		// indicate that the values on the left side of row i change in doStep()
		public void stampNonLinear(int i) {
			if (i > 0) {
				circuitRowInfo[i - 1].lsChanges = true;
			}
		}

		#endregion

		public void updateVoltageSource(int n1, int n2, int vs, double v) {
			int vn = nodeList.Count + vs;
			stampRightSide(vn, v);
		}

		public CircuitNode getCircuitNode(int n) {
			if (n >= nodeList.Count) {
				return null;
			}
			return nodeList[n];
		}
		
		public CircuitElement getElm(int n) {
			if (n >= elements.Count) {
				return null;
			}
			return elements[n];
		}

		public double getIterCount() {
			if (speed == 0) {
				return 0;
			}
			// return (Math.exp((speedBar.getValue()-1)/24.) + 0.5);
			return 0.1 * Math.Exp((speed - 61) / 24.0);
		}

		private void runCircuit() {
			if (circuitMatrix == null || elements.Count == 0) {
				circuitMatrix = null;
				return;
			}
			int iter;
			// int maxIter = getIterCount();
			bool debugprint = dumpMatrix;
			dumpMatrix = false;
			long steprate = (long) (160 * getIterCount());
			long tm = watch.ElapsedMilliseconds;
			long lit = lastIterTime;
			if (1000 >= steprate * (tm - lastIterTime)) {
				return;
			}
			for (iter = 1;; iter++) {
				int i, j, k, subiter;
				for (i = 0; i != elements.Count; i++) {
					CircuitElement ce = getElm(i);
					ce.startIteration();
				}
				steps++;
				int subiterCount = 5000;
				for (subiter = 0; subiter != subiterCount; subiter++) {
					converged = true;
					subIterations = subiter;
					for (i = 0; i != circuitMatrixSize; i++) {
						circuitRightSide[i] = origRightSide[i];
					}
					if (circuitNonLinear) {
						for (i = 0; i != circuitMatrixSize; i++) {
							for (j = 0; j != circuitMatrixSize; j++) {
								circuitMatrix[i][j] = origMatrix[i][j];
							}
						}
					}
					for (i = 0; i != elements.Count; i++) {
						CircuitElement ce = getElm(i);
						ce.doStep();
					}
					if (stopMessage != null) {
						return;
					}
					bool printit = debugprint;
					debugprint = false;
					for (j = 0; j != circuitMatrixSize; j++) {
						for (i = 0; i != circuitMatrixSize; i++) {
							double x = circuitMatrix[i][j];
							if (Double.IsNaN(x) || Double.IsInfinity(x)) {
								stop("nan/infinite matrix!", null);
								return;
							}
						}
					}
					if (printit) {
						for (j = 0; j != circuitMatrixSize; j++) {
							for (i = 0; i != circuitMatrixSize; i++) {
								//System.out.print(circuitMatrix[j][i] + ",");
							}
							//System.out.print("  " + circuitRightSide[j] + "\n");
						}
						//System.out.print("\n");
					}
					if (circuitNonLinear) {
						if (converged && subiter > 0) {
							break;
						}
						if (!lu_factor(circuitMatrix, circuitMatrixSize,
								circuitPermute)) {
							stop("Singular matrix!", null);
							return;
						}
					}
					lu_solve(circuitMatrix, circuitMatrixSize, circuitPermute, circuitRightSide);

					for (j = 0; j != circuitMatrixFullSize; j++) {
						RowInfo ri = circuitRowInfo[j];
						double res = 0;
						if (ri.type == RowInfo.ROW_CONST) {
							res = ri.value;
						} else {
							res = circuitRightSide[ri.mapCol];
						}
						// System.out.println(j + " " + res + " " + ri.type + " " + ri.mapCol);
						if (Double.IsNaN(res)) {
							converged = false;
							// debugprint = true;
							break;
						}
						if (j < nodeList.Count - 1) {
							CircuitNode node = getCircuitNode(j + 1);
							for (k = 0; k != node.links.Count; k++) {
								ElementLead lead = node.links[k];
								lead.element.setNodeVoltage(lead.index, res);
							}
						} else {
							int ji = j - (nodeList.Count - 1);
							// System.out.println("setting vsrc " + ji + " to " + res);
							voltageSources[ji].setCurrent(ji, res);
						}
					}
					if (!circuitNonLinear) {
						break;
					}
				}
				if (subiter > 5) {
					//System.out.print("converged after " + subiter + " iterations\n");
				}
				if (subiter == subiterCount) {
					stop("Convergence failed!", null);
					break;
				}
				time += timeStep;
				for (i = 0; i != scopeCount; i++) {
					//scopes[i].timeStep();
				}
				tm = watch.ElapsedMilliseconds;
				lit = tm;
				if (iter * 1000 >= steprate * (tm - lastIterTime) || (tm - lastFrameTime > 500)) {
					break;
				}
			}
			lastIterTime = lit;
			// System.out.println((System.currentTimeMillis()-lastFrameTime)/(double)iter);
		}

		public void stop(String s, CircuitElement ce) {
			stopMessage = s;
			circuitMatrix = null;
			stopElm = ce;
			stopped = true;
			analyzeFlag = false;
		}

		// factors a matrix into upper and lower triangular matrices by
		// gaussian elimination. On entry, a[0..n-1][0..n-1] is the
		// matrix to be factored. ipvt[] returns an integer vector of pivot
		// indices, used in the lu_solve() routine.
		public bool lu_factor(double[][] a, int n, int[] ipvt) {
			double[] scaleFactors;
			int i, j, k;

			scaleFactors = new double[n];

			// divide each row by its largest element, keeping track of the
			// scaling factors
			for (i = 0; i != n; i++) {
				double largest = 0;
				for (j = 0; j != n; j++) {
					double x = Math.Abs(a[i][j]);
					if (x > largest) {
						largest = x;
					}
				}
				// if all zeros, it's a singular matrix
				if (largest == 0) {
					return false;
				}
				scaleFactors[i] = 1.0 / largest;
			}

			// use Crout's method; loop through the columns
			for (j = 0; j != n; j++) {

				// calculate upper triangular elements for this column
				for (i = 0; i != j; i++) {
					double q = a[i][j];
					for (k = 0; k != i; k++) {
						q -= a[i][k] * a[k][j];
					}
					a[i][j] = q;
				}

				// calculate lower triangular elements for this column
				double largest = 0;
				int largestRow = -1;
				for (i = j; i != n; i++) {
					double q = a[i][j];
					for (k = 0; k != j; k++) {
						q -= a[i][k] * a[k][j];
					}
					a[i][j] = q;
					double x = Math.Abs(q);
					if (x >= largest) {
						largest = x;
						largestRow = i;
					}
				}

				// pivoting
				if (j != largestRow) {
					double x;
					for (k = 0; k != n; k++) {
						x = a[largestRow][k];
						a[largestRow][k] = a[j][k];
						a[j][k] = x;
					}
					scaleFactors[largestRow] = scaleFactors[j];
				}

				// keep track of row interchanges
				ipvt[j] = largestRow;

				// avoid zeros
				if (a[j][j] == 0.0) {
					//System.out.println("avoided zero");
					a[j][j] = 1e-18;
				}

				if (j != n - 1) {
					double mult = 1.0 / a[j][j];
					for (i = j + 1; i != n; i++) {
						a[i][j] *= mult;
					}
				}
			}
			return true;
		}

		// Solves the set of n linear equations using a LU factorization
		// previously performed by lu_factor. On input, b[0..n-1] is the right
		// hand side of the equations, and on output, contains the solution.
		public void lu_solve(double[][] a, int n, int[] ipvt, double[] b) {
			int i;

			// find first nonzero b element
			for (i = 0; i != n; i++) {
				int row = ipvt[i];

				double swap = b[row];
				b[row] = b[i];
				b[i] = swap;
				if (swap != 0) {
					break;
				}
			}

			int bi = i++;
			for (; i < n; i++) {
				int row = ipvt[i];
				int j;
				double tot = b[row];

				b[row] = b[i];
				// forward substitution using the lower triangular matrix
				for (j = bi; j < i; j++) {
					tot -= a[i][j] * b[j];
				}
				b[i] = tot;
			}
			for (i = n - 1; i >= 0; i--) {
				double tot = b[i];

				// back-substitution using the upper triangular matrix
				int j;
				for (j = i + 1; j != n; j++) {
					tot -= a[i][j] * b[j];
				}
				b[i] = tot / a[i][i];
			}
		}

		private class FindPathInfo {
			
			public enum PathType {
				INDUCT,
				VOLTAGE,
				SHORT,
				CAP_V,
			}
			
			CirSim sim;
			bool[] used;
			int dest;
			CircuitElement firstElm;
			PathType type;
			
			public FindPathInfo(CirSim r,PathType t, CircuitElement e, int d){
				sim = r;
				dest = d;
				type = t;
				firstElm = e;
				used = new bool[sim.nodeList.Count];
			}
			
			public bool findPath(int n1){
				return findPath(n1, -1);
			}
			
			public bool findPath(int n1, int depth){
				if(n1 == dest)
					return true;
				
				if(depth-- == 0)
					return false;
				
				if(used[n1]){
					// System.out.println("used " + n1);
					return false;
				}
				
				used[n1] = true;
				int i;
				for (i = 0; i != sim.elements.Count; i++){
					CircuitElement ce = sim.getElm(i);
					if(ce == firstElm)
						continue;
					
					if(type == PathType.INDUCT){
						if(ce is CurrentElm)
							continue;
					}
					
					if(type == PathType.VOLTAGE) {
						if(!(ce.isWire() || ce is VoltageElm))
							continue;
					}
					
					if(type == PathType.SHORT && !ce.isWire())
						continue;
					
					
					if(type == PathType.CAP_V){
						if(!(ce.isWire() || ce is CapacitorElm || ce is VoltageElm))
							continue;
					}
					
					if(n1 == 0){
						// look for posts which have a ground connection;
						// our path can go through ground
						int z;
						for(z = 0; z != ce.getLeadCount(); z++){
							if(ce.hasGroundConnection(z)
							   && findPath(ce.getNode(z), depth)){
								used[n1] = false;
								return true;
							}
						}
					}
					
					int j;
					for(j = 0; j != ce.getLeadCount(); j++){
						// System.out.println(ce + " " + ce.getNode(j));
						if(ce.getNode(j) == n1)
							break;
					}
					
					if(j == ce.getLeadCount())
						continue;
					
					if(ce.hasGroundConnection(j) && findPath(0, depth)){
						// System.out.println(ce + " has ground");
						used[n1] = false;
						return true;
					}
					
					if(type == PathType.INDUCT && ce is InductorElm){
						double c = ce.getCurrent();
						if(j == 0)
							c = -c;
						
						// System.out.println("matching " + c + " to " +
						// firstElm.getCurrent());
						// System.out.println(ce + " " + firstElm);
						if(Math.Abs(c - firstElm.getCurrent()) > 1e-10)
							continue;
					}
					
					int k;
					for(k = 0; k != ce.getLeadCount(); k++){
						if(j == k)
							continue;
						
						// System.out.println(ce + " " + ce.getNode(j) + "-" +
						// ce.getNode(k));
						if(ce.getConnection(j, k) && findPath(ce.getNode(k), depth)){
							// System.out.println("got findpath " + n1);
							used[n1] = false;
							return true;
						}
						// System.out.println("back on findpath " + n1);
					}
				}
				
				used[n1] = false;
				// System.out.println(n1 + " failed");
				return false;
			}
		}

	}
}