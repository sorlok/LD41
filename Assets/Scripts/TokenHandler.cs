using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TokenHandler : MonoBehaviour {
	public GameObject self;

	public GameObject MapHandler;

	// Did our fan move this turn?
	public bool MovedThisTurn = false;

	// What are the tile x/y positions of this Token?
	private int tileX = 0;
	public int TileX 
	{
		get { return tileX; }
	}
	private int tileY = 0;
	public int TileY
	{
		get { return tileY; }
	}

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

		this.tileX = tileX;
		this.tileY = tileY;
			
		gameObject.transform.position = new Vector3 (tileX * 8, tileHeight, tileY*8);
	}
}
