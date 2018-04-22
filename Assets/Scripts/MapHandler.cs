using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapHandler : MonoBehaviour {
	public List<int> mapTileValues = new List<int>();
	public List<int> mapTileHeights = new List<int>();

	public int mapTileWidth = 10;
	public int mapTileHeight = 10;

	public Transform LeadPrefab;
	public Transform FanPrefab;


	// Use this for initialization
	void Start () {
		
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void CreateLead(int tileX, int tileY) {
		// 2 for normal, 5 for hill height; 8 for house-on-hill height
		Instantiate(LeadPrefab, new Vector3(0, 2, 0), Quaternion.identity);

	}

	public void CreateFan(int tileX, int tileY) {

	}

	public void GetClickTile () {

	}
}
