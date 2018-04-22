using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class BFSTree {
	public BFSTreeNode root;

	public BFSTree(Vector2Int start, Vector2Int dest, MapHandler mapHnd) {
		GenerateTree (start, dest, mapHnd);
	}

	int CalculateDistances (Vector2Int dest, BFSTreeNode node) {
		if (node.isDest) {
			return 1;
		}

		if (node.children.Count == 0) {
			return -1;
		}
			
		// assign distance values for all children
		int minDistance = 1000000000;
		foreach (BFSTreeNode child in node.children) {
			int distance = CalculateDistances (dest, child);
			if (distance == -1) {
				node.distances.Add (-1);
			} else {
				node.distances.Add (distance);
				if (distance < minDistance) {
					minDistance = distance;
				}
			}
		}

		// return minimum, non-zero distance value
		return 1 + minDistance;
	}

	void GenerateTree (Vector2Int start, Vector2Int dest, MapHandler mapHnd) {
		root = new BFSTreeNode (start);

		// Track all cells we have or might visit
		Dictionary<Vector2Int, bool> blacklisted = new Dictionary<Vector2Int, bool> ();

		// structure to keep track of frontier of breadth-first search
		Queue<BFSTreeNode> frontier = new Queue<BFSTreeNode> ();
		frontier.Enqueue (root);
	
		// construct breadth-first tree
		blacklisted.Add (start, true);
		while (frontier.Count > 0) {
			BFSTreeNode currentNode = frontier.Dequeue ();

			foreach (Vector2Int nextPos in GetNeighbors(currentNode.id, mapHnd)) {
				BFSTreeNode nextNode = new BFSTreeNode ( nextPos );
				currentNode.AddChild (nextNode );

				// Success is when we are +1 in either direction from the destination
				int xDiff = Math.Abs(nextPos.x - dest.x);
				int yDiff = Math.Abs(nextPos.y - dest.y);
				if (xDiff == 1 && yDiff == 0) {
					nextNode.isDest = true;
					break;
				}
				if (yDiff == 1 && xDiff == 0) {
					nextNode.isDest = true;
					break;
				}

				// expand search frontier
				if (!blacklisted.ContainsKey(nextPos)) {
					blacklisted.Add (nextPos, true);
					frontier.Enqueue (nextNode);
				}
			}
		}

		// calculate distances to dest
		CalculateDistances(dest, root);
	}

	public static List<Vector2Int> GetNeighbors(Vector2Int pos, MapHandler mapHnd) {
		// check that pos is a valid coordinate
		if (pos.x < 0 || pos.x >= mapHnd.mapTileWidth || pos.y < 0 || pos.y >= mapHnd.mapTileHeight) {
			return null;
		}

		List<Vector2Int> neighbors = new List<Vector2Int> ();

		foreach (char s in "NSEW") {
			IntPoint newPos = IntPoint.FromCardinal (pos.x, pos.y, s);
			if (mapHnd.CanMove (newPos)) {
				neighbors.Add (new Vector2Int(newPos.x, newPos.y));
			}
		}

		return neighbors;
	}
}
