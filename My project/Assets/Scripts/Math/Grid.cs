using System;
using System.Collections.Generic;

using UnityEngine;

namespace Scripts.Dungeon.Pathfinder
{
	public class Grid : MonoBehaviour
	{
		public LayerMask unwalkableMask;
		public Vector2 gridWorldSize;
		public float nodeRadius;
		private Node[,] grid;

		private float nodeDiamter;
		private int gridSizeX, gridSizeY;
		
		private void Start()
		{
			nodeDiamter = nodeRadius * 2;
			gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiamter);
			gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiamter);
			CreateGrid();
		}

		private void CreateGrid()
		{
			grid = new Node[gridSizeX, gridSizeY];
			Vector3 worldBotLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.forward * gridWorldSize.y / 2;

			for(int i = 0; i < gridSizeX; i++)
			{
				for(int j = 0; j < gridSizeY; j++)
				{
					Vector3 worldPoint = worldBotLeft + Vector3.right * (i * nodeDiamter + nodeRadius) 
					                                  + Vector3.forward * (j * nodeDiamter + nodeRadius);
					bool walkable = !(Physics.CheckSphere(worldPoint, nodeRadius, unwalkableMask));

					grid[i, j] = new Node(walkable, worldPoint, i, j);
				}
			}
		}

		public List<Node> GetNeighbours(Node node)
		{
			List<Node> neighbours = new List<Node>();
			for(int i = -1; i <= 1; i++)
			{
				for(int j = -1; j <= 1; j++)
				{
					if(i == 0 && j == 0)
						continue;

					int checkX = node.gridX + i;
					int checkY = node.gridY + j;

					if(checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
					{
						neighbours.Add(grid[checkX, checkY]);
					}
				}
			}

			return neighbours;
		}

		public Node NodeFromWorldPoint(Vector3 worldPos)
		{
			float percentX = (worldPos.x + gridWorldSize.x / 2) / gridWorldSize.x;
			float percentY = (worldPos.y + gridWorldSize.y / 2) / gridWorldSize.y;
			
			percentX = Mathf.Clamp01(percentX);
			percentY = Mathf.Clamp01(percentY);

			int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
			int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);

			return grid[x, y];
		}

		private void OnDrawGizmos()
		{
			Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, 1, gridWorldSize.y));

			if(grid != null)
			{
				foreach(Node n in grid)
				{
					Gizmos.color = (n.walkable)? Color.white : Color.red;
					Gizmos.DrawCube(n.worldPos, Vector3.one * (nodeDiamter - .1f));
				}
			}
		}
	}
}