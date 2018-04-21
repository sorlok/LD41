using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class BFSTreeNode {
	public Vector2Int id;

	public List<int> distances;
	public List<BFSTreeNode> children;

	public BFSTreeNode(Vector2Int _id) {
		id = _id;

		distances = new List<int> ();
		children = new List<BFSTreeNode> ();
	}

	public void AddChild (BFSTreeNode _node) {
		children.Add (_node);
	}

	public void AddChild (Vector2Int _id) {
		children.Add (new BFSTreeNode(_id));
	}
}
