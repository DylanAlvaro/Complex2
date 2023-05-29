using UnityEngine;

namespace Math
{
    public class Rooms
    {
        public bool isMainRoom { get; private set; }
        public bool isStartingRoom { get; private set; }
        public bool isEndRoom { get; private set; }


        public void SetMain()
        {
            isMainRoom = true;
        }

        public void SetStartRoom()
        {
            isStartingRoom = true;
        }

        public void SetEndRoom()
        {
            isEndRoom = true;
        }
        
        public BoundsInt bounds;
        public Rooms(Vector3Int location, Vector3Int size) {
                bounds = new BoundsInt(location, size);
            }

        public static bool RoomsIntersecting(Rooms room1, Rooms room2)
        {
            return !(room1.bounds.position.x >= room2.bounds.position.x + room1.bounds.size.x || room1.bounds.x <= room2.bounds.x  - room2.bounds.size.x||
                     room1.bounds.y >= room2.bounds.y + room1.bounds.size.y|| room1.bounds.y <= room2.bounds.y - room2.bounds.size.y||
                     room1.bounds.z >= room2.bounds.z + room1.bounds.size.z|| room1.bounds.z <= room2.bounds.z - room2.bounds.size.z);
        }
    }
}