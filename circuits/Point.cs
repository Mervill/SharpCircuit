using System;
using System.Collections;
using System.Collections.Generic;

namespace Circuits {

	public class Point {

		public int x{ get; set; }
		public int y{ get; set; }

		public CircuitElm linked;

		public Point(){
			
		}

		public Point(CircuitElm other){
			linked = other;
		}

		public Point(Point other){
			x = other.x;
			y = other.y;
		}

		public Point(int xvalue,int yvalue){
			x = xvalue;
			y = yvalue;
		}

	}

}