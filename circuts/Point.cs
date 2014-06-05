using System;
using System.Collections;
using System.Collections.Generic;

namespace Circuts {

	public class Point {

		public int x{ get; set; }
		public int y{ get; set; }

		public Point(){
			
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