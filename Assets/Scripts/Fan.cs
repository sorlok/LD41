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

	private Vector3 dateLoc;
	private Vector3 promotionLoc;

	/* combat */
	private uint damage = 1;


	/* --- Methods --- */
	void DoDamage () {}

	bool IsPCAdjacent () {
		return false;
	}

	void MoveToPromotion () {}

	void MoveToDate () {}

	void TakeTurn() {
		if (influenced) {
			MoveToPromotion ();
		} else if (aware) {
			if (IsPCAdjacent) {
				DoDamage ();
			} else {
				MoveToDate ();
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
