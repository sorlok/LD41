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
	public List<int> mapTileValues = new List<int>();

	// 
	public List<int> mapTileHeights = new List<int>();

	public int mapTileWidth = 10;
	public int mapTileHeight = 10;

	// Used to create the lead/fan
	public GameObject LeadPrefab;
	public GameObject FanPrefab;

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
	void Start () {}
	
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
			2,2,2,2,2,2,2,2,2,2,  //0
			2,2,2,2,2,2,2,5,2,2,  //1
			2,2,2,2,2,5,5,2,2,2,  //2
			5,5,5,2,2,2,2,2,2,2,  //3
			2,2,2,5,2,5,2,2,5,2,  //4
			2,2,2,5,2,5,5,2,5,2,  //5
			2,2,2,5,2,5,5,2,5,2,  //6
			2,2,2,2,2,2,2,2,2,2,  //7
			2,2,2,2,5,5,5,2,5,2,  //8
			2,2,2,2,2,2,2,2,2,2,  //9
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

		for (int i = 2; i < 10; i++) {
			CreateFan (i, i);
		}

	}

	public GameObject CreateLead(int tileX, int tileY) {
		GameObject res = Instantiate(LeadPrefab, new Vector3(0, 2, 0), Quaternion.identity);
		res.GetComponent<TokenHandler>().MapHandler = this.gameObject;
		res.GetComponent<TokenHandler>().MoveToTile (tileX, tileY);
		res.GetComponent<TokenHandler> ().LeadObj = new Lead ();
		return res;
	}

	public GameObject CreateFan(int tileX, int tileY) {
		GameObject res = Instantiate(FanPrefab, new Vector3(0, 2, 0), Quaternion.identity);
		res.GetComponent<TokenHandler>().MapHandler = this.gameObject;
		res.GetComponent<TokenHandler>().MoveToTile (tileX, tileY);
		res.GetComponent<TokenHandler> ().FanObj = new Fan ();
		fans.Add (res);
		return res;
	}

	public GameObject CreateHeart(int tileX, int tileY) {
		GameObject res = Instantiate(heartPrefab, new Vector3(0, 5, 0), Quaternion.identity);
		res.GetComponent<TokenHandler>().MapHandler = this.gameObject;
		res.GetComponent<TokenHandler>().MoveToTile (tileX, tileY);
		return res;
	}

	/*public void GetClickTile () {
	}*/

	// Find the next NPC that hasn't moved and move it randomly.
	// Return true if an NPC moved
	// TEMP function
	public bool MoveNextNPC() {
		foreach (GameObject fanObj in fans) {
			TokenHandler fan = fanObj.GetComponent<TokenHandler> ();
			if (fan.MovedThisTurn) {
				continue;
			}

			fan.MovedThisTurn = true;

			// TODO: Check if it knows about the players or not
			fan.FanRandomWalk();

			// Play movement SFX
			sfxSource.clip = movementSFX[ Random.Range(0, movementSFX.Length) ];
			sfxSource.Play();

			// Do damage to player
			foreach (char s in "NSEW") {
				IntPoint next = IntPoint.FromCardinal (fan.TileX, fan.TileY, s);
				if (SingleCollide (leadPlayer, next)) {
					leadPlayer.GetComponent<TokenHandler> ().LeadObj.SelfEsteem -= 1;

					// Destroy this fan
					fans.Remove (fanObj);
					Destroy (fanObj);
				}
			}

			return true;
		}
		return false;
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
