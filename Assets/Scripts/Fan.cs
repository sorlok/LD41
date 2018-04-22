using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fan : MonoBehaviour {
	public GameObject fan;

	/* basic resources */
	private bool aware = false;
	private bool enraged = false;
	private bool influenced = false;
	private bool isTraveling = false;

	/* movement */
	private uint speed = 2;

	private Vector2Int currentLoc;
	private Vector2Int dateLoc;
	private Vector2Int promotionLoc;

	private int[,] map;
	private BFSTreeNode travelPlan;

	/* combat */
	public Lead lead;

	private uint damage = 1;


	/* --- Methods --- */
	void DoDamage () {
		if (IsPCAdjacent()) {
			lead.SelfEsteem = lead.SelfEsteem - (damage * (enraged ? 2u : 1u)); 
		}
	}

	bool IsPCAdjacent () {
		Vector2Int result = currentLoc - lead.currentLoc;
		return (result.magnitude == 1);
	}

	void MakeTravelPlan() {
		BFSTree bfsTravelPlan = new BFSTree (currentLoc, dateLoc, map);
		travelPlan = bfsTravelPlan.root;
		isTraveling = true;
	}

	void MoveToLocation () {
		// get minimum distance to travel destination from current node
		int minDistance = 1000000000;
		int[] distances = travelPlan.distances.ToArray ();
		foreach (int distance in distances) {
			if (distance < minDistance) {
				minDistance = distance;
			}
		}

		// get list of relevant tree nodes
		List<BFSTreeNode> possibleChoices = new List<BFSTreeNode>();
		BFSTreeNode[] children = travelPlan.children.ToArray ();
		for (int i = 0; i < children.Length; i++) {
			if (distances[i] == minDistance) {
				possibleChoices.Add (children[i]);
			}
		}

		// make choice
		BFSTreeNode[] choices = possibleChoices.ToArray();
		travelPlan = choices[Random.Range (0, choices.Length)];

		// move to chosen location
		currentLoc = travelPlan.id;		// TODO: do actual movement later
		if (IsPCAdjacent ()) {
			isTraveling = false;
		}
	}

	void TakeTurn() {
		/* Priority 1: If this fan has been affected by a promoted tweet, they perform the following behavior. */
		if (influenced) {
			MoveToLocation ();

		/* Priority 2: If this fan is aware of the Leads, they perform the following behavior. */
		} else if (aware) {
			if (IsPCAdjacent()) {
				DoDamage ();
			
			/* Make travel plans or execute on the travel plans. */
			} else {
				if (isTraveling) {
					MoveToLocation ();
				} else {
					MakeTravelPlan ();
				}
			}

		/* Priority 3: Do a random walk. */
		} else {
			RandomWalk ();
		}
	}

	void RandomWalk () {
		// get adjacent tiles
		List<Vector2Int> neighborList = BFSTree.GetNeighbors (currentLoc, map);
		Vector2Int[] neighbors = neighborList.ToArray();

		// choose adjacent tile
		Vector2Int choiceLoc = neighbors[ Random.Range(0, neighbors.Length) ];

		currentLoc = choiceLoc; // TODO: do actual movement later
	}

	public void SetAwareness (bool awareness, Vector2Int dateLocation) {
		aware = awareness;
		dateLoc = dateLocation;
	}


	/* --- Unity Life Cycle callbacks --- */
	// Use this for initialization
	void Start () {}

	// Update is called once per frame
	void Update () {
		if (Input.GetKey (KeyCode.F)) {
			// test things here
		}
	}
}
