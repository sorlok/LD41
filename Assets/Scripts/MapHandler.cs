using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Helper class for representing movement easily.
public class IntPoint
{
	public IntPoint(int x, int y) 
	{
		this.x = x;
		this.y = y;
	}
	public int x;
	public int y;

	public static IntPoint FromCardinal(int x, int y, char dir) {
		if (dir == 'N') { return new IntPoint (x+1, y); }
		if (dir == 'S') { return new IntPoint (x-1, y); }
		if (dir == 'E') { return new IntPoint (x, y-1); }
		if (dir == 'W') { return new IntPoint (x, y+1); }
		return new IntPoint (x, y);
	}
}

public class MapHandler : MonoBehaviour {
	//public bool showDebugMoves = false;

	public List<int> mapTileValues = new List<int>();

	// 
	public List<int> mapTileHeights = new List<int>();

	public List<TextMesh> debugArray = new List<TextMesh>();

	public int mapTileWidth = 10;
	public int mapTileHeight = 10;

	//Destroying Tokens
	//public List<TokenHandler> dyingTokens = new List<TextMesh>();

	// Used to create the lead/fan
	public GameObject LeadPrefab0M;
	public GameObject LeadPrefab1M;
	public GameObject LeadPrefab2M;
	public GameObject LeadPrefab0W;
	public GameObject LeadPrefab1W;
	public GameObject LeadPrefab2W;
	public GameObject FanPrefab;
	public GameObject DebugPrefab;

	public GameObject heartPrefab;

	// Actual Lead tokens
	private GameObject leadPlayer;
	public GameObject LeadPlayer
	{
		get { return leadPlayer; }
		set { CallClearAllEvents (leadPlayer); leadPlayer = value; }
	}
		
	private GameObject leadDate;
	public GameObject LeadDate
	{
		get { return leadDate; }
		set { CallClearAllEvents (leadDate); leadDate = value; }
	}

	//Tweet Handler
	public TweetHandler tweetHandler;

	// Handy helper; just get the lead players' scripts
	public Lead LeadPlayerScript 
	{
		get { return leadPlayer.GetComponent<TokenHandler> ().LeadObj; }
	}
	public Lead LeadDateScript 
	{
		get { return leadDate.GetComponent<TokenHandler> ().LeadObj; }
	}

	// Fans are stored when created
	private List<GameObject> fans = new List<GameObject>();
	public int FanCount
	{
		get { return fans.Count; }
	}
	public GameObject Fan(int id) 
	{
		return fans [id];
	}
	public Fan FanScript(int id)
	{
		return fans [id].GetComponent<TokenHandler> ().FanObj;
	}

	public AudioSource sfxSource;
	public AudioClip[] movementSFX;


	// Helper
	private void CallClearAllEvents(GameObject gobj) {
		if (gobj != null) {
			gobj.GetComponent<Lead>().ClearAllEvents ();
		}
	}

	public int GetTileValue(int tileX, int tileY) {
		int id = GetTileIndex (tileX, tileY);
		return GetTileValue (mapTileValues, id);
	}

	public int GetTileHeight(int tileX, int tileY) {
		int id = GetTileIndex (tileX, tileY);
		return GetTileValue (mapTileHeights, id);
	}

	public int GetTileIndex(int tileX, int tileY) {
		if (tileX < 0 || tileX >= mapTileWidth || tileY < 0 || tileY >= mapTileHeight) {
			return -1;
		}
		return tileY * mapTileWidth + tileX;
	}

	private int GetTileValue(List<int> list, int id) {
		if (id >= 0 && id < list.Count) {
			return list [id];
		}
		return -1;
	}


	// Use this for initialization
	void Start () {
		// TEMP: Some useful debug code
		if (false) { //showDebugMoves
			for (int i = 0; i < mapTileWidth * mapTileHeight; i++) {
				debugArray.Add (null);
			}
			for (int y=0; y<mapTileHeight; y++) {
				for (int x = 0; x < mapTileWidth; x++) {
					GameObject res = Instantiate (DebugPrefab, new Vector3 (x*8, 1, y*8), Quaternion.identity);
					TextMesh mesh = res.GetComponentInChildren<TextMesh> ();
					mesh.text = "";
					debugArray[GetTileIndex (x,y)] = mesh;
				}
			}
		}
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	// Load a given level
	public void LoadMap1() {
		// Load the map data
		// 0 = Grass/Road/etc.
		// 1 = House
		mapTileValues = new List<int> {
			0, 0, 0, 0, 0, 0, 0, 0, 0, 0,  //0
			0, 0, 0, 0, 0, 0, 0, 1, 0, 0,  //1
			0, 0, 0, 0, 0, 1, 1, 0, 0, 0,  //2
			0, 0, 0, 0, 0, 0, 0, 0, 0, 0,  //3
			0, 0, 0, 1, 0, 1, 0, 0, 1, 0,  //4
			0, 0, 0, 1, 0, 1, 1, 0, 1, 0,  //5
			0, 0, 0, 1, 0, 1, 1, 0, 1, 0,  //6
			0, 0, 0, 0, 0, 0, 0, 0, 0, 0,  //7
			0, 0, 0, 0, 1, 1, 1, 0, 1, 0,  //8
			0, 0, 0, 0, 0, 0, 0, 0, 0, 0,  //9
		};


		// For tile heights:
		// 2 = default, "on the ground"
		// 5 = on a hill, or on a house
		// 8 = on a house that's on a hill (shouldn't happen)
		mapTileHeights = new List<int> {
			2, 2, 2, 2, 2, 2, 2, 2, 2, 2,  //0
			2, 2, 2, 2, 2, 2, 2, 5, 2, 2,  //1
			2, 2, 2, 2, 2, 5, 5, 2, 2, 2,  //2
			5, 5, 5, 2, 2, 2, 2, 2, 2, 2,  //3
			2, 2, 2, 5, 2, 5, 2, 2, 5, 2,  //4
			2, 2, 2, 5, 2, 5, 5, 2, 5, 2,  //5
			2, 2, 2, 5, 2, 5, 5, 2, 5, 2,  //6
			2, 2, 2, 2, 2, 2, 2, 2, 2, 2,  //7
			2, 2, 2, 2, 5, 5, 5, 2, 5, 2,  //8
			2, 2, 2, 2, 2, 2, 2, 2, 2, 2,  //9
		};

		// Sanity check
		if (mapTileWidth * mapTileHeight != mapTileValues.Count) {
			throw new System.ArgumentException ("Bad map width/height and array sizes [1]");
		}
		if (mapTileWidth * mapTileHeight != mapTileHeights.Count) {
			throw new System.ArgumentException ("Bad map width/height and array sizes [2]");
		}

		// Create the players, and some fans
		leadPlayer = CreateLead (1, 3);
		leadDate = CreateLead (2, 3);

		SpawnFans (3);
	}


	private int getMinDistFromPlayer(IntPoint fan) {
		int dist1 = Mathf.Abs(fan.x - LeadDate.GetComponent<TokenHandler>().TileX) + Mathf.Abs(fan.y - LeadDate.GetComponent<TokenHandler>().TileY);
		int dist2 = Mathf.Abs(fan.x - LeadPlayer.GetComponent<TokenHandler>().TileX) + Mathf.Abs(fan.y - LeadPlayer.GetComponent<TokenHandler>().TileY);
		if (dist1 < dist2) {
			return dist1;
		} else {
			return dist2;
		}
	}


	public void SpawnFans(int count) {
		// Save allowed fan locations
		List<IntPoint> fanSpawns = new List<IntPoint>();
		for (int y = 0; y < mapTileHeight; y++) {
			for (int x = 0; x < mapTileWidth; x++) {
				IntPoint res = new IntPoint (x, y);
				if (CanMove (res)) {
					// Additional safety: not too close to dates
					int amt = 4;
					int dist = getMinDistFromPlayer (res);
					if (dist >= amt) {
						fanSpawns.Add (res);
					}
				}
			}
		}

		for (int i = 0; i < count; i++) {
			// Safety
			if (fanSpawns.Count == 0) {
				break;
			}

			// Make a fan at a random position
			int id = GameState.rng.Next(fanSpawns.Count);
			IntPoint pt = fanSpawns [id];
			fanSpawns.RemoveAt (id);

			CreateFan (pt.x, pt.y);
		}

	}

	public GameObject CreateLead(int tileX, int tileY) {
		int nxt = GameState.rng.Next (6);
		GameObject res = null;
		if (nxt == 0) {
			res = Instantiate(LeadPrefab0M, new Vector3(0, 2, 0), Quaternion.identity);
		} else if (nxt == 1) {
			res = Instantiate(LeadPrefab0W, new Vector3(0, 2, 0), Quaternion.identity);
		} else if (nxt == 2) {
			res = Instantiate(LeadPrefab1M, new Vector3(0, 2, 0), Quaternion.identity);
		} else if (nxt == 3) {
			res = Instantiate(LeadPrefab1W, new Vector3(0, 2, 0), Quaternion.identity);
		} else if (nxt == 4) {
			res = Instantiate(LeadPrefab2M, new Vector3(0, 2, 0), Quaternion.identity);
		} else {
			res = Instantiate(LeadPrefab2W, new Vector3(0, 2, 0), Quaternion.identity);
		}
		res.GetComponent<TokenHandler>().MapHandler = this.gameObject;
		res.GetComponent<TokenHandler> ().self = res;
		res.GetComponent<TokenHandler> ().tweetHandler = tweetHandler;

		res.GetComponent<TokenHandler>().MoveToTile (tileX, tileY);
		res.GetComponent<TokenHandler> ().LeadObj = new Lead ();
		return res;
	}

	public GameObject CreateFan(int tileX, int tileY) {
		GameObject res = Instantiate(FanPrefab, new Vector3(0, 2, 0), Quaternion.identity);
		res.GetComponent<TokenHandler>().MapHandler = this.gameObject;
		res.GetComponent<TokenHandler> ().self = res;
		res.GetComponent<TokenHandler> ().tweetHandler = tweetHandler;

		res.GetComponent<TokenHandler>().MoveToTile (tileX, tileY);
		res.GetComponent<TokenHandler> ().FanObj = new Fan ();
		fans.Add (res);
		return res;
	}

	public GameObject CreateHeart(int tileX, int tileY) {
		GameObject res = Instantiate(heartPrefab, new Vector3(0, 5, 0), Quaternion.identity);
		res.GetComponent<TokenHandler>().MapHandler = this.gameObject;
		res.GetComponent<TokenHandler> ().self = res;
		res.GetComponent<TokenHandler> ().tweetHandler = tweetHandler;

		res.GetComponent<TokenHandler>().MoveToTile (tileX, tileY);
		return res;
	}

	/*public void GetClickTile () {
	}*/

	// Find the next NPC that hasn't moved and move it randomly.
	// Return true if an NPC moved
	// TEMP function
	public bool MoveNextNPC() {
		// Find the NEAREST fan that hasn't moved
		GameObject nextFan = null;

		foreach (GameObject fanObj in fans) {
			TokenHandler fan = fanObj.GetComponent<TokenHandler> ();
			if (fan.MovedThisTurn) {
				continue;
			}

			// Set/compare it
			if (nextFan == null) {
				nextFan = fanObj;
			} else {
				TokenHandler curr = nextFan.GetComponent<TokenHandler> ();
				int currDist = getMinDistFromPlayer (new IntPoint (curr.TileX, curr.TileY));
				int nxtDist = getMinDistFromPlayer (new IntPoint (fan.TileX, fan.TileY));
				if (nxtDist < currDist) {
					nextFan = fanObj;
				}
			}
		}

		// Anything?
		if (nextFan != null) {
			TokenHandler fan = nextFan.GetComponent<TokenHandler> ();
			fan.MovedThisTurn = true;

			// Are we moving or attacking?
			foreach (char s in "NSEW") {
				IntPoint next = IntPoint.FromCardinal (fan.TileX, fan.TileY, s);
				if (SingleCollide (leadPlayer, next) || SingleCollide (leadDate, next)) {
					leadPlayer.GetComponent<TokenHandler> ().LeadObj.SelfEsteem -= 1;

					// TODO: Show animation

					// TODO: Play damage sound (placeholder)
					sfxSource.clip = movementSFX[ Random.Range(0, movementSFX.Length) ];
					sfxSource.Play();

					// Damage player
					LeadPlayerScript.SelfEsteem -= 1;

					// James: death animation
					Debug.Log("James: a death animation is needed here.");

					// Destroy this fan
					DestroyFan(nextFan);

					return true;
				}
			}

			// Ok, we're walking
			bool didAWalk = fan.FanWalkTowards(LeadDate, LeadPlayer);

			// Play movement SFX
			if (didAWalk) {
				sfxSource.clip = movementSFX [Random.Range (0, movementSFX.Length)];
				sfxSource.Play ();

				return true;
			} else {
				return false;
			}
		}
		return false;
	}

	public void DestroyFan(GameObject fan) {
		fans.Remove (fan);
		//fan.GetComponent<TokenHandler> ().FlipToken ();
		//Destroy (fan);
	}

	private bool SingleCollide(GameObject other, IntPoint dest) {
		return dest.x == other.GetComponent<TokenHandler> ().TileX && dest.y == other.GetComponent<TokenHandler> ().TileY;
	}

	// Is this tile free?
	public bool CanMove(IntPoint dest) 
	{
		// Bounds collisions
		if (dest.x < 0 || dest.y < 0) {
			return false;
		}
		if (dest.x >= mapTileWidth || dest.y >= mapTileHeight) {
			return false;
		}

		// Tile Collisions
		if (GetTileValue (dest.x, dest.y) != 0) {
			return false;
		}

		// Player/Fan collisions
		foreach (GameObject fan in fans) {
			if (SingleCollide(fan, dest)) {
				return false;
			}
		}
		if (SingleCollide(leadPlayer, dest)) {
			return false;
		}
		if (SingleCollide(leadDate, dest)) {
			return false;
		} 


		return true;
	}

	public void ResetNPCMoves() {
		foreach (GameObject fan in fans) {
			fan.GetComponent<TokenHandler>().MovedThisTurn = false;
		}
	}
}
