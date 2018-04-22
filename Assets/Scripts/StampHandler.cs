using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StampHandler : MonoBehaviour {
	public GameObject thisStamp;
	public Button buttonA, buttonB, buttonC;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void ShowStamp () {
		DisableButtons ();
		thisStamp.SetActive (true);
	}

	public void HideStamp () {
		EnableButtons ();
		thisStamp.SetActive (false);
	}

	public void DisableButtons () {
		if (buttonA != null) {
			buttonA.interactable = false;
		}
		if (buttonB != null) {
			buttonB.interactable = false;
		}
		if (buttonC != null) {
			buttonC.interactable = false;
		}
	}

	public void EnableButtons () {
		if (buttonA != null) {
			buttonA.interactable = true;
		}
		if (buttonB != null) {
			buttonB.interactable = true;
		}
		if (buttonC != null) {
			buttonC.interactable = true;
		}
	}
}
