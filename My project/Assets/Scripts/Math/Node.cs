using System.Collections.Generic;

using UnityEngine;

namespace Scripts.Dungeon.Pathfinder
{
	public class Node
	{
		public bool walkable;
		public Vector3 worldPos;
		public int gridX;
		public int gridY;

		public int gCost;
		public int hCost;
		public Node Parent { get; set; }

		public Node(bool _walkable, Vector3 _worldPos, int _gridX, int _gridY)
		{
			walkable = _walkable;
			worldPos = _worldPos;
			gridX = _gridX;
			gridY = _gridY;
		}

		public int fCost
		{
			get
			{
				return gCost + hCost;
			}
		}
	}
	
}