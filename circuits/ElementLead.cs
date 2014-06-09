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

		public CircuitElement Connect(ElementLead other){

			//if(other.element == element)
			//	throw new CircuitException("Can't attach a lead to it's own element!");

			// Both sides are unconnected.
			bool empty = !IsConnected() && !other.IsConnected();
			if(empty){
				CircuitNode newNode = new CircuitNode();
				newNode.Connect(this);
				newNode.Connect(other);
				return other.element;
			}

			// One side is unconnected.
			bool lhs = (!IsConnected()) && (other.IsConnected());
			if(lhs){
				CircuitNode left = (lhs) ? other.node : node;
				ElementLead right = (!lhs) ? this : other;
				left.Connect(right);
				return other.element;
			}

			// Swich connection of other node.
			node.Connect(other);

			return other.element;
		}

		public void Disconnect(){
			if(IsConnected())
				node.Disconnect(this);
		}

		public bool ConnectedTo(ElementLead other){
			return node == other.node;
		}
		
		public bool ConnectedTo(CircuitNode other){
			return node == other;
		}

		public bool IsConnected(){
			return node != null;
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