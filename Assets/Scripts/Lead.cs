using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lead {
	public GameState state;
	public GameObject lead;

	public bool isPC = false;

	// Delegate for all resources
	public delegate void ValueChanged(int newval);
	public delegate void ValueChanged2(string newval);

	// Helper
	private void FireChanged(ValueChanged tracker, int value) {
		if (tracker != null) {
			tracker (value);
		}
	}
	private void FireChanged(ValueChanged2 tracker, string value) {
		if (tracker != null) {
			tracker (value);
		}
	}

	public void ClearAllEvents() {
		SelfEsteemTracker = null;
		FanCountTracker = null;
		ConnectionTracker = null;
		MovesTracker = null;
	}

	/* basic resources */
	private string atmosphere = "C-";
	public string Atmosphere 
	{
		get { return atmosphere; }
		set { atmosphere = value; FireChanged (AtmosphereTracker, value); }
	}
	public event ValueChanged2 AtmosphereTracker;

	private int selfEsteem = 3;
	public int SelfEsteem
	{
		get { return selfEsteem; }
		set { selfEsteem = value; FireChanged (SelfEsteemTracker, value); }
	}
	public event ValueChanged SelfEsteemTracker;

	private int fanCount = 300;
	public int FanCount
	{
		get { return fanCount; }
		set { fanCount = value; FireChanged (FanCountTracker, value); }
	}
	public event ValueChanged FanCountTracker;


	/* date resources */
	private int connection = 50;	// range: 0-100
	public int Connection
	{
		get { return connection; }
		set { connection = value; FireChanged (ConnectionTracker, value); }
	}
	public event ValueChanged ConnectionTracker;

	private int moves = 2;
	public int Moves
	{
		get { return moves; }
		set { moves = value; FireChanged (MovesTracker, value); }
	}
	public event ValueChanged MovesTracker;

	public Lead date;				// if PC, 'date' object is date; otherwise, 'date' object is PC


	/* special resources */
	//private uint promotions = 3;
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
		if (Random.Range (0, 101) < 51) {
			Tweet ();
		// interact with date
		} else {
			TalkToDate ();
		}
	}

	// This method describes behavior of your date talking to you.
	void TalkToDate () {
		bool affectYou = Random.Range (0, 2) == 0;
		bool affectEsteem = Random.Range (0, 2) == 0;
		int rollTalk = Random.Range (0, 101);

		if (affectYou) {
			if (affectEsteem) {
				date.SelfEsteem = date.SelfEsteem + (int)(rollTalk < connection ? 1 : -1);
			} else {
				date.FanCount = date.FanCount + (int)(rollTalk < connection ? 100 : -100);
			}
		} else {
			if (affectEsteem) {
				SelfEsteem = SelfEsteem + (int)(rollTalk < connection ? 1 : -1);
			} else {
				FanCount = FanCount + (int)(rollTalk < connection ? 100 : -100);
			}
		}
	}

	void Tweet () {}

	void TweetDiscouragement () {}

	void TweetEncouragment () {}

	void TweetPromotion () {}
}
