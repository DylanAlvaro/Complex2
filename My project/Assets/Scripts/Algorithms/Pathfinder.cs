using System;
using System.Collections.Generic;

using UnityEditor;

using UnityEngine;

namespace Scripts.Dungeon.Pathfinder
{
	public class Pathfinder : MonoBehaviour
	{
		
		
		
		public Grid grid;

		private void Awake()
		{
			grid = GetComponent<Grid>();
		}

		public void FindPath(Vector3 startPos, Vector3 targetPos)
		{
			Node startNode = grid.NodeFromWorldPoint(startPos);
			Node targetNode = grid.NodeFromWorldPoint(targetPos);

			List<Node> openList = new List<Node>();
			HashSet<Node> closedList = new HashSet<Node>();
			
			openList.Add(startNode);

			while(openList.Count > 0)
			{
				Node currentNode = openList[0];

				for(int i = 1; i < openList.Count; i++)
				{
					if(openList[i].fCost < currentNode.fCost || openList[i].fCost == currentNode.fCost && openList[i].hCost < currentNode.hCost) 
						currentNode = openList[i];
				} 
				openList.Remove(currentNode);
				closedList.Add(currentNode);

				if(currentNode == targetNode)
					RetracePath(startNode, targetNode);

				foreach(Node neighbours in grid.GetNeighbours(currentNode))
				{
					if(!neighbours.walkable || closedList.Contains(neighbours))
						continue;

					int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbours);
					if(newMovementCostToNeighbour < neighbours.gCost || !openList.Contains(neighbours))
					{
						neighbours.gCost = newMovementCostToNeighbour;
						neighbours.hCost = GetDistance(neighbours, targetNode);
						neighbours.Parent = currentNode;
						
						if(!openList.Contains(neighbours))
							openList.Add(neighbours);
					}
				}
			}
		}

		void RetracePath(Node startNode, Node endNode)
		{
			List<Node> path = new List<Node>();
			Node currentNode = endNode;

			while(currentNode != startNode)
			{
				path.Add(currentNode);
				currentNode = currentNode.Parent;
			}
			path.Reverse();
		}

		int GetDistance(Node nodeA, Node nodeB)
		{
			int distanceX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
			int distanceY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

			if(distanceX > distanceY) 
				return 14 * distanceY + 10 * (distanceX - distanceY);
			else 
				return 14 * distanceX + 10 * (distanceY - distanceX);
		}
		
	#region Editor
	#if UNITY_EDITOR
		[CustomEditor(typeof(Pathfinder))]	
		public class DungeonEditor : Editor
		{
			public override void OnInspectorGUI()
			{
				base.OnInspectorGUI();

				Pathfinder pathfinder = (Pathfinder) target;
			
				EditorGUILayout.Space();
				EditorGUILayout.BeginHorizontal();
			
				EditorGUILayout.LabelField("Generate Delaunay Dungeon");
				EditorGUILayout.LabelField("Generate BSP Dungeon");

				EditorGUILayout.EndHorizontal();
			}
		}
	#endif
	#endregion
	}
}