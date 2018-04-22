using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lead : MonoBehaviour {
	public bool isPC = false;

	public GameObject lead;
	public Lead date;

	// Delegate for all resources
	public delegate void ValueChanged(uint newval);

	// Helper
	private void FireChanged(ValueChanged tracker, uint value) {
		if (tracker != null) {
			tracker (value);
		}
	}

	/* basic resources */
	private uint selfEsteem = 3;
	public uint SelfEsteem
	{
		get { return selfEsteem; }
		set { selfEsteem = value; FireChanged (SelfEsteemTracker, value); }
	}
	public event ValueChanged SelfEsteemTracker;

	private uint fanCount = 300;
	public uint FanCount
	{
		get { return fanCount; }
		set { fanCount = value; FireChanged (FanCountTracker, value); }
	}
	public event ValueChanged FanCountTracker;


	/* date resources */
	private uint connection = 50;	// range: 0-100
	public uint Connection
	{
		get { return connection; }
		set { connection = value; FireChanged (ConnectionTracker, value); }
	}
	public event ValueChanged ConnectionTracker;


	private uint moves = 2;
	public uint Moves
	{
		get { return moves; }
		set { moves = value; FireChanged (MovesTracker, value); }
	}
	public event ValueChanged MovesTracker;


	/* special resources */
	private uint promotions = 3;
	private string[] perks;





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
