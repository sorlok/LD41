using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lead : MonoBehaviour {
	public bool isPC = false;

	public GameObject lead;
	public Lead date;

	/* basic resources */
	private uint selfEsteem = 3;
	private uint fanCount = 300;

	/* date resources */
	private uint connection = 50;	// range: 0-100
	private uint moves = 2;

	/* special resources */
	private uint promotions = 3;
	private string[] perks;

	/* movement */
	public Vector2Int currentLoc;


	/* --- Methods --- */
	void DoInstagramPromotion () {}

	void EndDate () {}

	void TakeTurn () {
		if (selfEsteem < 1) {
			EndDate ();
		}

		// interact with social media
		if (Random.Range (0, 101) < connection) {
			Tweet ();
		// interact with date
		} else {
			TalkToDate ();
		}
	}

	void TalkToDate () {}

	void Tweet () {}

	void TweetDiscouragement () {}

	void TweetEncouragment () {}

	void TweetPromotion () {}


	/* --- Unity Life Cycle callbacks --- */
	// Use this for initialization
	void Start () {}
	
	// Update is called once per frame
	void Update () {}
}
