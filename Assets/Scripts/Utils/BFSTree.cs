using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class BFSTree {
	public BFSTreeNode root;

	public BFSTree(Vector2Int start, Vector2Int dest, int[,] matrix) {
		GenerateTree (start, dest, matrix);
	}

	int CalculateDistances (Vector2Int dest, BFSTreeNode node) {
		if (node.children.Count == 0) {
			return -1;
		}

		if (node.id == dest) {
			return 1;
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

	void GenerateTree (Vector2Int start, Vector2Int dest, int[,] matrix) {
		root = new BFSTreeNode (start);
		BFSTreeNode destNode = new BFSTreeNode (dest);

		// structure to keep track of visited matrix cells
		Dictionary<Vector2Int, bool> visited = new Dictionary<Vector2Int, bool> ();

		// structure to keep track of frontier of breadth-first search
		Queue<Vector2Int> frontier = new Queue<Vector2Int> ();
		frontier.Enqueue (start);

		// construct breadth-first tree
		Vector2Int currentPos;
		BFSTreeNode currentNode = root;
		while (frontier.Count > 0) {
			currentPos = frontier.Dequeue ();
			if (currentPos != dest) {
				visited.Add (currentPos, true);
			}

			foreach (Vector2Int nextPos in GetNeighbors(currentPos, matrix)) {
				// leaf node
				if (nextPos == dest) {
					currentNode.AddChild (destNode);
				}

				// expand search frontier
				if (!visited.ContainsKey(nextPos)) {
					currentNode.AddChild (nextPos);
					frontier.Enqueue (nextPos);
				}
			}
		}

		// calculate distances to dest
		CalculateDistances(dest, root);
	}

	List<Vector2Int> GetNeighbors(Vector2Int pos, int[,] matrix) {
		int maxRow = matrix.GetLength (0);
		int maxCol = matrix.GetLength (1);

		// check that pos is a valid coordinate
		if (pos.x < 0 || pos.x > maxRow || pos.y < 0 || pos.y > maxCol) {
			return null;
		}

		List<Vector2Int> neighbors = new List<Vector2Int> ();

		if (pos.y + 1 > maxCol) {		// north
			neighbors.Add (new Vector2Int (pos.x, pos.y + 1));
		}

		if (pos.x + 1 > maxRow) {		// east
			neighbors.Add (new Vector2Int (pos.x + 1, pos.y));
		}
			
		if (pos.y - 1 < 0) {			// south
			neighbors.Add (new Vector2Int (pos.x, pos.y + 1));
		}

		if (pos.x - 1 < 0) {			// west
			neighbors.Add (new Vector2Int (pos.x - 1, pos.y));
		}

		return neighbors;
	}
}
