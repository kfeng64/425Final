using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PlayerSelection
{
	private static int p1, p2, p1Wins, p2Wins;

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

	public static int p1WinCount {
		get {
			return p1Wins;
		}
		set {
			p1Wins = value;
		}
	}

	public static int p2WinCount {
		get {
			return p2Wins;
		}
		set {
			p2Wins = value;
		}
	}
}
