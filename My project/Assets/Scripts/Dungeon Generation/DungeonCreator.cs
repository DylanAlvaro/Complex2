using System;
using System.Collections;
using System.Collections.Generic;
using Dungeon_Generation;
using Math;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
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
    
    [Header("Enemey Values")]
    public GameObject enemeyPrefab;
    public int enemeyCount;
    public GameObject[] changeEnemiesOBJ;
    public GameObject _enemiesObject;
    
    private List<Vector3Int> _possibleWallHorizPos;
    private List<Vector3Int> _possibleWallVertPos;
    private List<Vector3Int> _possibleFloorHorizPos;
    private List<Vector3Int> _possibleFloorVertPos;
    
    public bool playerSpawned = false;
    public bool itemSpawned = false;
    
   public void Start()
    {
        CreateDungeon();
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
        
        // creates floor mesh
       GameObject dungeonFloor = new GameObject("Mesh" + bottomLeftCorner, typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider), typeof(NavMesh));
       dungeonFloor.transform.position = Vector3.zero;
       dungeonFloor.transform.localScale = Vector3.one;
       dungeonFloor.GetComponent<MeshFilter>().mesh = mesh;
       dungeonFloor.GetComponent<MeshRenderer>().material = floorMaterial;
       dungeonFloor.GetComponent<MeshCollider>().sharedMesh = mesh;


       // creates roof mesh
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
    
    public void PlacePlayerInWorld(Vector3 minRange, Vector3 maxRange)
    {
        float spawnX = Random.Range(minRange.x, maxRange.x );
        float spawnY = Random.Range(minRange.y, maxRange.y );
        Vector3 spawnPosition = new Vector3(spawnX, 0f, spawnY);
        
        var playerSpawn = Instantiate(player);
        playerSpawn.name = "Player";
        playerSpawn.transform.parent = transform.parent;
        playerSpawn.transform.localPosition = spawnPosition;
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
        }
    }

    public void PlaceEnemiesInWorld()
    {
        _enemiesObject = new GameObject("Enemies");
        _enemiesObject.transform.parent = transform;
        _enemiesObject.transform.localPosition = Vector3.zero;

        changeEnemiesOBJ = new GameObject[enemeyCount];

        for (int i = 0; i < enemeyCount; i++)
        {
            GameObject newEnemy = Instantiate(enemeyPrefab);
            newEnemy.name = "enemey" + (i + 1);
            newEnemy.transform.localPosition = new Vector3(i / 2f, 0f, i % 2f);
            changeEnemiesOBJ[i] = newEnemy;
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

