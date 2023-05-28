using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Dungeon_Generation
{
    public class ItemAreaSpawner : MonoBehaviour
    {
        public GameObject itemToSpread;
        public int numItemsToSpanw = 10;
        public float itemXSpread = 10;
        public float itemYSpread = 0;
        public float itemZSpread = 10;

        private void Start()
        {
            for (int i = 0; i < numItemsToSpanw; i++)
            {
                SpreadItems();
            }
        }

        void SpreadItems()
        {
            Vector3 randPos = new Vector3(Random.Range(-itemXSpread, itemXSpread),
                Random.Range(-itemYSpread, itemYSpread), Random.Range(-itemZSpread, itemZSpread)) + transform.position;

            GameObject clone = Instantiate(itemToSpread, randPos, Quaternion.identity);
        }
    }
}