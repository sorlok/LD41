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

	// As a fan, walk towards a given other fan or player
	public void FanWalkTowards(GameObject otherObj) {
		// Try to take 1 step in our plan.
		if (FanMakeNextMove ()) {
			return;
		}

		// TEMP
		gameObject.transform.RotateAround (gameObject.transform.position, Vector3.forward, 90f);
		return;
	}

	// Try to make the next move in the current plan, recalculating once.
	public bool FanMakeNextMove() {
		if (FanObj.TravelPlan == null) {
			MakeTravelPlan ();
		} else {
			bool planOk = true;
			foreach (IntPoint pt in FanObj.TravelPlan) {
				if (! MapHandler.GetComponent<MapHandler>().CanMove(pt)) {
					planOk = false;
					break;
				}
			}
			if (!planOk) {
				MakeTravelPlan();
			}
		}

		// Try to make the move
		if (FanObj.TravelPlan != null && FanObj.TravelPlan.Count>0) {
			MoveToTile (FanObj.TravelPlan[0]);
			FanObj.TravelPlan.RemoveAt (0);
			return true;
		}

		return false;
	}


	// Do a BFS lookup and keep one of the best paths.
	// Store the result in FanObj.TravelPlan
	private void MakeTravelPlan() {
		FanObj.TravelPlan = null;

		// Pass the map to the search algorithm
		MapHandler handler = MapHandler.GetComponent<MapHandler>();
		TokenHandler other = handler.LeadDate.GetComponent<TokenHandler>();
		BFSTree tree = new BFSTree (new Vector2Int(TileX, TileY), new Vector2Int(other.TileX,other.TileY), handler);

		// Now walk down the tree and select a path (destination).
		List<IntPoint> res = new List<IntPoint>();
		BFSTreeNode curr = tree.root;
		while (!curr.isDest) {
			// Pick the smallest distance.
			int minDistance = 100*100;
			int[] distances = curr.distances.ToArray ();
			foreach (int distance in distances) {
				if (distance < minDistance) {
					minDistance = distance;
				}
			}

			// Pick the next direction
			List<BFSTreeNode> possibleChoices = new List<BFSTreeNode>();
			for (int i = 0; i < curr.children.Count; i++) {
				if (distances[i] == minDistance) {
					possibleChoices.Add (curr.children[i]);
				}
			}

			// make choice
			if (possibleChoices.Count > 0) {
				curr = possibleChoices [GameState.rng.Next (possibleChoices.Count)];
				res.Add (new IntPoint (curr.id.x, curr.id.y));
			} else {
				curr = null;
				break;
			}
		}

		// Did we find it?
		if (curr != null) {
			FanObj.TravelPlan = res;

			Debug.Log ("FOUND: " + FanObj.TravelPlan);
		}
	}


	// As a fan, walk randomly (avoiding going out of bounds, non-steppable tiles, and other fans)
	public void FanRandomWalk() {
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
			return;
		}

		// Get one
		IntPoint pt = allowedTileMoves[GameState.rng.Next(allowedTileMoves.Count)];
		MoveToTile (pt.x, pt.y);
	}

	public void MoveToTile (IntPoint pos) {
		MoveToTile (pos.x, pos.y);
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
	}
}
