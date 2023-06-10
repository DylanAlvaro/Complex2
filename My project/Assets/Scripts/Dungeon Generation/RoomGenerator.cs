using System.Collections;
using System.Collections.Generic;
using Math;
using UnityEngine;
using Unity.VisualScripting;

namespace Dungeon_Generation
{
    public class RoomGenerator : MonoBehaviour
    {
        private int maxIterations;
        private int roomLengthMin;
        private int roomWidthMin;

      

        private List<Vector3Int> _possibleWallHorizPos;
        private List<Vector3Int> _possibleWallVertPos;
        private List<Vector3Int> _possibleFloorHorizPos;
        private List<Vector3Int> _possibleFloorVertPos;

        public Material floorMaterial;

        public RoomGenerator(int maxIterations, int roomLengthMin, int roomWidthMin)
        {
            this.maxIterations = maxIterations;
            this.roomLengthMin = roomLengthMin;
            this.roomWidthMin = roomWidthMin;
        }

        public List<RoomNode> GenerateRoomsInGivenSpaces(List<Node> roomSpaces, float roomBottomCornerModifier,
            float roomTopCornerMidifier, int roomOffset)
        {
            List<RoomNode> listToReturn = new List<RoomNode>();
            foreach (var space in roomSpaces)
            {
                Vector2Int newBottomLeftPoint = StructureHelper.GenerateBottomLeftCornerBetween(
                    space.BottomLeftAreaCorner, space.TopRightAreaCorner, roomBottomCornerModifier, roomOffset);

                Vector2Int newTopRightPoint = StructureHelper.GenerateTopRightCornerBetween(
                    space.BottomLeftAreaCorner, space.TopRightAreaCorner, roomTopCornerMidifier, roomOffset);
                space.BottomLeftAreaCorner = newBottomLeftPoint;
                space.TopRightAreaCorner = newTopRightPoint;
                space.BottomRightAreaCorner = new Vector2Int(newTopRightPoint.x, newBottomLeftPoint.y);
                space.TopLeftAreaCorner = new Vector2Int(newBottomLeftPoint.x, newTopRightPoint.y);
                listToReturn.Add((RoomNode)space);

            }

            return listToReturn;
        }
    }
}