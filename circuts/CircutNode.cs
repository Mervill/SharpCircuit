using System;
using System.Collections;
using System.Collections.Generic;

namespace Circuts {

	public class CircuitNodeLink {
		public int num;
		public CircuitElm elm;
	}

	public class CircuitNode {
		public int x, y;
		public List<CircuitNodeLink> links;
		public bool @internal;
		
		public CircuitNode() {
			links = new List<CircuitNodeLink>();
		}
	}
}