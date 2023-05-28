using System;
using System.Collections;
using System.Collections.Generic;
using Dungeon_Generation;
using Math;
using UnityEngine;

public class DungeonCreator : MonoBehaviour
{
    public int dungeonWidth; 
    public int dungeonLength;
    public int dungeonHeight;
    public int roomWidthMin, roomLengthMin; 
    public int maxIterations; 
    public int corridorWidth;
    
    public Material material;
    [Range(0.0f, 0.3f)]
    public float roomBottomCornerModifier;
    [Range(0.7f, 1.0f)]
    public float roomTopCornerMidifier;
    [Range(0, 2)]
    public int roomOffset;

    public GameObject wallVertical;
    public GameObject wallHorizontal;

    public GameObject floorHorizontal;
    public GameObject floorVertical;

    private List<Vector3Int> _possibleDoorVertPos;
    private List<Vector3Int> _possibleDoorHorizPos;
    private List<Vector3Int> _possibleWallHorizPos;
    private List<Vector3Int> _possibleWallVertPos;
    
    private List<Vector3Int> _possibleFloorHorizPos;
    private List<Vector3Int> _possibleFloorVertPos;

    // Start is called before the first frame update
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
        _possibleDoorVertPos = new List<Vector3Int>();
        _possibleDoorHorizPos = new List<Vector3Int>();
        _possibleWallHorizPos = new List<Vector3Int>();
        _possibleWallVertPos = new List<Vector3Int>();

        GameObject floorParent = new GameObject("FloorParent");
        _possibleFloorHorizPos = new List<Vector3Int>();
        _possibleFloorVertPos = new List<Vector3Int>();

        for (int i = 0; i < listOfRooms.Count; i++)
        {
            //CreateFloors(floorParent, listOfRooms[i].BottomLeftAreaCorner, listOfRooms[i].TopRightAreaCorner);
            CreateMesh(listOfRooms[i].BottomLeftAreaCorner, listOfRooms[i].TopRightAreaCorner);
        }
        
        CreateWalls(wallParent);
       // CreateFloors(floorParent);
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

    private void CreateFloors(GameObject floorParent)
    {
        foreach (var floorPos in _possibleFloorHorizPos)
        {
            CreateFloor(floorParent, floorPos, floorHorizontal);
        }
        
        foreach (var floorPos in _possibleFloorVertPos)
        {
            CreateFloor(floorParent, floorPos, floorVertical);
        }
    }

    private void CreateFloor(GameObject floorParent, Vector3Int floorPos, GameObject floorPrefab)
    {
        Instantiate(floorPrefab, floorPos, Quaternion.identity, floorParent.transform);
    }


    /// <summary>
    /// CodeMonkeys Implementation of creating a 3D mesh in code
    /// </summary>
    /// <param name="bottomLeftCorner"></param>
    /// <param name="topRightCorner"></param>
    
    private void CreateMesh(Vector2 bottomLeftCorner, Vector2 topRightCorner)
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

        GameObject dungeonFloor = new GameObject("Mesh" + bottomLeftCorner, typeof(MeshFilter), typeof(MeshRenderer));
        
        dungeonFloor.transform.position = Vector3.zero;
        dungeonFloor.transform.localScale = Vector3.one;
        dungeonFloor.GetComponent<MeshFilter>().mesh = mesh;
        dungeonFloor.GetComponent<MeshRenderer>().material = material;
        dungeonFloor.transform.parent = transform;

        for (int row = (int)bottomLeftVert.x; row < (int)bottomRightVert.x; row++)
        {
            var wallPos = new Vector3(row, 0, bottomLeftVert.z);
            AddWallPositionToList(wallPos, _possibleWallHorizPos, _possibleDoorHorizPos);
        }

        for (int row = (int)topLeftVert.x; row < (int)topRightCorner.x; row++)
        {
            var wallPos = new Vector3(row, 0, topRightVert.z);
            AddWallPositionToList(wallPos, _possibleWallHorizPos, _possibleDoorHorizPos);
        }

        for (int col = (int)bottomLeftVert.z; col < (int)topLeftVert.z; col++)
        {
            var wallPos = new Vector3(bottomLeftVert.x, 0, col);
            AddWallPositionToList(wallPos, _possibleWallVertPos, _possibleDoorVertPos);
        }
        
        for (int col = (int)bottomRightVert.z; col < (int)topRightVert.z; col++)
        {
            var wallPos = new Vector3(bottomRightVert.x, 0, col);
            AddWallPositionToList(wallPos, _possibleWallVertPos, _possibleDoorVertPos);
        }
        
    }

    private void AddWallPositionToList(Vector3 wallPos, List<Vector3Int> wallList, List<Vector3Int> doorList)
    {
        Vector3Int point = Vector3Int.CeilToInt(wallPos);
        if (wallList.Contains(point))
        {
            doorList.Add(point);
            wallList.Remove(point);
        }
        else
        {
            wallList.Add(point);
        }
    }
    
    void PlaceCube(Vector3Int location, Vector3Int size) 
    {
        floorHorizontal = Instantiate(floorHorizontal, location, Quaternion.identity);
        floorHorizontal.GetComponent<Transform>().localScale = size;
        floorHorizontal.GetComponent<MeshRenderer>().material = material;
    }

    void PlaceRoom(Vector3Int location, Vector3Int size) 
    {
        PlaceCube(location, size);
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
