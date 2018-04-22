using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TokenHandler : MonoBehaviour {
	public GameObject self;

	public GameObject MapHandler;


	// Use this for initialization
	void Start () {

		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void MoveToTile (int tileX, int tileY) {
		// TEMP: Workaround
		if (MapHandler == null) {
			MapHandler = GameObject.Find ("Map00");
		}

		int tileHeight = 2;
		int expTileHeight = MapHandler.GetComponent<MapHandler>().GetTileHeight (tileX, tileY);
		if (expTileHeight > 0) {
			tileHeight = expTileHeight;
		}
			
		gameObject.transform.position = new Vector3 (tileX * 8, tileHeight, tileY*8);
	}
}
