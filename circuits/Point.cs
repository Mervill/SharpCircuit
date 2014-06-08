using System;
using System.Collections;
using System.Collections.Generic;

namespace Circuits {

	public class Point {

		public Weld weld;

		public Point(){
			
		}

		public void Soder(Point other){
			if(weld == null)
				weld = new Weld();

			other.weld = weld;
		}

		public bool SoderedTo(Point other){
			return weld == this.weld;
		}

	}

}