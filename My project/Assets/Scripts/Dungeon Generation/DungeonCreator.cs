using System;
using System.Collections;
using System.Collections.Generic;
using Dungeon_Generation;
using Math;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class DungeonCreator : MonoBehaviour
{
    [Header("Dungeon Values")]
    public int dungeonWidth; 
    public int dungeonLength;
    public int roomWidthMin, roomLengthMin; 
    public int maxIterations; 
    public int corridorWidth;

    [Header("Dungeon Room Modifiers")]
    public Material floorMaterial;
    public Material roofMaterial;
    [Range(0.0f, 0.3f)]
    public float roomBottomCornerModifier;
    [Range(0.7f, 1.0f)]
    public float roomTopCornerMidifier;
    [Range(0, 2)]
    public int roomOffset;

    public bool startingRoom = true;
    public bool lootRoom = true;

    [Header("Change GameObjects")]
    public GameObject wallHorizontal;
    public GameObject wallVertical;
    public GameObject floorPrefab;
    
    [Header("Player Prefab")] 
    public GameObject player;

    [Header("Spawn Items")] 
    public GameObject itemSpawn;
    public int ItemCount;

    [Header("Set Dressing")] 
    [Header("Obstacles")]
    public GameObject obstacleSpawns;
    public int obstacleCount;
    public PlacesToSpawn obstacleSpawnLocations = new PlacesToSpawn();
    
    [Header("Collectables")]
    public GameObject[] collectableSpawns;
    public int collectableCount;
    public PlacesToSpawn collectableSpawnLocations = new PlacesToSpawn();
    
    [Header("Hazards")]
    public GameObject[] hazardSpawns;
    public int hazardCount;
    public PlacesToSpawn hazardSpawnLocations = new PlacesToSpawn();

    [Header("Minimap")] 
    public Texture2D exploredAreas;
    public RawImage minimapImage;
    public Texture2D minimapTexture;
    private Color[] _exploredAreaColors;
    private bool[,] _exploredAreaMap;
   
    private Color[] _unexploredAreaColors;
    private bool[,] _unexploredAreaMap;
    private Color _playerColor = Color.blue;
    private Color _unplayerColor = Color.magenta;
    private Color _exploredItemColor = Color.red;
    private Color _exploredCollectableColor = Color.yellow;
    private Color _exploredRoomColor = Color.green;
    private Color _startingRoomColor = Color.cyan;
    private Transform _minimap;
    private bool _exploredRoom;
    private bool _isInRoom = false;
    

    private List<Vector3Int> _possibleWallHorizPos;
    private List<Vector3Int> _possibleWallVertPos;
    private List<Vector3Int> _possibleFloorHorizPos;
    private List<Vector3Int> _possibleFloorVertPos;

    private Transform _playerSpawned;
    private Transform _itemSpawned;
    private Transform _obstacleSpawned;
    private Transform _collectablesSpawned;
    private Transform _hazardsSpawned;
    private Transform _roomSpawned;
    
    private Vector3 _exploredHallway;
    private GameObject _dungeonFloor;
    private bool playerSpawned = false;
    private bool itemSpawned = false;
    private bool obstacleSpawned = false;
    private bool collectableSpawned = false;
    private bool hazardSpawned = false;
    private bool minimapSpawned = false;

    private RoomGenerator roomWidth;
    private RoomGenerator roomLength;

    public List<RoomGenerator> randomGenerators;
    public enum PlacesToSpawn
    {
        Corner,
        Middle,
        Hallway
    }
   public void Start()
    {
        CreateDungeon();
        InitializeMinimap();
    }

   private void Update()
   {
      UpdateMinimap();
   }

   /// <summary>
   /// Create Dungeon, this is the bulk of the code and creates the rooms based on semi-determistic features that the user selects
   /// it also creates walls and floors as well as spawns in objects such as the player, items such as coins, obstacles and hazards
   /// which are in different rooms. 
   /// </summary>
   public void CreateDungeon()
   {
       DestroyAllChildren();
       DungeonGenerator generator = new DungeonGenerator(dungeonWidth, dungeonLength);
       var listOfRooms = generator.CalculateRooms(maxIterations,
           roomWidthMin,
           roomLengthMin,
           roomBottomCornerModifier,
           roomTopCornerMidifier,
           roomOffset,
           corridorWidth);

       GameObject wallParent = new GameObject("WallParent");
       wallParent.transform.parent = transform;
       _possibleWallHorizPos = new List<Vector3Int>();
       _possibleWallVertPos = new List<Vector3Int>();

       GameObject floorParent = new GameObject("FloorParent");
       floorParent.transform.parent = transform;
       _possibleFloorHorizPos = new List<Vector3Int>();
       _possibleFloorVertPos = new List<Vector3Int>();


       int roomIndex = Random.Range(maxIterations / 2, listOfRooms.Count / 2);
       
       playerSpawned = false;
       itemSpawned = false;
       obstacleSpawned = false;
       collectableSpawned = false;
       minimapSpawned = false;
       
       for (int i = 0; i < listOfRooms.Count; i++)
       {
           CreateFloorAndRoof(listOfRooms[i].BottomLeftAreaCorner, listOfRooms[i].TopRightAreaCorner);

           if (i == 0)
           {
               //if (!playerSpawned)
               //{
               //    Vector3 minRange = new Vector3(listOfRooms[i].BottomLeftAreaCorner.x / 2f  , listOfRooms[i].BottomLeftAreaCorner.y /2f, 0f);
               //    Vector3 maxRange = new Vector3(listOfRooms[i].TopRightAreaCorner.x / 2f , listOfRooms[i].TopRightAreaCorner.y / 2f, 0f);
               //    PlacePlayerInWorld(minRange, maxRange);
               //    playerSpawned = true;
               //}
               
               if (!playerSpawned)
               {
                   SpawnPlayer(new Vector3((listOfRooms[i].BottomLeftAreaCorner.x + listOfRooms[i].TopRightAreaCorner.x) / 2,
                       1, (listOfRooms[i].BottomLeftAreaCorner.y + listOfRooms[i].TopRightAreaCorner.y) / 2));
                   playerSpawned = true;
               }

               if (!startingRoom)
               {
                   if (!obstacleSpawned)
                   {
                       Vector3 minRange = new Vector3(listOfRooms[i].BottomLeftAreaCorner.x / 2f  , listOfRooms[i].BottomLeftAreaCorner.y /2f, 5f);
                       Vector3 maxRange = new Vector3(listOfRooms[i].TopRightAreaCorner.x / 2f , listOfRooms[i].TopRightAreaCorner.y / 2f, 5f);
                       PlaceObstaclesInWorld(minRange, maxRange);
                       obstacleSpawned = true;
                   }
               }
           }

           if (!lootRoom) continue;
           {
               //if (!itemSpawned)
               //{
               //    Vector3 minRange = new Vector3(listOfRooms[i].BottomLeftAreaCorner.x / 2f  , 5f, listOfRooms[i].BottomLeftAreaCorner.y / 2f);
               //    Vector3 maxRange = new Vector3(listOfRooms[i].TopRightAreaCorner.x / 2f , 5f, listOfRooms[i].TopRightAreaCorner.y / 2f);
               //    PlaceEndItemInWorld(minRange, maxRange);
               //    itemSpawned = true;
               //}
               
               if (!itemSpawned)
               {
                   PlaceItemInWorld(new Vector3((listOfRooms[i].BottomLeftAreaCorner.x + listOfRooms[i].TopRightAreaCorner.x) / 2,
                       0, (listOfRooms[i].BottomLeftAreaCorner.y + listOfRooms[i].TopRightAreaCorner.y) / 2));
                   itemSpawned = true;
               }
           
          
           
               if (!collectableSpawned)
               {
                   PlaceCollectablesInWorld(new Vector3((listOfRooms[i].BottomLeftAreaCorner.x + listOfRooms[i].TopRightAreaCorner.x) / 2,
                       0, (listOfRooms[i].BottomLeftAreaCorner.y + listOfRooms[i].TopRightAreaCorner.y) / 2));
                   collectableSpawned = true;
               }
           }
       }
       CreateWalls(wallParent);
   }

   private void SpawnPlayer(Vector3 position)
   {
       var playerSpawn = Instantiate(player, position, Quaternion.identity);
       
       _playerSpawned = playerSpawn.transform;
   }
   
   private void CreateWalls(GameObject wallParent)
    {
        foreach (var wallPos in _possibleWallHorizPos)
        {
            CreateWall(wallParent, wallPos, wallHorizontal);
        }

        foreach (var wallPos in _possibleWallVertPos)
        {
            CreateWall(wallParent, wallPos, wallVertical);
        }
    }

    private void CreateWall(GameObject wallParent, Vector3Int wallPos, GameObject wallPrefab)
    {
        Instantiate(wallPrefab, wallPos, Quaternion.identity, wallParent.transform);
    }
    
    
    /// <summary>
    /// CodeMonkeys Implementation of creating a 3D mesh in code
    /// https://www.youtube.com/watch?v=gmuHI_wsOgI&ab_channel=CodeMonkey
    /// </summary>
    /// <param name="bottomLeftCorner"></param>
    /// <param name="topRightCorner"></param>
    private void CreateFloorAndRoof(Vector2 bottomLeftCorner, Vector2 topRightCorner)
    {
        Vector3 bottomLeftVert = new Vector3(bottomLeftCorner.x, 0, bottomLeftCorner.y);
        Vector3 bottomRightVert = new Vector3(topRightCorner.x, 0, bottomLeftCorner.y);
        Vector3 topLeftVert = new Vector3(bottomLeftCorner.x, 0, topRightCorner.y);
        Vector3 topRightVert = new Vector3(topRightCorner.x, 0, topRightCorner.y);

       Vector3[] vertices = new Vector3[]
       {
           topLeftVert,
           topRightVert,
           bottomLeftVert,
           bottomRightVert
       };

       //uvs calculation
       Vector2[] uvs = new Vector2[vertices.Length];
       for (int i = 0; i < uvs.Length; i++)
       {
           uvs[i] = new Vector2(vertices[i].x, vertices[i].z);
       }

        int[] tris = new int[]
        {
            0, 1, 2, 2, 1, 3
        };

        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.vertices = vertices;
        mesh.triangles = tris;

        // creates floor mesh
        
        _dungeonFloor = new GameObject("Mesh" + bottomLeftCorner, typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider));
        _dungeonFloor.transform.position = Vector3.zero;
        _dungeonFloor.transform.localScale = Vector3.one;
        _dungeonFloor.GetComponent<MeshFilter>().mesh = mesh;
        _dungeonFloor.GetComponent<MeshRenderer>().material = floorMaterial;
        _dungeonFloor.GetComponent<MeshCollider>().sharedMesh = mesh;
        //_dungeonFloor.GetComponent<MeshCollider>().isTrigger = true;

        _dungeonFloor.tag = "floor";

        _roomSpawned = _dungeonFloor.transform;
        _roomSpawned.transform.position = Vector3.zero;
       // Debug.Log("locationX" + _roomSpawned.transform.position.x);
       // Debug.Log("locationY" + _roomSpawned.transform.position.y);

        //// creates roof mesh
       GameObject dungeonRoof = new GameObject("Dungeon Roof" + topRightCorner, typeof(MeshFilter), typeof(MeshRenderer));
       dungeonRoof.transform.position = new Vector3(0, 3f, 0);
       dungeonRoof.transform.localScale = Vector3.one;
       dungeonRoof.GetComponent<MeshFilter>().mesh = mesh;
       dungeonRoof.GetComponent<MeshRenderer>().material = roofMaterial;
       dungeonRoof.transform.parent = transform;

        // walls
        for (int row = (int)bottomLeftVert.x; row < (int)bottomRightVert.x; row++)
        {
            var wallPos = new Vector3(row, 0, bottomLeftVert.z);
            AddWallPositionToList(wallPos, _possibleWallHorizPos);
        } 
        
        for (int row = (int)topLeftVert.x; row < (int)topRightCorner.x; row++)
        {
            var wallPos = new Vector3(row, 0, topRightVert.z);
            AddWallPositionToList(wallPos, _possibleWallHorizPos);
        }

        for (int col = (int)bottomLeftVert.z; col < (int)topLeftVert.z; col++)
        {
            var wallPos = new Vector3(bottomLeftVert.x, 0, col);
            AddWallPositionToList(wallPos, _possibleWallVertPos);
        }
        
        for (int col = (int)bottomRightVert.z; col < (int)topRightVert.z; col++)
        {
            var wallPos = new Vector3(bottomRightVert.x, 0, col);
            AddWallPositionToList(wallPos, _possibleWallVertPos);
        }
    }
    private void AddWallPositionToList(Vector3 wallPos, List<Vector3Int> wallList)
    {
        Vector3Int point = Vector3Int.CeilToInt(wallPos);
        if (wallList.Contains(point))
        {
            wallList.Remove(point);
        }
        else
        {
            wallList.Add(point);
        }
    }
    
    public void PlaceItemInWorld(Vector3 position)
    {
        
        for (int i = 0; i < ItemCount; i++)
        {
            float spawnY = Random.Range(position.x, position.y);
            float spawnX = Random.Range(position.x, position.x);
            
            Vector3 spawnPosition = new Vector3(spawnX, 0f, spawnY);
            var item = Instantiate(itemSpawn, position, Quaternion.identity);
            item.name = "End Item";
            item.transform.parent = transform.parent;
            item.transform.localPosition = spawnPosition;

            _itemSpawned = item.transform;
        }
    }
    public void PlaceObstaclesInWorld(Vector3 minRange, Vector3 maxRange)
    {
        for (int i = 0; i < obstacleCount; i++)
        {
            float spawnY = Random.Range(minRange.x, maxRange.y);
            float spawnX = Random.Range(minRange.x, maxRange.x);
            
            Vector3 spawnPosition = new Vector3(spawnX, 0f, spawnY);
            
            var obstacle = Instantiate(obstacleSpawns);
            obstacle.name = "ObstacleItems";
            obstacle.transform.parent = transform.parent;
            obstacle.transform.localPosition = spawnPosition;

            _obstacleSpawned = obstacle.transform;
        }
    }
    
    public void PlaceCollectablesInWorld(Vector3 position)
    {
        for (int i = 0; i < collectableCount; i++)
        {
            float spawnY = Random.Range(position.x, position.z);
            float spawnX = Random.Range(position.x, position.z);
            
            Vector3 spawnPosition = new Vector3(spawnX, 0f, spawnY);
            
            var collectables = collectableSpawns[i] = Instantiate(collectableSpawns[i], position, Quaternion.identity) as GameObject;
            collectables.name = "Collectable Items";
            collectables.transform.parent = transform.parent;
            collectables.transform.localPosition = spawnPosition;

            _collectablesSpawned = collectables.transform;
        }
    }
    private void InitializeMinimap()
    {
        minimapTexture = new Texture2D(dungeonWidth, dungeonLength);
        _exploredAreaColors = new Color[dungeonWidth * dungeonLength];
        _exploredAreaMap = new bool[dungeonWidth, dungeonLength];
        _unexploredAreaColors = new Color[dungeonWidth * dungeonLength];
        _unexploredAreaMap = new bool[roomWidthMin, roomLengthMin];

        for (int i = 0; i < dungeonWidth; i++)
        {
            for (int j = 0; j < dungeonLength; j++)
            {
                _unexploredAreaColors[j * dungeonWidth + i] = Color.black;
            }
        }
        
        minimapTexture.SetPixels(_unexploredAreaColors);
        minimapTexture.Apply();

        minimapImage.texture = minimapTexture;
    }

    private void UpdateMinimap()
    {
        var position = _playerSpawned.position;
        int spawnX = Mathf.RoundToInt(position.x);
        int spawnZ = Mathf.RoundToInt(position.z);

        var position1 = _itemSpawned.position;
        int itemSpawnX = Mathf.RoundToInt(position1.x);
        int itemSpawnZ = Mathf.RoundToInt(position1.z);
        
        var position4 = _collectablesSpawned.position;
        int collectableSpawnX = Mathf.RoundToInt(position4.x);
        int collectableSpawnZ = Mathf.RoundToInt(position4.z);
        
        if (_isInRoom && _roomSpawned != null)
        {
            var position3 = _roomSpawned.transform.position;
            int roomSpawnX = Mathf.RoundToInt(position3.x);
            int roomSpawnZ = Mathf.RoundToInt(position3.y);
            
            if (Mathf.Abs(spawnX - roomSpawnX) >= roomWidthMin / 2 || Mathf.Abs(spawnZ - roomSpawnZ) >= roomLengthMin / 2)
            {
                _isInRoom = false;
            }
        }
        
        if (spawnX >= 0 && spawnX < dungeonWidth && spawnZ >= 0 && spawnZ < dungeonLength)
        {
            if (!_exploredAreaMap[spawnX, spawnZ])
            {
                    minimapTexture.SetPixel(itemSpawnX, itemSpawnZ, _exploredItemColor);
                    minimapTexture.SetPixel(collectableSpawnX, collectableSpawnZ, _exploredCollectableColor);
                    _exploredAreaMap[spawnX, spawnZ] = true; 
                    minimapTexture.SetPixel(spawnX, spawnZ, _playerColor);
                    _exploredRoom = true;

                    minimapTexture.Apply();
                    
                    // Check if the player is over the floor mesh
                   if (IsPlayerInRoom(out Vector3 roomPosition, out Vector3 roomSize))
                   {
                       DrawRoomsOnMinimap(Mathf.RoundToInt(roomPosition.x), Mathf.RoundToInt(roomPosition.z), Mathf.RoundToInt(roomSize.x), Mathf.RoundToInt(roomSize.z));
                      
                       _isInRoom = true;
                   }
            }
        }
    }
    
   
    

    private bool IsPlayerInRoom(out Vector3 roomPosition, out Vector3 roomSize)
    {
        roomPosition = Vector3.forward;
        roomSize = Vector3.zero;
        RaycastHit hit;
        
        Ray ray = new Ray(_playerSpawned.position, Vector3.down);
        
        if (Physics.Raycast(ray, out hit, 2f))
        {
            if (hit.collider.CompareTag("floor"))
            {
                // Get the entire room's position and size based on the floor
                Collider roomCollider = hit.collider;
                roomPosition = roomCollider.bounds.min;
                roomSize = roomCollider.bounds.size;
                return true;
            }
        }

        return false;
    }
    
    private void DrawRoomsOnMinimap(int x, int z, int roomWidth, int roomLength)
    {
        for (int i = x; i < x + roomWidth; i++)
        {
            for (int j = z; j < z + roomLength; j++)
            {
                minimapTexture.SetPixel(i, j, _exploredRoomColor);
            }
        }
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

