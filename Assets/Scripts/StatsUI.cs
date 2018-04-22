using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

public class StatsUI : MonoBehaviour {
	private Lead player = new Lead();

	public GameObject fanCountLabel;

	// --- Methods ---
	void FanCountChanged(uint newVal) {
		fanCountLabel.GetComponent<Text>().text = "" + newVal;
	}

	void SelfEsteemChanged(uint newVal) {
		// update ui
	}


	// --- Unity Lifecycle Callbacks ---
	// Use this for initialization
	void Start () {
		player.SelfEsteemTracker += SelfEsteemChanged;
		player.FanCountTracker += FanCountChanged;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
