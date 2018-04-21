using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fan : MonoBehaviour {
	public GameObject fan;

	/* basic resources */
	private bool aware = false;
	private bool enraged = false;
	private bool influenced = false;

	/* movement */
	private uint speed = 2;

	private Vector2Int dateLoc;
	private Vector2Int promotionLoc;

	private Vector2Int[,] map;

	/* combat */
	private uint damage = 1;


	/* --- Methods --- */
	void DoDamage () {}

	bool IsPCAdjacent () {
		return false;
	}

	void MoveToLocation () {
		
	}

	void TakeTurn() {
		if (influenced) {
			MoveToLocation ();
		} else if (aware) {
			if (IsPCAdjacent()) {
				DoDamage ();
			} else {
				MoveToLocation ();
			}
		} else {
			RandomWalk ();
		}
	}

	void RandomWalk () {}


	/* --- Unity Life Cycle callbacks --- */
	// Use this for initialization
	void Start () {}

	// Update is called once per frame
	void Update () {}
}
