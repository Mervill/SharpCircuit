// CirSim.java (c) 2010 by Paul Falstad
// For information about the theory behind this, see Electronic Circuit & System Simulation Methods by Pillage
// http://www.falstad.com/circuit/

using System;
using System.Collections.Generic;

using Snowflake;

namespace SharpCircuit {

	public class Circuit {

		Random random;
		IdWorker snowflake;

		public class Lead : Tuple<CircuitElement, Int32> {

			public CircuitElement elem { get { return Item1; } }
			public Int32 ndx { get { return Item2; } }

			public Lead(CircuitElement c, Int32 i) : base(c, i) {

			}

		}

		public readonly static String muString = "u";
		public readonly static String ohmString = "ohm";

		public double time { get; private set; }
		public double timeStep { get; set; }
		public int speed { get; set; } // 172 // Math.Log(10 * 14.3) * 24 + 61.5;

		public List<CircuitElement> elements { get; set; }
		public List<long[]> nodeMesh { get; set; }

		List<long> nodeList = new List<long>();
		CircuitElement[] voltageSources;

		public int nodeCount { get { return nodeList.Count; } }

		bool _analyze;
		
		double[][] circuitMatrix;
		double[] circuitRightSide;
		double[][] origMatrix;
		double[] origRightSide;
		RowInfo[] circuitRowInfo;
		int[] circuitPermute;
		
		int circuitMatrixSize, 
			circuitMatrixFullSize;

		bool circuitNonLinear, 
			 circuitNeedsMap;

		long lastTime, 
			 lastFrameTime, 
			 lastIterTime, 
			 secTime;

		public bool converged;
		public int subIterations;

		Dictionary<ICircuitComponent, List<ScopeFrame>> scopeMap;

		public String stopMessage { get; private set; }
		public CircuitElement stopElm { get; private set; }

		public Circuit() {
			
			snowflake = new IdWorker(1, 1);

			time = 0;
			timeStep = 5.0E-6;
			speed = 172;

			elements = new List<CircuitElement>();
			nodeMesh = new List<long[]>();
			scopeMap = new Dictionary<ICircuitComponent, List<ScopeFrame>>();
		}

		public T Create<T>(params object[] args) where T : CircuitElement {
			T circuit = Activator.CreateInstance(typeof(T), args) as T;
			AddElement(circuit);
			return circuit;
		}

		public void AddElement(CircuitElement elm) {
			if(!elements.Contains(elm)) {
				elements.Add(elm);
				
				nodeMesh.Add(new long[elm.getLeadCount()]);
				for(int x = 0; x < elm.getLeadCount(); x++)
					nodeMesh[nodeMesh.Count - 1][x] = -1;

				int e_ndx = elements.Count - 1;
				int m_ndx = nodeMesh.Count - 1;
				if(e_ndx != m_ndx)
					throw new System.Exception("AddElement array length mismatch");
			}
		}

		public void Connect(Lead left, Lead right) {
			Connect(left.elem, left.ndx, right.elem, right.ndx);
		}

		public void Connect(CircuitElement left, int leftLeadNdx, CircuitElement right, int rightLeadNdx) {
			int leftNdx = elements.IndexOf(left);
			int rightNdx = elements.IndexOf(right);
			Connect(leftNdx, leftLeadNdx, rightNdx, rightLeadNdx);
			needAnalyze();
		}

		public void Connect(int leftNdx, int leftLeadNdx, int rightNdx, int rightLeadNdx) {

			long[] leftLeads = nodeMesh[leftNdx];
			long[] rightLeads = nodeMesh[rightNdx];

			long leftConn = leftLeads[leftLeadNdx];
			long rightConn = rightLeads[rightLeadNdx];

			// If both leads are unconnected, we need a new node
			bool empty = leftConn == -1 && rightConn == -1;
			if(empty) {
				long id = snowflake.NextId();
				leftLeads[leftLeadNdx] = id;
				rightLeads[rightLeadNdx] = id;
				return;
			}

			// If the left lead is unconnected, attach to right
			if(leftConn == -1)
				leftLeads[leftLeadNdx] = rightLeads[rightLeadNdx];

			// If the right lead is unconnected, attach to left node
			// If the right lead is _connected_, replace with left node
			rightLeads[rightLeadNdx] = leftLeads[leftLeadNdx];
		}

		public List<ScopeFrame> Watch(ICircuitComponent component) {
			if(!scopeMap.ContainsKey(component)) {
				List<ScopeFrame> scope = new List<ScopeFrame>();
				scopeMap.Add(component, scope);
				return scope;
			}
			return null;
		}

		public List<ScopeFrame> GetWatch(ICircuitComponent component) {
			if(scopeMap.ContainsKey(component))
				return scopeMap[component];

			return new List<ScopeFrame>();
		}

		public int getRand(int x) {
			int q = random.Next();
			if(q < 0)
				q = -q;

			return q % x;
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
			_analyze = true;
		}

		public void updateVoltageSource(int n1, int n2, int vs, double v) {
			int vn = nodeList.Count + vs;
			stampRightSide(vn, v);
		}

		public long getCircuitNode(int n) {
			return (n < nodeList.Count) ? nodeList[n] : 0;
		}

		public CircuitElement getElm(int n) {
			return (n < elements.Count) ? elements[n] : null;
		}

		public double getIterCount() {
			return (speed != 0) ? 0.1 * Math.Exp((speed - 61) / 24.0) : 0;
		}

		public void update(long elapsedMilliseconds) {

			if(_analyze)
				analyze();

			run(elapsedMilliseconds);

			if(elapsedMilliseconds - secTime >= 1000)
				secTime = elapsedMilliseconds;
				
			lastTime = elapsedMilliseconds;
			lastFrameTime = lastTime;

			foreach(KeyValuePair<ICircuitComponent, List<ScopeFrame>> kvp in scopeMap)
				kvp.Value.Add(kvp.Key.GetScopeFrame(elapsedMilliseconds));
		}

		private void run(long elapsedMilliseconds) {
			if(circuitMatrix == null || elements.Count == 0) {
				circuitMatrix = null;
				return;
			}
			
			long steprate = (long)(160 * getIterCount());
			long tm = elapsedMilliseconds;
			long lit = lastIterTime;

			if(1000 >= steprate * (tm - lastIterTime))
				return;

			for(int iter = 1; ; iter++) {
				int subiter;
				for(int i = 0; i != elements.Count; i++) {
					CircuitElement ce = elements[i];
					ce.startIteration(timeStep);
				}
				
				int subiterCount = 5000;
				for(subiter = 0; subiter != subiterCount; subiter++) {
					converged = true;
					subIterations = subiter;

					for(int i = 0; i != circuitMatrixSize; i++)
						circuitRightSide[i] = origRightSide[i];

					if(circuitNonLinear)
						for(int i = 0; i != circuitMatrixSize; i++)
							for(int j = 0; j != circuitMatrixSize; j++)
								circuitMatrix[i][j] = origMatrix[i][j];

					for(int i = 0; i != elements.Count; i++)
						elements[i].doStep(this);

					if(stopMessage != null)
						return;

					for(int j = 0; j != circuitMatrixSize; j++) {
						for(int i = 0; i != circuitMatrixSize; i++) {
							double x = circuitMatrix[i][j];
							if(Double.IsNaN(x) || Double.IsInfinity(x)) {
								stop("nan/infinite matrix!", null);
								return;
							}
						}
					}

					if(circuitNonLinear) {
						if(converged && subiter > 0)
							break;

						if(!lu_factor(circuitMatrix, circuitMatrixSize, circuitPermute)) {
							stop("Singular matrix!", null);
							return;
						}
					}

					lu_solve(circuitMatrix, circuitMatrixSize, circuitPermute, circuitRightSide);

					for(int j = 0; j != circuitMatrixFullSize; j++) {
						RowInfo ri = circuitRowInfo[j];
						double res = 0;
						if(ri.type == RowInfo.ROW_CONST) {
							res = ri.value;
						} else {
							res = circuitRightSide[ri.mapCol];
						}
						if(Double.IsNaN(res)) {
							converged = false;
							break;
						}
						if(j < nodeList.Count - 1) {
							long node = getCircuitNode(j + 1);
							for(int k = 0; k != nodeMesh.Count; k++) {
								CircuitElement elm = elements[k];
								List<long> leads = new List<long>(nodeMesh[k]);
								int ndx = leads.IndexOf(node);
								if(ndx != -1)
									elm.setLeadVoltage(ndx, res);
							}
						} else {
							int ji = j - (nodeList.Count - 1);
							voltageSources[ji].setCurrent(ji, res);
						}
					}
					if(!circuitNonLinear)
						break;
				}

				/*if (subiter > 5) {
					System.out.print("converged after " + subiter + " iterations\n");
				}*/
				if(subiter == subiterCount) {
					stop("Convergence failed!", null);
					break;
				}

				time += timeStep;
				tm = elapsedMilliseconds;
				lit = tm;

				if(iter * 1000 >= steprate * (tm - lastIterTime) || (tm - lastFrameTime > 500))
					break;
			}
			lastIterTime = lit;
		}

		public void analyze() {

			if(elements.Count == 0)
				return;

			stopMessage = null;
			stopElm = null;

			int vscount = 0;
			bool gotGround = false;
			bool gotRail = false;
			CircuitElement voltageElm = null;

			nodeList.Clear();
			List<bool> internalList = new List<bool>();
			Action<long, bool> pushNode = (id, isInternal) => {
				if(!nodeList.Contains(id)) {
					nodeList.Add(id);
					internalList.Add(isInternal);
				}
			};

			#region //// Look for Voltage or Ground element ////
			for(int i = 0; i != elements.Count; i++) {
				CircuitElement ce = elements[i];
				if(ce is GroundElm) {
					gotGround = true;
					break;
				}

				if(ce is RailElm)
					gotRail = true;

				if(voltageElm == null && ce is VoltageElm)
					voltageElm = ce;
			}

			if(!gotGround && voltageElm != null && !gotRail) {
				// If no ground, and no rails, then the voltage elm's first terminal is ground.
				int elm_ndx = elements.IndexOf(voltageElm);
				long[] ndxs = nodeMesh[elm_ndx];
				pushNode(ndxs[0], false);
			} else {
				pushNode(snowflake.NextId(), false);
			}
			#endregion

			#region //// Nodes and Voltage Sources ////
			for(int i = 0; i != elements.Count; i++) {
				CircuitElement elm = elements[i];
				int elmLeads = elm.getLeadCount();

				// Populate the node list
				for(int lead_ndx = 0; lead_ndx != elmLeads; lead_ndx++) {
					// Id of the node at lead_index
					long leadNode = nodeMesh[i][lead_ndx];
					int node_ndx = nodeList.IndexOf(leadNode);
					if(node_ndx == -1) {
						elm.setLeadNode(lead_ndx, nodeList.Count);
						pushNode(leadNode, false);
					} else {
						elm.setLeadNode(lead_ndx, node_ndx);
						// if it's the ground node, make sure the node voltage is 0,
						// cause it may not get set later
						if(leadNode == 0)
							elm.setLeadVoltage(lead_ndx, 0);
					}
				}

				int internal_nodes = elm.getInternalLeadCount();
				for(int internal_lead_index = 0; internal_lead_index != internal_nodes; internal_lead_index++) {
					long id = snowflake.NextId();
					elm.setLeadNode(elmLeads + internal_lead_index, nodeList.Count); // Associate the lead with the new node.
					pushNode(id, true);
				}

				int ivs = elm.getVoltageSourceCount();
				vscount += ivs;
			}

			voltageSources = new CircuitElement[vscount];
			vscount = 0;
			#endregion

			// == Determine if circuit is nonlinear
			circuitNonLinear = false;
			for(int i = 0; i != elements.Count; i++) {
				CircuitElement ce = elements[i];
				if(ce.nonLinear())
					circuitNonLinear = true;

				int ivs = ce.getVoltageSourceCount();
				for(int lead_ndx = 0; lead_ndx != ivs; lead_ndx++) {
					voltageSources[vscount] = ce;
					ce.setVoltageSource(lead_ndx, vscount++);
				}
			}

			#region //// Matrix setup ////
			int matrixSize = nodeList.Count - 1 + vscount;
			circuitMatrix = new double[matrixSize][]; //matrixSize
			for(int z = 0; z < matrixSize; z++)
				circuitMatrix[z] = new double[matrixSize];

			origMatrix = new double[matrixSize][];
			for(int z = 0; z < matrixSize; z++)
				origMatrix[z] = new double[matrixSize];

			circuitRowInfo = new RowInfo[matrixSize];
			for(int i = 0; i != matrixSize; i++)
				circuitRowInfo[i] = new RowInfo();

			circuitRightSide = new double[matrixSize];
			origRightSide = new double[matrixSize];
			circuitPermute = new int[matrixSize];
			circuitMatrixSize = circuitMatrixFullSize = matrixSize;

			circuitNeedsMap = false;
			#endregion

			// Stamp linear circuit elements.
			for(int i = 0; i != elements.Count; i++)
				elements[i].stamp(this);

			#region //// Determine nodes that are unconnected ////
			bool[] closure = new bool[nodeList.Count];
			bool changed = true;
			closure[0] = true;
			while(changed) {
				changed = false;
				for(int i = 0; i != elements.Count; i++) {
					CircuitElement ce = elements[i];
					// loop through all ce's nodes to see if they are connected
					// to other nodes not in closure
					for(int lead_ndx = 0; lead_ndx < ce.getLeadCount(); lead_ndx++) {
						if(!closure[ce.getLeadNode(lead_ndx)]) {
							if(ce.hasGroundConnection(lead_ndx))
								closure[ce.getLeadNode(lead_ndx)] = changed = true;

							continue;
						}

						for(int k = 0; k != ce.getLeadCount(); k++) {
							if(lead_ndx == k)
								continue;

							int kn = ce.getLeadNode(k);
							if(ce.getConnection(lead_ndx, k) && !closure[kn]) {
								closure[kn] = true;
								changed = true;
							}
						}
					}
				}

				if(changed)
					continue;

				// connect unconnected nodes
				for(int i = 0; i != nodeList.Count; i++) {
					if(!closure[i] && !internalList[i]) {
						//System.out.println("node " + i + " unconnected");
						stampResistor(0, i, 1E8);
						closure[i] = true;
						changed = true;
						break;
					}
				}
			}

			#endregion

			#region //// Sanity checks ////
			for(int i = 0; i != elements.Count; i++) {
				CircuitElement ce = elements[i];

				// look for inductors with no current path
				if(ce is InductorElm) {
					FindPathInfo fpi = new FindPathInfo(this, FindPathInfo.PathType.INDUCT, ce, ce.getLeadNode(1));
					// first try findPath with maximum depth of 5, to avoid slowdowns
					if(!fpi.findPath(ce.getLeadNode(0), 5) && !fpi.findPath(ce.getLeadNode(0))) {
						//System.out.println(ce + " no path");
						ce.reset();
					}
				}

				// look for current sources with no current path
				if(ce is CurrentElm) {
					FindPathInfo fpi = new FindPathInfo(this, FindPathInfo.PathType.INDUCT, ce, ce.getLeadNode(1));
					if(!fpi.findPath(ce.getLeadNode(0))) {
						stop("No path for current source!", ce);
						return;
					}
				}

				// look for voltage source loops
				if((ce is VoltageElm && ce.getLeadCount() == 2) || ce is WireElm) {
					FindPathInfo fpi = new FindPathInfo(this, FindPathInfo.PathType.VOLTAGE, ce, ce.getLeadNode(1));
					if(fpi.findPath(ce.getLeadNode(0))) {
						stop("Voltage source/wire loop with no resistance!", ce);
						return;
					}
				}

				// look for shorted caps, or caps w/ voltage but no R
				if(ce is CapacitorElm) {
					FindPathInfo fpi = new FindPathInfo(this, FindPathInfo.PathType.SHORT, ce, ce.getLeadNode(1));
					if(fpi.findPath(ce.getLeadNode(0))) {
						//System.out.println(ce + " shorted");
						ce.reset();
					} else {
						fpi = new FindPathInfo(this, FindPathInfo.PathType.CAP_V, ce, ce.getLeadNode(1));
						if(fpi.findPath(ce.getLeadNode(0))) {
							stop("Capacitor loop with no resistance!", ce);
							return;
						}
					}
				}
			}
			#endregion

			#region //// Simplify the Matrix ////
			for(int i = 0; i != matrixSize; i++) {
				int qm = -1, qp = -1;
				double qv = 0;
				RowInfo re = circuitRowInfo[i];

				if(re.lsChanges || re.dropRow || re.rsChanges)
					continue;

				double rsadd = 0;

				// look for rows that can be removed
				int lead_ndx = 0;
				for(; lead_ndx != matrixSize; lead_ndx++) {

					double q = circuitMatrix[i][lead_ndx];
					if(circuitRowInfo[lead_ndx].type == RowInfo.ROW_CONST) {
						// keep a running total of const values that have been removed already
						rsadd -= circuitRowInfo[lead_ndx].value * q;
						continue;
					}

					if(q == 0)
						continue;

					if(qp == -1) {
						qp = lead_ndx;
						qv = q;
						continue;
					}

					if(qm == -1 && q == -qv) {
						qm = lead_ndx;
						continue;
					}

					break;
				}

				if(lead_ndx == matrixSize) {

					if(qp == -1) {
						stop("Matrix error", null);
						return;
					}

					RowInfo elt = circuitRowInfo[qp];
					if(qm == -1) {
						// we found a row with only one nonzero entry;
						// that value is a constant
						int k;
						for(k = 0; elt.type == RowInfo.ROW_EQUAL && k < 100; k++) {
							// follow the chain
							// System.out.println("following equal chain from " + i + " " + qp + " to " + elt.nodeEq);
							qp = elt.nodeEq;
							elt = circuitRowInfo[qp];
						}

						if(elt.type == RowInfo.ROW_EQUAL) {
							// break equal chains
							// System.out.println("Break equal chain");
							elt.type = RowInfo.ROW_NORMAL;
							continue;
						}

						if(elt.type != RowInfo.ROW_NORMAL) {
							//System.out.println("type already " + elt.type + " for " + qp + "!");
							continue;
						}

						elt.type = RowInfo.ROW_CONST;
						elt.value = (circuitRightSide[i] + rsadd) / qv;
						circuitRowInfo[i].dropRow = true;
						// System.out.println(qp + " * " + qv + " = const " + elt.value);
						i = -1; // start over from scratch
					} else if(circuitRightSide[i] + rsadd == 0) {
						// we found a row with only two nonzero entries, and one
						// is the negative of the other; the values are equal
						if(elt.type != RowInfo.ROW_NORMAL) {
							// System.out.println("swapping");
							int qq = qm;
							qm = qp;
							qp = qq;
							elt = circuitRowInfo[qp];
							if(elt.type != RowInfo.ROW_NORMAL) {
								// we should follow the chain here, but this hardly
								// ever happens so it's not worth worrying about
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
			for(int i = 0; i != matrixSize; i++) {
				RowInfo elt = circuitRowInfo[i];
				if(elt.type == RowInfo.ROW_NORMAL) {
					elt.mapCol = nn++;
					// System.out.println("col " + i + " maps to " + elt.mapCol);
					continue;
				}
				if(elt.type == RowInfo.ROW_EQUAL) {
					RowInfo e2 = null;
					// resolve chains of equality; 100 max steps to avoid loops
					for(int lead_ndx = 0; lead_ndx != 100; lead_ndx++) {
						e2 = circuitRowInfo[elt.nodeEq];
						if(e2.type != RowInfo.ROW_EQUAL)
							break;

						if(i == e2.nodeEq)
							break;

						elt.nodeEq = e2.nodeEq;
					}
				}
				if(elt.type == RowInfo.ROW_CONST)
					elt.mapCol = -1;
			}

			for(int i = 0; i != matrixSize; i++) {
				RowInfo elt = circuitRowInfo[i];
				if(elt.type == RowInfo.ROW_EQUAL) {
					RowInfo e2 = circuitRowInfo[elt.nodeEq];
					if(e2.type == RowInfo.ROW_CONST) {
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

			// == Make the new, simplified matrix.
			int newsize = nn;
			double[][] newmatx = new double[newsize][]; // newsize
			for(int z = 0; z < newsize; z++)
				newmatx[z] = new double[newsize];

			double[] newrs = new double[newsize];
			int ii = 0;
			for(int i = 0; i != matrixSize; i++) {
				RowInfo rri = circuitRowInfo[i];
				if(rri.dropRow) {
					rri.mapRow = -1;
					continue;
				}
				newrs[ii] = circuitRightSide[i];
				rri.mapRow = ii;
				// System.out.println("Row " + i + " maps to " + ii);
				for(int lead_ndx = 0; lead_ndx != matrixSize; lead_ndx++) {
					RowInfo ri = circuitRowInfo[lead_ndx];
					if(ri.type == RowInfo.ROW_CONST) {
						newrs[ii] -= ri.value * circuitMatrix[i][lead_ndx];
					} else {
						newmatx[ii][ri.mapCol] += circuitMatrix[i][lead_ndx];
					}
				}
				ii++;
			}
			#endregion

			#region //// solve ////
			circuitMatrix = newmatx;
			circuitRightSide = newrs;
			matrixSize = circuitMatrixSize = newsize;

			for(int i = 0; i != matrixSize; i++)
				origRightSide[i] = circuitRightSide[i];

			for(int i = 0; i != matrixSize; i++)
				for(int lead_ndx = 0; lead_ndx != matrixSize; lead_ndx++)
					origMatrix[i][lead_ndx] = circuitMatrix[i][lead_ndx];

			circuitNeedsMap = true;

			// if a matrix is linear, we can do the lu_factor here instead of
			// needing to do it every frame
			if(!circuitNonLinear) {
				if(!lu_factor(circuitMatrix, circuitMatrixSize, circuitPermute)) {
					stop("Singular matrix!", null);
					return;
				}
			}
			#endregion

			_analyze = false;
		}

		public void stop(String s, CircuitElement ce) {
			circuitMatrix = null;
			stopMessage = s;
			stopElm = ce;
			needAnalyze();
		}

		#region //// Stamp ////

		// control voltage source vs with voltage from n1 to n2 
		// (must also call stampVoltageSource())
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

		// use this if the amount of voltage is going to be updated in doStep(CirSim sim)
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
			if(i > 0 && j > 0) {
				if(circuitNeedsMap) {
					i = circuitRowInfo[i - 1].mapRow;
					RowInfo ri = circuitRowInfo[j - 1];
					if(ri.type == RowInfo.ROW_CONST) {
						circuitRightSide[i] -= x * ri.value;
						return;
					}
					j = ri.mapCol;
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
			if(i > 0) {
				if(circuitNeedsMap) {
					i = circuitRowInfo[i - 1].mapRow;
				} else {
					i--;
				}
				circuitRightSide[i] += x;
			}
		}

		// indicate that the value on the right side of row i changes in doStep(CirSim sim)
		public void stampRightSide(int i) {
			if(i > 0)
				circuitRowInfo[i - 1].rsChanges = true;
		}

		// indicate that the values on the left side of row i change in doStep(CirSim sim)
		public void stampNonLinear(int i) {
			if(i > 0)
				circuitRowInfo[i - 1].lsChanges = true;
		}

		#endregion

		// Factors a matrix into upper and lower triangular matrices by
		// gaussian elimination. On entry, a[0..n-1][0..n-1] is the
		// matrix to be factored. ipvt[] returns an integer vector of pivot
		// indices, used in the lu_solve() routine.
		public static bool lu_factor(double[][] a, int n, int[] ipvt) {
			double[] scaleFactors;
			int i, j, k;

			scaleFactors = new double[n];

			// divide each row by its largest element, keeping track of the
			// scaling factors
			for(i = 0; i != n; i++) {
				double largest = 0;
				for(j = 0; j != n; j++) {
					double x = Math.Abs(a[i][j]);
					if(x > largest)
						largest = x;
				}
				// if all zeros, it's a singular matrix
				if(largest == 0)
					return false;
				scaleFactors[i] = 1.0 / largest;
			}

			// use Crout's method; loop through the columns
			for(j = 0; j != n; j++) {

				// calculate upper triangular elements for this column
				for(i = 0; i != j; i++) {
					double q = a[i][j];
					for(k = 0; k != i; k++)
						q -= a[i][k] * a[k][j];
					a[i][j] = q;
				}

				// calculate lower triangular elements for this column
				double largest = 0;
				int largestRow = -1;
				for(i = j; i != n; i++) {
					double q = a[i][j];
					for(k = 0; k != j; k++)
						q -= a[i][k] * a[k][j];
					a[i][j] = q;
					double x = Math.Abs(q);
					if(x >= largest) {
						largest = x;
						largestRow = i;
					}
				}

				// pivoting
				if(j != largestRow) {
					double x;
					for(k = 0; k != n; k++) {
						x = a[largestRow][k];
						a[largestRow][k] = a[j][k];
						a[j][k] = x;
					}
					scaleFactors[largestRow] = scaleFactors[j];
				}

				// keep track of row interchanges
				ipvt[j] = largestRow;

				// avoid zeros
				if(a[j][j] == 0.0)
					a[j][j] = 1e-18;

				if(j != n - 1) {
					double mult = 1.0 / a[j][j];
					for(i = j + 1; i != n; i++)
						a[i][j] *= mult;
				}
			}
			return true;
		}

		// Solves the set of n linear equations using a LU factorization
		// previously performed by lu_factor. On input, b[0..n-1] is the right
		// hand side of the equations, and on output, contains the solution.
		public static void lu_solve(double[][] a, int n, int[] ipvt, double[] b) {
			// find first nonzero b element
			int i;
			for(i = 0; i != n; i++) {
				int row = ipvt[i];

				double swap = b[row];
				b[row] = b[i];
				b[i] = swap;
				if(swap != 0)
					break;
			}

			int bi = i++;
			for(; i < n; i++) {
				int row = ipvt[i];
				double tot = b[row];

				b[row] = b[i];
				// forward substitution using the lower triangular matrix
				for(int j = bi; j < i; j++)
					tot -= a[i][j] * b[j];

				b[i] = tot;
			}

			for(i = n - 1; i >= 0; i--) {
				double tot = b[i];

				// back-substitution using the upper triangular matrix
				for(int j = i + 1; j != n; j++)
					tot -= a[i][j] * b[j];

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

			Circuit sim;
			bool[] used;
			int dest;
			CircuitElement firstElm;
			PathType type;

			public FindPathInfo(Circuit r, PathType t, CircuitElement e, int d) {
				sim = r;
				dest = d;
				type = t;
				firstElm = e;
				used = new bool[sim.nodeList.Count];
			}

			public bool findPath(int n1) {
				return findPath(n1, -1);
			}

			public bool findPath(int n1, int depth) {
				if(n1 == dest)
					return true;

				if(depth-- == 0)
					return false;

				if(used[n1])
					return false;

				used[n1] = true;
				for(int i = 0; i != sim.elements.Count; i++) {

					CircuitElement ce = sim.getElm(i);
					if(ce == firstElm)
						continue;

					if(type == PathType.INDUCT)
						if(ce is CurrentElm)
							continue;

					if(type == PathType.VOLTAGE)
						if(!(ce.isWire() || ce is VoltageElm))
							continue;

					if(type == PathType.SHORT && !ce.isWire())
						continue;

					if(type == PathType.CAP_V)
						if(!(ce.isWire() || ce is CapacitorElm || ce is VoltageElm))
							continue;

					if(n1 == 0) {
						// look for posts which have a ground connection;
						// our path can go through ground
						for(int z = 0; z != ce.getLeadCount(); z++) {
							if(ce.hasGroundConnection(z) && findPath(ce.getLeadNode(z), depth)) {
								used[n1] = false;
								return true;
							}
						}
					}

					int j;
					for(j = 0; j != ce.getLeadCount(); j++)
						if(ce.getLeadNode(j) == n1)
							break;

					if(j == ce.getLeadCount())
						continue;

					if(ce.hasGroundConnection(j) && findPath(0, depth)) {
						// System.out.println(ce + " has ground");
						used[n1] = false;
						return true;
					}

					if(type == PathType.INDUCT && ce is InductorElm) {
						double c = ce.getCurrent();
						if(j == 0)
							c = -c;

						// System.out.println("matching " + c + " to " + firstElm.getCurrent());
						// System.out.println(ce + " " + firstElm);
						if(Math.Abs(c - firstElm.getCurrent()) > 1e-10)
							continue;
					}

					for(int k = 0; k != ce.getLeadCount(); k++) {
						if(j == k)
							continue;

						// System.out.println(ce + " " + ce.getNode(j) + "-" + ce.getNode(k));
						if(ce.getConnection(j, k) && findPath(ce.getLeadNode(k), depth)) {
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