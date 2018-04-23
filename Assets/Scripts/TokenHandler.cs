using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TokenHandler : MonoBehaviour {
	public GameObject self;
	public GameObject selfRender;

	public GameObject MapHandler;

	//Token Removal
	private int minDistance = 100, maxDistance = 180, minHeight = 80, maxHeight = 160;
	private bool removing = false;
	private float removalDelay = 5, removalTime;
	private float rotateSpeed = 30;
	private Vector2 removalTarget;
	private float targetDistance;
	private float xShift, zShift, yShift;
	private int heightDirection = 1;
	private float heightDistance;

	// Either of these is non-null, depending on the type. 
	// Use LeadPlayerScript/etc. from MapHandler
	public Lead LeadObj;
	public Fan FanObj;
	public TweetHandler tweetHandler;
	private bool highlighting = false;
	public MeshRenderer tokenRender;
	public Material fanMaterialOn, fanMaterialOff;
	public Material leadMaterialOn, leadMaterialOff;

	private bool leadHighlighting = false;

	//Lead Highlighting
	private float highlightTime, highlightDelay = .3f;

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
		//tokenMaterialOff = tokenRender.material;
		selfRender = self.transform.GetChild(0).gameObject;
		//
		//ShiftToken();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if (removing) {
			HandleRemoval ();
		}

		if (LeadObj != null) {
			if (Delayed (highlightDelay, highlightTime)) {
				HideToken ();
			}
		}
		//FlipToken ();
	}

	// As a fan, walk towards a given other fan or player
	public bool FanWalkTowards(GameObject otherObj1, GameObject otherObj2) {
		// Try to take 1 step in our plan.
		if (FanMakeNextMove ()) {
			return true;
		}

		// TEMP
		//gameObject.transform.RotateAround (gameObject.transform.position, Vector3.forward, 90f);
		return false;
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


	// More accurate BFS lookup
	private List<IntPoint> MakeNewTravelPlan() {
		MapHandler mapHnd = MapHandler.GetComponent<MapHandler>();


		// TODO: Allow appearing NEXT to either of these.
		TokenHandler other1 = mapHnd.LeadDate.GetComponent<TokenHandler>();
		TokenHandler other2 = mapHnd.LeadPlayer.GetComponent<TokenHandler>();


		Vector2Int start = new Vector2Int (TileX, TileY);
		Vector2Int dest = new Vector2Int (other1.TileX, other1.TileY);
		int infinity = 100 * 100;

		// Step 1: Make a new array with the "moves from destination" count.
		List<int> lookup = new List<int>();
		for (int i = 0; i < mapHnd.mapTileWidth * mapHnd.mapTileHeight; i++) {
			lookup.Add (infinity);
		}

		// Step 1.1: Start at the destination and work outwards
		List<Vector2Int> todo = new List<Vector2Int>();
		Dictionary<Vector2Int, bool> blacklist = new Dictionary<Vector2Int, bool>();
		todo.Add (dest);
		blacklist.Add (dest, true);
		lookup [mapHnd.GetTileIndex (dest.x, dest.y)] = 0;
		while (todo.Count > 0) {
			// Next tile to consider
			Vector2Int next = todo [0];
			todo.RemoveAt (0);

			// Consider all 4 neighbors
			foreach (char dir in "NSEW") {
				IntPoint newPosI = IntPoint.FromCardinal (next.x, next.y, dir);
				Vector2Int newPos = new Vector2Int (newPosI.x, newPosI.y);

				// Can we move here normally?
				if (!mapHnd.CanMove (newPosI)) {
					continue;
				}

				// Have we already queued up this tile?
				if (blacklist.ContainsKey(newPos)) {
					continue;
				}

				// Ok, enqueue it
				todo.Add (newPos);
				blacklist.Add (newPos, true);
				int newDist = lookup [mapHnd.GetTileIndex (next.x, next.y)] + 1;
				lookup [mapHnd.GetTileIndex (newPos.x, newPos.y)]  = newDist;

				// TEMP: Useful debugging
				if (false) { //mapHnd.showDebugMoves
					mapHnd.debugArray [mapHnd.GetTileIndex (newPos.x, newPos.y)].text = "" + newDist;
				}
			}
		}

		// Step 2: Start from the source, and try to find a path forward.
		// Choose the least-cost pat at east junction, unless there is none.
		List<IntPoint> res = new List<IntPoint>();
		IntPoint curr = new IntPoint (start.x, start.y);
		while (true) {
			// Are we done?
			if (lookup [mapHnd.GetTileIndex (curr.x, curr.y)] == 1) {
				return res;
			}

			// Find the next jump
			List<IntPoint> options = new List<IntPoint>();
			foreach (char dir in "NSEW") {
				IntPoint next = IntPoint.FromCardinal (curr.x, curr.y, dir);
				if (next.x >= 0 && next.x < mapHnd.mapTileWidth && next.y >= 0 && next.y < mapHnd.mapTileHeight) {
					int myDist = lookup [mapHnd.GetTileIndex (next.x, next.y)];
					if (myDist != infinity) {
						// Compare it to the current best distance.
						if (options.Count > 0) {
							int otherDist = lookup [mapHnd.GetTileIndex (options [0].x, options [0].y)];
							if (myDist < otherDist) {
								// It's strictly better
								options.Clear();
							}
							if (myDist <= otherDist) {
								// It's tied or better (we're considering it)
								options.Add(next);
							}
						} else {
							// No contest
							options.Add(next);
						}
					}
				}
			}

			// Take it
			if (options.Count > 0) {
				IntPoint next = options[GameState.rng.Next(options.Count)];
				res.Add (next);
				curr = next;
			} else {
				// Shouldn't happen
				break;
			}
		}

		return null;
	}


	// Do a BFS lookup and keep one of the best paths.
	// Store the result in FanObj.TravelPlan
	private void MakeTravelPlan() {
		FanObj.TravelPlan = null;

		// TEMP
		FanObj.TravelPlan = MakeNewTravelPlan();
		return;
		// END TEMP

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

	public void RemoveToken () {
		removing = true;
		removalTime = Time.fixedTime;
		//print ("Token Removed.");
		MapHandler.GetComponent<MapHandler> ().DestroyFan (self);

		ShiftToken ();
		heightDistance = Random.Range (minHeight, maxHeight);
		targetDistance = Random.Range (minDistance, maxDistance);
		removalTarget = Random.insideUnitCircle * targetDistance;
		SetZShift ();
		SetXShift ();
		SetYShift ();
	}

	public void HighlightToken () {
		highlighting = true;
		tokenRender.material = fanMaterialOn;
		highlightTime = Time.fixedTime;
		//print ("Token Highlighted.");
	}
	public void HideToken () {
		highlighting = false;
		tokenRender.material = fanMaterialOff;
		//print ("Token Hidden.");

	}

	void OnTriggerStay (Collider other) {
		//Remove Token if Tweet Trigger and Fan interact.
		if (FanObj != null && other.tag == "Tweet") {
			if (tweetHandler.sending && !removing) {
				RemoveToken ();
			}
		}
	}

	void OnTriggerEnter (Collider other) {
		if (FanObj != null && other.tag == "Tweet") {
			if (!tweetHandler.sending && !highlighting) {
				HighlightToken();
			}
		}
	}

	void OnTriggerExit (Collider other) {
		if (FanObj != null && other.tag == "Tweet") {
			if (!tweetHandler.sending && highlighting) {
				HideToken();
			}
		}
	}

	void DestroyToken () {
		removing = false;
		Destroy (self);
	}

	void HandleRemoval () {		
		FlipToken ();
		MoveToken ();
		if (Delayed(removalDelay, removalTime)) {
			DestroyToken ();
		}
	}

	void ShiftToken () {
		int yShift = Random.Range (0, 360);
		self.transform.localEulerAngles = new Vector3 (0,yShift,0);
	}

	void FlipToken () {
		//print ("Flipping");
		//selfRender.transform.RotateAround (selfRender.transform.position, Vector3.left, 1600 * Time.deltaTime);
		selfRender.transform.Rotate (rotateSpeed, 0, 0, Space.Self);
	}

	bool Delayed (float delay, float time) {
		if ((Time.fixedTime - time) >= delay) {
			return(true);
		} 
		else {
			return(false);
		}
	}
	void SetXShift () {
		float xDelta = (self.transform.position.x - removalTarget.x);
		xShift = (xDelta/200f);
	}
	void SetZShift () {
		float zDelta = (self.transform.position.z - removalTarget.y);
		zShift = (zDelta/200f);
	}
	void SetYShift () {
		yShift = (heightDistance / 50f);
	}
	void MoveToken() {
		//Horizontal Movement.
		self.transform.position += new Vector3((xShift),(0),(zShift));
		//Vertical Movement.
		if (heightDirection == 1) {
			self.transform.position += new Vector3 ((0), (yShift), (0));
			if (self.transform.position.y >= heightDistance) {
				heightDirection = -1;
				self.transform.position = new Vector3 (self.transform.position.x, heightDistance, self.transform.position.z);
			}
		} else if (heightDirection == -1) {
			self.transform.position += new Vector3 ((0), (-yShift), (0));
		}
	}

}
