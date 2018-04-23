using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TweetHandler : MonoBehaviour {
	public GameObject thisTweet;
	public GameObject tweetBody;
	public GameObject tweetBirdBurst;

	private float burstTime, burstDelay = 1.5f;
	private bool bursting = false;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		//Testing
		/*
		if (Input.GetMouseButtonDown (0)) {
			//print ("Tweet Body Active.");
			ActivateTweetBody ();

		} 
		else if (Input.GetMouseButtonDown (1)) {
			ActivateTweetBurst();
		}
		*/
		//Tweet Bird Burst Handler
		HandleTweetBurst();
	}

	void ActivateTweetBody () {
		//print ("Body active.");
		tweetBody.SetActive (true);
	}

	void DeactivateTweet () {
		//print ("Body inactive.");
		tweetBody.SetActive (false);
	}

	void ActivateTweet () {
		//print ("Body active.");
		tweetBody.SetActive (true);
	}

	void DeactivateTweetBody () {
		//print ("Body inactive.");
		tweetBody.SetActive (false);
	}

	void ActivateTweetBurst () {
		//print ("Burst active.");
		tweetBirdBurst.SetActive (true);
		burstTime = Time.fixedTime;
		bursting = true;
	}

	void DeactivateTweetBurst () {
		//print ("Burst inactive.");
		tweetBirdBurst.SetActive (false);
		//burstTime = Time.fixedTime;
		bursting = false;
	}

	bool Delayed (float delay, float time) {
		if ((Time.fixedTime - time) >= delay) {
			return(true);
		} 
		else {
			return(false);
		}
	}

	void HandleTweetBurst () {
		if (bursting && Delayed (burstDelay, burstTime)) {
			DeactivateTweetBody ();
			DeactivateTweetBurst ();
		}
	}
}
