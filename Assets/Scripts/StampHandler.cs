using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StampHandler : MonoBehaviour {
	public GameObject thisStamp;
	public Button buttonA, buttonB, buttonC;
	public int MyStampId;

	// For calling back to GameState
	public GameObject GameHandler;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void ShowStamp () {
		// Hack: wait for button fading to be done.
		GameState.ActState state = GameHandler.GetComponent<GameState>().CurrState;
		if (state == GameState.ActState.FadingTextIn || state == GameState.ActState.FadingTextOut || state == GameState.ActState.DoingInterp) {
			return;
		}

		DisableButtons ();
		thisStamp.SetActive (true);

		// Perform the expected action
		GameHandler.GetComponent<GameState>().ReactToStamp(MyStampId);
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
