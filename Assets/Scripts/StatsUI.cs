using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

public class StatsUI : MonoBehaviour {
	private Lead player;

	public GameObject selfEsteemLabel;
	public GameObject fanCountLabel;

	public GameObject mapHandler;


	// --- Methods ---
	void FanCountChanged(uint newVal) {
		fanCountLabel.GetComponent<Text>().text = "" + newVal;
	}

	void SelfEsteemChanged(uint newVal) {
		selfEsteemLabel.GetComponent<Text>().text = "" + newVal;
	}


	// --- Unity Lifecycle Callbacks ---
	// Use this for initialization
	void Start () {
		gameObject.SetActive (true);

		player = mapHandler.GetComponent<MapHandler>().LeadPlayerScript;

		player.SelfEsteemTracker += SelfEsteemChanged;
		player.FanCountTracker += FanCountChanged;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
