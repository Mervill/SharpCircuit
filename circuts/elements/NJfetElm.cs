using System;
using System.Collections;
using System.Collections.Generic;

namespace Circuts {

	public class NJfetElm : JfetElm {

		public NJfetElm(int xx, int yy, CirSim s) : base (xx, yy, false, s) {

		}

	}

	class PJfetElm : JfetElm {

		public PJfetElm(int xx, int yy, CirSim s) : base (xx, yy, false, s) {

		}

	}
}