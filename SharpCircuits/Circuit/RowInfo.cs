using System;
using System.Collections;
using System.Collections.Generic;

namespace SharpCircuit {

	// Info about each row/column of the matrix for simplification purposes.
	public class RowInfo {

		public static readonly int ROW_NORMAL = 0; // Ordinary value.
		public static readonly int ROW_CONST  = 1; // Value is constant.
		public static readonly int ROW_EQUAL  = 2; // Value is equal to another value.

		public int nodeEq;
		public int type;
		public int mapCol;
		public int mapRow;
		public double value;
		public bool rsChanges; // Row's right side changes.
		public bool lsChanges; // Row's left side changes.
		public bool dropRow;   // Row is not needed in matrix.

		public RowInfo() {
			type = ROW_NORMAL;
		}
	}
}