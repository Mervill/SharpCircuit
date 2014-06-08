using System;
using System.Collections;
using System.Collections.Generic;

namespace Circuits {

	public class CircuitNodeLink {
		public int num;
		public CircuitElm elm;
	}

	public class CircuitNode {

		public Weld weld = new Weld();

		public List<CircuitNodeLink> links;
		public bool @internal;
		
		public CircuitNode() {
			links = new List<CircuitNodeLink>();
		}
	}

	public class Weld {

	}

}