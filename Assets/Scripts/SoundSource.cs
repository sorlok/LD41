using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundSource : MonoBehaviour {
	public GameObject self;

	private float sourceTime, sourceDelay = 4;

	// Use this for initialization
	void Start () {
		sourceTime = Time.fixedTime;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if (Delayed(sourceDelay,sourceTime)) {
			Destroy(self);
		}
	}

	bool Delayed (float delay, float time) {
		if ((Time.fixedTime - time) >= delay) {
			return(true);
		} 
		else {
			return(false);
		}
	}
}
