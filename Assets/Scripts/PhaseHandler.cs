using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PhaseHandler : MonoBehaviour {
	//public GameState gameHandler;
	public SunHandler sun;

	public Text turnCountText, activeUserText;
	public float thisMinute = 0, thisHour = 7;

	//private string you, 

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void UpdateMinute () {
		thisMinute += 5;
		sun.MoveSun (0);
	}

	public void UpdateHour () {
		thisHour += 1;
		sun.MoveSun (1);
	}

	public void UpdateColoring() {
		//sun.RecolorSun (false); // not needed
	}

	public void UpdateTime () {
		string time;
		if (thisMinute < 10) {
			time = "" + thisHour + ":" + "0" + thisMinute + "pm";
		} 
		else {
			time = "" + thisHour + ":" + thisMinute + "pm";
		}
		turnCountText.text = time;
	}

	public void UpdateActiveUser (string phaseName) {
		activeUserText.text = phaseName;
	}
}
