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

		public void Connect(ElementLead other){

			// Both sides are unconnected.
			bool empty = (node == null) && (other.node == null);
			if(empty){
				CircuitNode newNode = new CircuitNode();
				newNode.Connect(this);
				newNode.Connect(other);
				return;
			}

			// One side is unconnected.
			bool lhs = (node == null) && (other.node != null);
			if(lhs){
				CircuitNode left = (lhs) ? node : other.node;
				ElementLead right = (!lhs) ? other : this;
				left.Connect(right);
				return;
			}

			// Swich connection of other node.
			node.Connect(other);
		}

		public void Disconnect(){
			if(node != null)
				node.Disconnect(this);
		}

		public bool ConnectedTo(ElementLead other){
			return node == other.node;
		}
		
		public bool ConnectedTo(CircuitNode other){
			return node == other;
		}

	}

	public class CircuitNode {

		public List<ElementLead> links;
		public bool @internal;
		
		public CircuitNode() {
			links = new List<ElementLead>();
		}

		public void Connect(ElementLead lead){
			if(!links.Contains(lead)){
				lead.Disconnect();
				lead.node = this;
				links.Add(lead);
			}
		}

		public void Disconnect(ElementLead lead){
			if(links.Contains(lead)){
				lead.node = null;
				links.Remove(lead);
			}
		}

	}

}