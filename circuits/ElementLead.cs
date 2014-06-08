using System;
using System.Collections;
using System.Collections.Generic;

namespace Circuits {

	public class ElementLead {

		public CircuitElement element{ get; private set; }
		public int index{ get; private set; }

		public CircuitNode node;

		public ElementLead(CircuitElement elm,int ndx){
			element = elm;
			index = ndx;
		}

		public void Soder(ElementLead other){
			if(node == null)
				node = new CircuitNode();
			
			other.node = node;
		}
		
		public bool SoderedTo(ElementLead other){
			return node == other.node;
		}

	}

	public class CircuitNode {

		public List<ElementLead> links;
		public bool @internal;
		
		public CircuitNode() {
			links = new List<ElementLead>();
		}
	}

}