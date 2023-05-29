using UnityEngine;

namespace Dungeon_Generation
{
	public class Props : MonoBehaviour
	{
		public GameObject propPrefab;

		[Header("placement type")] 
		public bool CornerOfRoom = true;

		public bool MiddleOfRoom = true;

		public int PropQuantityMin = 1;
		public int PropQuantityMax = 1;
		
	}
}