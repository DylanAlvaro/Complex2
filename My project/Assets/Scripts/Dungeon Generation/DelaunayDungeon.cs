using System.Collections.Generic;
using System.Drawing;
using Algorithms;
using Graphs;
using Math;
using UnityEngine;

namespace Dungeon_Generation
{
    public class DelaunayDungeon : MonoBehaviour
    {
        // Important variables for making sure the generation works
        [SerializeField] public Vector3Int size;
        [SerializeField] public Vector3Int maxRoomSize;
        [SerializeField] public int roomCount;
        [SerializeField] public GameObject floorPrefab;
        [SerializeField] public GameObject wallPrefab;
        [SerializeField] public GameObject hallwayPrefab;
        public int seed;

        // private variables
        private bool addDifferentRooms = true;
        private bool canAddWalls = true;
        private List<Rooms> _rooms;
        private int roomDistance = 10;
        private int _spacingBetween = 50;
        private bool canConnectHalls = true;

        HashSet<Edge> selectedEdges;


        enum CellType
        {
            None,
            Room,
            Hallway
        }

        private List<Point> _verts;

        private List<Edge> edges;

        //public variables
        public DelaunayTriagulation DelaunayTri;

        // Start is called before the first frame update
        void Start()
        {
            _rooms = new List<Rooms>();
            _verts = new List<Point>();
            seed = Random.Range(0, 99999);
            //calling functions to start
            CreateRooms();
            // Triangulation();
            //AstarCorridors();
        }

        /// <summary>
        /// Placing Room functionality goes here:
        /// The first step is to go through a list of set rooms put into the inspector and make where they
        /// are placed random (x, y, z) this is also done for the size of the rooms plus how high they can go up
        /// if the bool returns true it calls the Place Room function which takes in a location and size
        /// </summary>
        public void CreateRooms()
        {
            DestroyAllChildren();
            for (int i = 0; i < roomCount; i++)
            {
                Vector3Int position = new Vector3Int(Random.Range(0, size.x + _spacingBetween),
                    Random.Range(0, size.y),
                    Random.Range(0, size.z + _spacingBetween));

                //_verts.Add(new Point(position.x, position.y));
                // making random sized rooms based on a x, y and z coord
                Vector3Int sizeOfRooms = new Vector3Int(Random.Range(1, maxRoomSize.x + 1),
                    Random.Range(0, maxRoomSize.y + 1),
                    Random.Range(1, maxRoomSize.z + 1));

                Rooms newAddedRoom = new Rooms(position, sizeOfRooms);
                Rooms spacing = new Rooms(position + new Vector3Int(-1, 0, -1), sizeOfRooms + new Vector3Int(5, 0, 5));


                //This should hopefully make sure that the cubes (rooms) aren't overlapping

                foreach (Rooms room1 in _rooms)
                {
                    if (Rooms.Intersect(newAddedRoom, spacing))
                    {
                        addDifferentRooms = false;

                        break;
                    }

                    if (newAddedRoom.bounds.xMin < 0 || newAddedRoom.bounds.xMax >= size.x ||
                        newAddedRoom.bounds.yMin < 0 || newAddedRoom.bounds.yMax >= size.y ||
                        newAddedRoom.bounds.zMin < 0 || newAddedRoom.bounds.zMax >= size.z)
                    {
                        addDifferentRooms = false;
                    }
                }

                if (addDifferentRooms)
                {
                    PlaceRoom(newAddedRoom.bounds.position, newAddedRoom.bounds.size);
                    // ConnectRooms();
                }
            }
        }

        private void Triangulation()
        {
            //delaunay implementation
            List<Graphs.Vertex> vertices = new List<Graphs.Vertex>();

            //foreach(Rooms room in _rooms)
            //{
            //    vertices.Add(new Graphs.Vertex<Rooms>((Vector3) room.bounds.position + ((Vector3) room.bounds.size) / 2, room));
            //}
            DelaunayTri.Triangulate();
        }



        /// <summary>
        /// This function will pathfind through the rooms and create links between them
        /// </summary>

        private void AstarCorridors(Rooms r1, Rooms r2)
        {
            // Pathfinder pathfinder = new Pathfinder();
            //
            // Vector3 startRoom = r1.bounds.position;
            // Vector3 endRoom = r2.bounds.position;
            //
            // Vector3 endRoomSize = r2.bounds.size;
            // Vector3 startRoomSize = r2.bounds.size;
//
            // Vector3 startPosF = r1.bounds.center;
            // Vector3 endPosF = r2.bounds.center;
            //
            // var startPos = new Vector3Int((int)startPosF.x, (int)startPosF.y, (int)startPosF.z);
            // var endPos = new Vector3Int((int)endPosF.x, (int)endPosF.y, (int)endPosF.z);
            // Vector3 dir = (endRoom - startRoom).normalized;
//
            // //float dst = Vector3.Distance(startRoom, endRoom); 
            // pathfinder.FindPath(startPos, endPos, );
            // 
            // 
            // 
//
            //// Vector3 hallwayScale = new Vector3(dst, 1f, 2f);
            // // Instantiate the hallway prefab
            // GameObject hallwayObj = Instantiate(hallwayPrefab, startPos, Quaternion.identity);
            // hallwayObj.GetComponent<Transform>().localScale = size;
        }

        // private void ConnectRooms()
        // {
        //     List<Room> connectedRooms = new List<Room>();
        //     connectedRooms.Add(_rooms[roomCount + 5]);
        //
        //     while(connectedRooms.Count < roomCount)
        //     {
        //         Room newRooms = GetNearestRoom(connectedRooms);
        //         Room nearestRoom = null;
        //         float shortestDst = float.MaxValue;
        ///
        //         foreach(Room r in connectedRooms)
        //         {
        //             float dist = Vector3.Distance(r.transform.position, newRooms.transform.position);

        // }




//private void ConnectHalls(Room r1, Room r2)
//  {
//      Pathfinder pathfinder = new Pathfinder();
//      
//      Vector3 startRoom = r1.bounds.position;
//      Vector3 endRoom = r2.bounds.position;
//      
//      Vector3 endRoomSize = r2.bounds.size;
//      Vector3 startRoomSize = r2.bounds.size;
//
//      Vector3 startPosF = r1.bounds.center;
//      Vector3 endPosF = r2.bounds.center;
//      
//      var startPos = new Vector3Int((int)startPosF.x, (int)startPosF.y, (int)startPosF.z);
//      var endPos = new Vector3Int((int)endPosF.x, (int)endPosF.y, (int)endPosF.z);
//      Vector3 dir = (endRoom - startRoom).normalized;
//
//     
//      float dst = Vector3.Distance(startRoom, endRoom); 
//      //pathfinder.FindPath(startPos, endPos);
//      Vector3 hallwayScale = new Vector3(dst, 1f, 2f);
//      
//      // Instantiate the hallway prefab
//      GameObject hallwayObj = Instantiate(hallwayPrefab, startPos, Quaternion.identity);
//      hallwayObj.GetComponent<Transform>().localScale = hallwayScale;
//  }
//
//  private void GenerateHallways()
//  {
//      List<Graphs.Edge> edges = new List<Graphs.Edge>();
//
//      foreach (var edge in DelaunayTri)
//      {
//          edges.Add(new Graphs.Edge(edge.U, edge.V));
//      }
//
//      List<Graphs.Edge> MST = Algorithms.PrimsAlgorithm.MinimumSpanningTree(edges, edges[0].U);
//      
//      selectedEdges = new HashSet<PrimsAlgorithm.Edge>(MST);
//      var remainingEdges = new HashSet<PrimsAlgorithm.Edge>(edges);
//      remainingEdges.ExceptWith(selectedEdges);
//
//      foreach (var edge in remainingEdges) 
//      {
//          if (Random.Range(0, 1f) < 0.125) {
//              selectedEdges.Add(edge);
//          }
//      }
//  }

        //private float GetMinimumDistance(Rooms room, List<Rooms> connectedRooms)
        //  {
        //      float minDistance = float.MaxValue;

        //      foreach (Rooms connectedRoom in connectedRooms)
        //      {
        //          float distance = Vector3.Distance(room.transform.position, connectedRoom.transform.position);
        //          if (distance < minDistance)
        //          {
        //              minDistance = distance;
        //          }
        //      }

        //      return minDistance;
        //  }


        /// <summary>
        /// Function takes in a vector3 location and size
        /// Instantiates a prefab and sets its transform
        /// </summary>
        /// <param name="location"></param>
        /// <param name="size"></param>
        /// <param name="material"></param>
        private void PlaceRoomFloors(Vector3Int location, Vector3Int size)
        {
            GameObject gameObject = Instantiate(floorPrefab, location, Quaternion.identity);
            gameObject.GetComponent<Transform>().localScale = size;
        }

        /// <summary>
        /// calls the place cube function above
        /// </summary>
        /// <param name="location"></param>
        /// <param name="size"></param>
        private void PlaceRoom(Vector3Int place, Vector3Int size)
        {
            PlaceRoomFloors(place, size);
        }

        private void PlaceCorridors(Vector3Int position)
        {
            PlaceRoomFloors(position, new Vector3Int(1, 1, 1));
        }
        
        private void DestroyAllChildren()
        {
            while (transform.childCount != 0)
            {
                foreach (Transform item in transform)
                {
                    DestroyImmediate(item.gameObject);
                }
            }
        }
    }
}