using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunHandler : MonoBehaviour {
	public GameObject sun;
	public GameObject sunTarget;
	private float targetSpeedFast = 5, targetSpeedSlow = 2.5f;
	public Color sunColor0, sunColor1, sunColor2, sunColor3;
	private int sunState = 0;

	// Use this for initialization
	void Start () {
		RotateSun ();
		RecolorSun ();
	}
	
	// Update is called once per frame
	void Update () {
		//RotateSun ();

		//Testing
		/*
		if (Input.GetMouseButtonDown (0)) {
			//print ("Rotating the Sun.");
			MoveSun (0);
		} 
		else if (Input.GetMouseButtonDown (1)) {
			MoveSun (1);
		}
		*/

	}

	public void MoveSun (int speed) {
		/*"Sun Speed updated slow [0] or fast [1]"*/
		switch (speed) {
		case 0:
			MoveSunTarget (targetSpeedSlow);
			break;
		case 1:
			MoveSunTarget (targetSpeedFast);
			RecolorSun ();
			break;
		}
		RotateSun ();
	}

	void MoveSunTarget (float speed) {
		sunTarget.transform.position += new Vector3 (speed, 0, (speed * 2));
	}

	void RotateSun () {
		sun.transform.LookAt (sunTarget.transform);
	}

	public void RecolorSun (bool updateState=true) {
		//Debug.Log ("Sun: " + sunState);
		switch (sunState) {
		case 0:
			sun.GetComponent<Light> ().color = sunColor0;
			if (updateState) { sunState += 1; }
			break;
		case 1:
			sun.GetComponent<Light> ().color = sunColor1;
			if (updateState) { sunState += 1; }
			break;
		case 2:
			sun.GetComponent<Light> ().color = sunColor2;
			if (updateState) { sunState += 1; }
			break;
		case 3:
			sun.GetComponent<Light> ().color = sunColor3;
			if (updateState) { sunState += 1; }
			break;
		}
	}
}
