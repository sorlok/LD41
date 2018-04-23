using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TweetHandler : MonoBehaviour {
	public Camera mainCamera;
	public GameObject thisTweet;
	public GameObject tweetBody;
	public GameObject tweetBirdBurst;
	public GameState gameState;

	//Bursting
	private float burstTime, burstDelay = 1.2f;
	private bool bursting = false;

	//Tracking
	private bool tweetTracking = false;
	private float distance = 500;
	public Collider cast; //May not need.
	private int layerMask = 1 << 9;

	//Scaling
	private int minScale = 1, maxScale = 5, scaleDirection = 1;
	private float scaleSpeed = .2f;
	private bool scaling = false;

	//Body and Icon Adjustment
	public GameObject iconTop, bodyMid, bodyHigh;

	//Sending
	public bool sending = false;

	// Use this for initialization
	void Start () {
		//DeactivateTweet ();
		//thisTweet.SetActive(false);
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
		if (scaling) {
			ScaleTweet ();
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
		thisTweet.transform.localScale = new Vector3 (minScale, minScale, minScale);
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
			//DeactivateTweetBody ();
			DeactivateTweetBurst ();
			DeactivateTweet ();
			tweetTracking = false;
			scaling = false;
		}
	}

	public void StartTweeting () {
		//thisTweet.transform.position = new Vector3 (origin.x, thisTweet.transform.position.y, origin.z);
		if (!thisTweet.activeSelf) {
			sending = false;
			scaling = true;
			ManageScale ();
			tweetTracking = true;
			ActivateTweet ();
		}
	}

	void HandleTweeting () {
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
		if (Physics.Raycast (ray, out hit, distance, layerMask)) {
			thisTweet.transform.position = new Vector3 (hit.point.x, thisTweet.transform.position.y, hit.point.z);
		}
	}

	void SendTweet () {
		//print ("Tweet.");
		sending = true;
		tweetTracking = false;
		scaling = false;
		//ActivateTweetBody ();
		ActivateTweetBurst ();

		//Update State
		gameState.SetupDateTurn();
	}

	void ScaleTweet () {
		//Scale x, y, z should always be the same.
		if (scaleDirection == 1) {
			//Scaling up.
			if (thisTweet.transform.localScale.x < maxScale) {
				thisTweet.transform.localScale += new Vector3 (scaleSpeed, scaleSpeed, scaleSpeed);
			} else {
				thisTweet.transform.localScale = new Vector3 (maxScale, maxScale, maxScale);
				scaleDirection = -1;
			}
		} else if (scaleDirection == -1) {
			//Scaling down.
			if (thisTweet.transform.localScale.x > minScale) {
				thisTweet.transform.localScale -= new Vector3 (scaleSpeed, scaleSpeed, scaleSpeed);
			} else {
				thisTweet.transform.localScale = new Vector3 (minScale, minScale, minScale);
				scaleDirection = 1;
			}
		}
		ManageScale ();
	}

	void ManageScale () {
		iconTop.transform.position = new Vector3 (iconTop.transform.position.x, 5, iconTop.transform.position.z);
		bodyMid.transform.position = new Vector3 (bodyMid.transform.position.x, 3, bodyMid.transform.position.z);
		bodyHigh.transform.position = new Vector3 (bodyHigh.transform.position.x, 6, bodyHigh.transform.position.z);
	}
}
