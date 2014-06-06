using System;
using System.Collections;
using System.Collections.Generic;

namespace Circuits {

	public class NJfetElm : JfetElm {

		public NJfetElm(int xx, int yy, CirSim s) : base (xx, yy, false, s) {

		}

	}

	class PJfetElm : JfetElm {

		public PJfetElm(int xx, int yy, CirSim s) : base (xx, yy, true, s) {

		}

	}
}