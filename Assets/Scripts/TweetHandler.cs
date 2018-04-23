using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TweetHandler : MonoBehaviour {
	public Camera mainCamera;
	public GameObject thisTweet;
	public GameObject tweetBody;
	public GameObject tweetBirdBurst;

	private float burstTime, burstDelay = 1.5f;
	private bool bursting = false;
	private bool tweetTracking = false;
	private float distance = 500;
	public Collider cast;

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
		/*
		if (Input.GetMouseButtonDown (1)) {
			StartTweeting ();
		}
		*/
		if (Input.GetMouseButtonDown (0)) {
			SendTweet();
		}

		if (tweetTracking) {
			HandleTweeting ();
		}

		HandleTweetBurst();
	}

	void ActivateTweetBody () {
		//print ("Body active.");
		tweetBody.SetActive (true);
	}

	void DeactivateTweet () {
		//print ("Body inactive.");
		thisTweet.SetActive (false);
	}

	void ActivateTweet () {
		//print ("Body active.");
		thisTweet.SetActive (true);
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
			DeactivateTweet ();
			tweetTracking = false;
		}
	}

	public void StartTweeting () {
		//thisTweet.transform.position = new Vector3 (origin.x, thisTweet.transform.position.y, origin.z);
		if (!thisTweet.activeSelf) {
			tweetTracking = true;
			ActivateTweet ();
		}
	}

	void HandleTweeting () {
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
		if (cast.Raycast (ray, out hit, distance)) {
			if (hit.collider.CompareTag ("Cast")) {
				//Debug.DrawLine (ray.origin, hit.point);
				//Debug.Log (hit.point);
				thisTweet.transform.position = new Vector3 (hit.point.x, thisTweet.transform.position.y, hit.point.z);
			}
		}
	}

	void SendTweet () {
		//print ("Tweet.");
		tweetTracking = false;
		ActivateTweetBody ();
		ActivateTweetBurst ();
	}
}
