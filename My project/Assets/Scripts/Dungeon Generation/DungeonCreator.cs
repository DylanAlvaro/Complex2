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
    public Vector3Int maxRoomSize;
    public Vector3Int size;
    
    [Header("Dungeon Room Modifiers")]
    public Material floorMaterial;
    public Material roofMaterial;
    [Range(0.0f, 0.3f)]
    public float roomBottomCornerModifier;
    [Range(0.7f, 1.0f)]
    public float roomTopCornerMidifier;
    [Range(0, 2)]
    public int roomOffset;

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

    [Header("Minimap")] 
    public Texture2D exploredAreas;
    public RawImage minimapImage;
    public Texture2D minimapTexture;
    private Color[] _exploredAreaColors;
    private bool[,] _exploredAreaMap;
    private Color _playerColor = Color.blue;
    private Color _exploredItemColor = Color.red;
    private Color _exploredCollectableColor = Color.yellow;
    private Color _exploredRoomColor = Color.green;
    private Transform _minimap;
    private bool _exploredRoom;
    

    private List<Vector3Int> _possibleWallHorizPos;
    private List<Vector3Int> _possibleWallVertPos;
    private List<Vector3Int> _possibleFloorHorizPos;
    private List<Vector3Int> _possibleFloorVertPos;

    private Transform _playerSpawned;
    private Transform _itemSpawned;
    private Transform _obstacleSpawned;
    private Transform _collectablesSpawned;
    private Transform _roomSpawned;
    
    private Vector3 _exploredHallway;
    private GameObject _dungeonFloor;
    private bool playerSpawned = false;
    private bool itemSpawned = false;
    private bool obstacleSpawned = false;
    private bool collectableSpawned = false;
    private bool minimapSpawned = false;

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
       
       playerSpawned = false;
       itemSpawned = false;
       obstacleSpawned = false;
       collectableSpawned = false;
       minimapSpawned = false;
       
       // Will generate the room 
       for (int i = 0; i < listOfRooms.Count; i++)
       {
           CreateFloorAndRoof(listOfRooms[i].BottomLeftAreaCorner, listOfRooms[i].TopRightAreaCorner);
           
           if (!playerSpawned)
           {
               Vector3 minRange = new Vector3(listOfRooms[i].BottomLeftAreaCorner.x / 2f  , listOfRooms[i].BottomLeftAreaCorner.y /2f, 0f);
               Vector3 maxRange = new Vector3(listOfRooms[i].TopRightAreaCorner.x / 2f , listOfRooms[i].TopRightAreaCorner.y / 2f, 0f);
               PlacePlayerInWorld(minRange, maxRange);
               playerSpawned = true;
           }

           if (!itemSpawned)
           {
               Vector3 minRange = new Vector3(listOfRooms[i].BottomLeftAreaCorner.x / 2f  , listOfRooms[i].BottomLeftAreaCorner.y /2f, 5f);
               Vector3 maxRange = new Vector3(listOfRooms[i].TopRightAreaCorner.x / 2f , listOfRooms[i].TopRightAreaCorner.y / 2f, 5f);
               PlaceEndItemInWorld(minRange, maxRange);
               itemSpawned = true;
           }
           
           if (!obstacleSpawned)
           {
               Vector3 minRange = new Vector3(listOfRooms[i].BottomLeftAreaCorner.x / 2f  , listOfRooms[i].BottomLeftAreaCorner.y /2f, 5f);
               Vector3 maxRange = new Vector3(listOfRooms[i].TopRightAreaCorner.x / 2f , listOfRooms[i].TopRightAreaCorner.y / 2f, 5f);
               PlaceObstaclesInWorld(minRange, maxRange);
               obstacleSpawned = true;
           }
           
           if (!collectableSpawned)
           {
               Vector3 minRange = new Vector3(listOfRooms[i].BottomLeftAreaCorner.x / 2f  , listOfRooms[i].BottomLeftAreaCorner.y /2f, 5f);
               Vector3 maxRange = new Vector3(listOfRooms[i].TopRightAreaCorner.x / 2f , listOfRooms[i].TopRightAreaCorner.y / 2f, 5f);
               PlaceCollectablesInWorld(minRange, maxRange);
               collectableSpawned = true;
           }
       }
       CreateWalls(wallParent);
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
        
        //adds rooms to minimap (hopefully)
        
        
        
        // creates floor mesh
        
        _dungeonFloor = new GameObject("Mesh" + bottomLeftCorner, typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider));
        _dungeonFloor.transform.position = Vector3.zero;
        _dungeonFloor.transform.localScale = Vector3.one;
        _dungeonFloor.GetComponent<MeshFilter>().mesh = mesh;
        _dungeonFloor.GetComponent<MeshRenderer>().material = floorMaterial;
        _dungeonFloor.GetComponent<MeshCollider>().sharedMesh = mesh;

        _roomSpawned = _dungeonFloor.transform;
        _roomSpawned.transform.position = Vector3.zero;
        Debug.Log("location" + _roomSpawned.transform);

        //// creates roof mesh
       //GameObject dungeonRoof = new GameObject("Dungeon Roof" + topRightCorner, typeof(MeshFilter), typeof(MeshRenderer));
       //dungeonRoof.transform.position = new Vector3(0, 3f, 0);
       //dungeonRoof.transform.localScale = Vector3.one;
       //dungeonRoof.GetComponent<MeshFilter>().mesh = mesh;
       //dungeonRoof.GetComponent<MeshRenderer>().material = roofMaterial;
       //dungeonRoof.transform.parent = transform;

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
    
    public void PlacePlayerInWorld(Vector3 minRange, Vector3 maxRange)
    { 
        float spawnX = Random.Range(minRange.x, maxRange.x );
        float spawnY = Random.Range(minRange.y, maxRange.y );
        
        Vector3 spawnPosition = new Vector3(spawnX, 0f, spawnY);
        
        var playerSpawn = Instantiate(player);
        playerSpawn.name = "Player";
        playerSpawn.transform.parent = transform.parent;
        playerSpawn.transform.localPosition = spawnPosition;

        _playerSpawned = playerSpawn.transform;
    }

    public void PlaceEndItemInWorld(Vector3 minRange, Vector3 maxRange)
    {
        for (int i = 0; i < ItemCount; i++)
        {
            float spawnY = Random.Range(minRange.x, maxRange.y);
            float spawnX = Random.Range(minRange.x, maxRange.x);
            
            Vector3 spawnPosition = new Vector3(spawnX, 0f, spawnY);
            
            var item = Instantiate(itemSpawn);
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
        }
    }
    
    public void PlaceCollectablesInWorld(Vector3 minRange, Vector3 maxRange)
    {
        for (int i = 0; i < collectableCount; i++)
        {
            float spawnY = Random.Range(minRange.x, maxRange.z);
            float spawnX = Random.Range(minRange.x, maxRange.z);
            
            Vector3 spawnPosition = new Vector3(spawnX, 0f, spawnY);
            
            //var collectables = Instantiate(collectableSpawns);

            var collectables = collectableSpawns[i] = Instantiate(collectableSpawns[i]) as GameObject;
            collectables.name = "ObstacleItems";
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

        for (int i = 0; i < dungeonWidth; i++)
        {
            for (int j = 0; j < dungeonLength; j++)
            {
                _exploredAreaColors[j * dungeonWidth + i] = Color.black;
            }
        }
        
        minimapTexture.SetPixels(_exploredAreaColors);
        minimapTexture.Apply();

        minimapImage.texture = minimapTexture;
    }

    private void UpdateMinimap()
    {
        int SpawnX = Mathf.RoundToInt(_playerSpawned.position.x);
        int SpawnY = Mathf.RoundToInt(_playerSpawned.position.z);

        int itemSpawnX = Mathf.RoundToInt(_itemSpawned.position.x);
        int itemSpawnZ = Mathf.RoundToInt(_itemSpawned.position.z);
        
        int roomSpawnX = Mathf.RoundToInt(_roomSpawned.position.x);
        int roomSpawnZ = Mathf.RoundToInt(_roomSpawned.position.z);
        
        int collectableSpawnX = Mathf.RoundToInt(_collectablesSpawned.position.x);
        int collectableSpawnZ = Mathf.RoundToInt(_collectablesSpawned.position.z);
        
        if (SpawnX >= 0 && SpawnX < dungeonWidth && SpawnY >= 0 && SpawnY < dungeonLength)
        {
            if (!_exploredAreaMap[SpawnX, SpawnY])
            {
                _exploredAreaMap[SpawnX, SpawnY] = true;
                minimapTexture.SetPixel(SpawnX, SpawnY, _playerColor);
                minimapTexture.SetPixel(itemSpawnX, itemSpawnZ, _exploredItemColor);
                minimapTexture.SetPixel(roomSpawnX, roomSpawnZ, _exploredRoomColor);
                minimapTexture.SetPixel(collectableSpawnX, collectableSpawnZ, _exploredCollectableColor);
                minimapTexture.Apply();
            }
        }
    }

    private void AddRoomsToMinimap(Vector2 bottomLeftCorner, Vector2 topRightCorner)
    {
        // Vector3 roomMinimapPos = new Vector3(bottomLeftCorner.x, 0f, topRightCorner.y);
//
        // if (!_exploredAreaMap[roomMinimapPos, roomMinimapPos])
        // {
        //     
        // }
         
        // minimapImage = Instantiate(minimapImage, _minimap); 
        // minimapImage.transform.localPosition = roomMinimapPos;
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

