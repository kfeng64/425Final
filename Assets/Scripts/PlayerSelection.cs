using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PlayerSelection
{
	private static int p1, p2;

	public static int P1Choice {
		get {
			return p1;
		}
		set {
			p1 = value;
		}
	}

	public static int P2Choice {
		get {
			return p2;
		}
		set {
			p2 = value;
		}
	}
}
