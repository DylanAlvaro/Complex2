using System;
using System.Collections.Generic;
using System.Linq;
using Math;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Dungeon_Generation
{
    public class ItemAreaSpawner : MonoBehaviour
    {
        private DungeonData _data;

        private List<Props> _propsList;
        private float cornerPropPlacementChance = .7f;

        private GameObject propPrefab;

        private void Awake()
        {
            _data = FindObjectOfType<DungeonData>();
        }

        public void ProcessRooms()
        {
            if (_data == null)
                return;

            foreach (Room room in _data.rooms)
            {
                List<Props> cornerProps = _propsList.Where(x => x.CornerOfRoom).ToList();
                
            }
        }
    }
}