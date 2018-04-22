using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TokenHandler : MonoBehaviour {
	public GameObject self;
	public GameObject MapHandler;

	// Either of these is non-null, depending on the type. 
	// Use LeadPlayerScript/etc. from MapHandler
	public Lead LeadObj;
	public Fan FanObj;

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

	// As a fan, walk randomly (avoiding going out of bounds, non-steppable tiles, and other fans)
	public void RandomWalkFan() {
		// Which tiles can we move to
		List<IntPoint> allowedTileMoves = new List<IntPoint>();
		foreach (char s in "NSEW") {
			IntPoint newPos = IntPoint.FromCardinal (TileX, TileY, s);
			if (MapHandler.GetComponent<MapHandler> ().CanMove (newPos)) {
				allowedTileMoves.Add (newPos);
			}
		}

		// No moves?
		if (allowedTileMoves.Count == 0) {
			// TEMP
			gameObject.transform.RotateAround (gameObject.transform.position, Vector3.forward, 90f);


			MovedThisTurn = true;
			return;
		}

		// Get one
		IntPoint pt = allowedTileMoves[GameState.rng.Next(allowedTileMoves.Count)];
		MoveToTile (pt.x, pt.y);
	}

	public void MoveToTile (int tileX, int tileY) {
		int tileHeight = 2;
		int expTileHeight = MapHandler.GetComponent<MapHandler>().GetTileHeight (tileX, tileY);
		if (expTileHeight > 0) {
			tileHeight = expTileHeight;
		}

		this.tileX = tileX;
		this.tileY = tileY;
			
		gameObject.transform.position = new Vector3 (tileX * 8, tileHeight, tileY*8);
		MovedThisTurn = true;
	}
}
