using System;
using System.Collections;
using System.Collections.Generic;

namespace Circuits {

	public class NJfetElm : JfetElm {

		public NJfetElm( CirSim s) : base( false, s) {

		}

	}

	class PJfetElm : JfetElm {

		public PJfetElm( CirSim s) : base( true, s) {

		}

	}
}