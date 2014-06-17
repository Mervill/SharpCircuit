using System;
using System.Collections;
using System.Collections.Generic;

namespace Circuits {

	public class PushSwitchElm : SwitchElm {

		public ElementLead leadIn 	{ get { return leads[0]; }}
		public ElementLead leadOut 	{ get { return leads[1]; }}

		public PushSwitchElm(CirSim s) : base(s,true) {

		}
	
	}
}