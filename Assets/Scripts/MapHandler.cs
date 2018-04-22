using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapHandler : MonoBehaviour {
	public List<int> mapTileValues = new List<int>();

	// 
	public List<int> mapTileHeights = new List<int>();

	public int mapTileWidth = 10;
	public int mapTileHeight = 10;

	public GameObject LeadPrefab;
	public GameObject FanPrefab;

	public int GetTileValue(int tileX, int tileY) {
		int id = GetTileIndex (tileX, tileY);
		return GetTileValue (mapTileValues, id);
	}

	public int GetTileHeight(int tileX, int tileY) {
		int id = GetTileIndex (tileX, tileY);
		return GetTileValue (mapTileHeights, id);
	}

	private int GetTileIndex(int tileX, int tileY) {
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
		
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public GameObject CreateLead(int tileX, int tileY) {
		// 2 for normal, 5 for hill height; 8 for house-on-hill height
		GameObject res = Instantiate(LeadPrefab, new Vector3(0, 2, 0), Quaternion.identity);
		res.GetComponent<TokenHandler>().MoveToTile (tileX, tileY);
		return res;
	}

	public GameObject CreateFan(int tileX, int tileY) {
		// 2 for normal, 5 for hill height; 8 for house-on-hill height
		GameObject res = Instantiate(FanPrefab, new Vector3(0, 2, 0), Quaternion.identity);
		res.GetComponent<TokenHandler>().MoveToTile (tileX, tileY);
		return res;
	}

	public void GetClickTile () {

	}
}
