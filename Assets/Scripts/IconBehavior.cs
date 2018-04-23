using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IconBehavior : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}

	// Update is called once per frame
	void Update () {
		this.gameObject.transform.position += new Vector3 (0f, 0.1f, 0f);
		if (this.gameObject.transform.position.y > 10f) {
			Destroy(this.gameObject);
		}
	}
}
