using System.Collections.Generic;

using UnityEngine;

using Vector2 = System.Numerics.Vector2;

namespace Math
{
	public class DungeonData : MonoBehaviour
	{
		public List<Room> rooms { get; set; } = new List<Room>();
		public HashSet<Vector2Int> path { get; set; } = new HashSet<Vector2Int>();
		public GameObject player { get; set; }
		
		
		public void Reset()
		{
			foreach (Room room1 in rooms)
			{
				foreach (var item in room1.PropObjectReferences)
				{
					Destroy(item);
				}
				foreach (var item in room1.EnemiesInTheRoom)
				{
					Destroy(item);
				}
			}
			rooms = new();
			path = new();
			Destroy(player);
		}

	}

	public class Room
	{
		public Vector2 roomCenterPos { get; set; }
		public HashSet<Vector2Int> PropPositions { get; set; } = new HashSet<Vector2Int>();
		public List<GameObject> PropObjectReferences { get; set; } = new List<GameObject>();
		
		public List<GameObject> EnemiesInTheRoom { get; set; } = new List<GameObject>();

		public Room(Vector2 roomCenterPos, HashSet<Vector2Int> floorTiles)
		{
			this.roomCenterPos = roomCenterPos;
		}
	}
}