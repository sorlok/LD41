using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum StatToTrack {
	PlayerSelfEsteem,
	PlayerFans,
	PlayerAtmosphere,
	PlayerConnection,

	// Could also track date stats, but I guess we're not doing that?
}

public class StatsUISimple : MonoBehaviour {
	// Where to show the updated text
	public Text TextTrack;

	// What do show for the String part of the stat
	private string StatName;

	// Which stat to get, and from whom.
	public StatToTrack StatTrack;

	// The associated MapHandler
	public GameObject MapHandler;

	public void CallbackUpdate(uint val) {
		TextTrack.text = StatName + ": " + val;

		Debug.Log ("Callback: " + val);
	}

	private bool turn1 = true;


	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		if (turn1) {
			turn1 = false;

			Lead lead = MapHandler.GetComponent<MapHandler> ().LeadPlayerScript;
			if (StatTrack == StatToTrack.PlayerSelfEsteem) {
				StatName = "Self Esteem";
				lead.SelfEsteemTracker += CallbackUpdate;
				CallbackUpdate (lead.SelfEsteem);
			} else if (StatTrack == StatToTrack.PlayerFans) {
				StatName = "Fan Count";
				lead.FanCountTracker += CallbackUpdate;
				CallbackUpdate (lead.FanCount);
			} else if (StatTrack == StatToTrack.PlayerAtmosphere) {
				// TODO
				//StatName = "Atmosphere";
				//lead.AtmosphereTracker += CallbackUpdate;
				//CallbackUpdate (lead.Atmosphere);
			} else if (StatTrack == StatToTrack.PlayerConnection) {
				StatName = "Connection";
				lead.ConnectionTracker += CallbackUpdate;
				CallbackUpdate (lead.Connection);
			}
		}
		
	}
}
