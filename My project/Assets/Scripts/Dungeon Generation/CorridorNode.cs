using System.Collections.Generic;
using System.Linq;
using Math;
using Unity.VisualScripting;
using UnityEngine;

namespace Dungeon_Generation
{
    public class CorridorNode : Node
    {
        private Node _structure1;
        private Node _structure2;
        private int _corridorWidth;
        private int _modifierDistanceFromWall = 1;

        public CorridorNode(Node node1, Node node2, int corridorWidth) : base(null)
        {
            this._structure1 = node1;
            this._structure2 = node2;
            this._corridorWidth = corridorWidth;
            GenerateCorridor();
        }

        private void GenerateCorridor()
        {
            var relativePositionofRooms2 = CheckPositionStructure2AgainstStruct1();
            switch (relativePositionofRooms2)
            {
                case StructureHelper.RelativePosition.Up:
                    ProcessRoomInRelationUpOrDown(this._structure1, this._structure2);
                    break;
                case StructureHelper.RelativePosition.Down:
                    ProcessRoomInRelationUpOrDown(this._structure2, this._structure1);
                    break;
                case  StructureHelper.RelativePosition.Right:
                    ProcessRoomInRelationRightOrLeft(this._structure1, this._structure2);
                    break;
                case StructureHelper.RelativePosition.Left:
                    ProcessRoomInRelationRightOrLeft(this._structure2, this._structure1);
                    break;
                default:
                    break;
            }
        }

        private void ProcessRoomInRelationRightOrLeft(Node structure1, Node structure2)
        {
            Node leftStruct = null;
            List<Node> leftStructChild = StructureHelper.TraverseGraphToExtractLowestLeafes(structure1);
            Node rightStruct = null;
            List<Node> rightStructChild = StructureHelper.TraverseGraphToExtractLowestLeafes(structure2);

            var sortedLeftStruct = leftStructChild.OrderByDescending(child => child.TopRightAreaCorner.x).ToList();
            if (sortedLeftStruct.Count == 1)
            {
                leftStruct = sortedLeftStruct[0];
            }
            else
            {
                int maxX = sortedLeftStruct[0].TopRightAreaCorner.x;
                sortedLeftStruct = sortedLeftStruct.Where(children => Mathf.Abs(maxX - children.TopRightAreaCorner.x) < 10).ToList();
                int index = Random.Range(0, sortedLeftStruct.Count);
                leftStruct = sortedLeftStruct[index];
            }

            var possibleNeighboursInRightStructList = rightStructChild.Where(
                child => GetValidYForNeighbourLeftRight(leftStruct.TopRightAreaCorner, 
                    leftStruct.BottomRightAreaCorner, child.TopLeftAreaCorner, child.BottomLeftAreaCorner) != -1).OrderBy(child => child.BottomRightAreaCorner.x).ToList();

            if (possibleNeighboursInRightStructList.Count <= 0)
            {
                rightStruct = structure2;
            }
            else
            {
                rightStruct = possibleNeighboursInRightStructList[0];
            }

            int y = GetValidYForNeighbourLeftRight(leftStruct.TopLeftAreaCorner, leftStruct.BottomRightAreaCorner,
                rightStruct.TopLeftAreaCorner, rightStruct.BottomLeftAreaCorner);
            
            
            while (y == -1 && sortedLeftStruct.Count > 0)
            {
                sortedLeftStruct = sortedLeftStruct
                    .Where(child => child.TopLeftAreaCorner.y != leftStruct.TopLeftAreaCorner.y).ToList();
                leftStruct = sortedLeftStruct[0];
                y = GetValidYForNeighbourLeftRight(leftStruct.TopLeftAreaCorner, leftStruct.BottomRightAreaCorner,
                    rightStruct.TopLeftAreaCorner, rightStruct.BottomLeftAreaCorner);
            }

            BottomLeftAreaCorner = new Vector2Int(leftStruct.BottomRightAreaCorner.x, y);
            TopRightAreaCorner = new Vector2Int(rightStruct.TopLeftAreaCorner.x, y + this._corridorWidth);
        }

        /// <summary>
        /// Different cases to consider when creating corridors:
        /// What if a room is longer on the Y than the room next to it?
        /// What if a room is smaller than the room next to it?
        /// What if a room is on a different Y coordinate (higher) than the room next to it?
        /// What if a room is on a different Y coordinate (lower) than the room next to it?
        ///
        /// This function will be the meat of this math to work
        /// </summary>
        /// <param name="leftNodeUp"></param>
        /// <param name="leftNodeDown"></param>
        /// <param name="rightNodeUp"></param>
        /// <param name="rightNodeDown"></param>
        /// <returns></returns>
        private int GetValidYForNeighbourLeftRight(Vector2Int leftNodeUp, Vector2Int leftNodeDown, Vector2Int rightNodeUp, Vector2Int rightNodeDown)
        {
            if (rightNodeUp.y >= leftNodeUp.y && leftNodeDown.y >= rightNodeDown.y)
            {
                return StructureHelper.CalculateMiddlePoint(
                    leftNodeDown + new Vector2Int(0, _modifierDistanceFromWall),
                    leftNodeUp - new Vector2Int(0, _modifierDistanceFromWall + this._corridorWidth)
                    ).y;
            }

            if (rightNodeUp.y <= leftNodeUp.y && leftNodeDown.y <= rightNodeDown.y)
            {
                return StructureHelper.CalculateMiddlePoint(
                    rightNodeDown + new Vector2Int(0, _modifierDistanceFromWall),
                    rightNodeUp - new Vector2Int(0, _modifierDistanceFromWall + this._corridorWidth)
                ).y;
            }

            if (leftNodeUp.y >= rightNodeDown.y && leftNodeUp.y <= rightNodeUp.y)
            {
                return StructureHelper.CalculateMiddlePoint(
                    rightNodeDown + new Vector2Int(0, _modifierDistanceFromWall),
                    leftNodeUp - new Vector2Int(0, _modifierDistanceFromWall)).y;
            }

            if (leftNodeDown.y >= rightNodeDown.y && leftNodeDown.y <= rightNodeUp.y)
            {
                return StructureHelper.CalculateMiddlePoint(
                    leftNodeDown + new Vector2Int(0, _modifierDistanceFromWall),
                    rightNodeUp - new Vector2Int(0, _modifierDistanceFromWall + this._corridorWidth)
                ).y;
            }

            return -1;
        }
        
        private void ProcessRoomInRelationUpOrDown(Node structure1, Node structure2)
        {
            Node bottomStruct = null;
            List<Node> structBottomChild = StructureHelper.TraverseGraphToExtractLowestLeafes(structure1);

            Node topStruct = null;
            List<Node> structAboveChild = StructureHelper.TraverseGraphToExtractLowestLeafes(structure2);

            var sortedBottomStruct = structBottomChild.OrderByDescending(child => child.TopRightAreaCorner.y).ToList();

            if (sortedBottomStruct.Count == 1)
            {
                bottomStruct = structBottomChild[0];
            }
            else
            {
                int maxY = sortedBottomStruct[0].TopLeftAreaCorner.y;
                sortedBottomStruct = sortedBottomStruct.Where(child => Mathf.Abs(maxY - child.TopLeftAreaCorner.y) < 10).ToList();

                int index = Random.Range(0, sortedBottomStruct.Count);
                bottomStruct = sortedBottomStruct[index];
            }

            var possibleNeighboursInTopStruct = structAboveChild.Where(child => GetValidXForNeighbourUpDown(
                bottomStruct.TopLeftAreaCorner, bottomStruct.TopRightAreaCorner, child.BottomLeftAreaCorner,
                child.BottomRightAreaCorner) != -1).OrderBy(child => child.BottomRightAreaCorner.y).ToList();

            if (possibleNeighboursInTopStruct.Count == 0)
            {
                topStruct = structure2;
            }
            else
            {
                topStruct = possibleNeighboursInTopStruct[0];
            }

            int x = GetValidXForNeighbourUpDown(bottomStruct.TopLeftAreaCorner, 
                bottomStruct.TopRightAreaCorner,
                topStruct.BottomLeftAreaCorner, 
                topStruct.BottomRightAreaCorner);

            while (x == -1 && sortedBottomStruct.Count > 1)
            {
                sortedBottomStruct = sortedBottomStruct
                    .Where(child => child.TopLeftAreaCorner.x != topStruct.TopLeftAreaCorner.x).ToList();
                bottomStruct = sortedBottomStruct[0];
                
                x = GetValidXForNeighbourUpDown(bottomStruct.TopLeftAreaCorner, 
                    bottomStruct.TopRightAreaCorner,
                    topStruct.BottomLeftAreaCorner, 
                    topStruct.BottomRightAreaCorner);
            }

            BottomLeftAreaCorner = new Vector2Int(x, bottomStruct.TopLeftAreaCorner.y);
            TopRightAreaCorner = new Vector2Int(x + this._corridorWidth, topStruct.BottomLeftAreaCorner.y);
        }

        private int GetValidXForNeighbourUpDown(Vector2Int bottomNodeLeft, Vector2Int bottomNodeRight, Vector2Int topNodeLeft, Vector2Int topNodeRight)
        {
            if (topNodeLeft.x < bottomNodeLeft.x && bottomNodeRight.x < topNodeRight.x)
            {
                return StructureHelper.CalculateMiddlePoint(
                    bottomNodeLeft + new Vector2Int(_modifierDistanceFromWall, 0),
                    bottomNodeRight - new Vector2Int(this._corridorWidth + _modifierDistanceFromWall, 0)).x;
            }

            if (topNodeLeft.x >= bottomNodeLeft.x && bottomNodeRight.x >= topNodeRight.x)
            {
                return StructureHelper.CalculateMiddlePoint(
                    topNodeLeft + new Vector2Int(_modifierDistanceFromWall, 0),
                    topNodeRight - new Vector2Int(this._corridorWidth + _modifierDistanceFromWall, 0)).x;
            }

            if (bottomNodeLeft.x >= topNodeLeft.x && bottomNodeLeft.x <= topNodeRight.x)
            {
                return StructureHelper.CalculateMiddlePoint(
                    bottomNodeLeft + new Vector2Int(_modifierDistanceFromWall, 0),
                    topNodeRight - new Vector2Int(this._corridorWidth + _modifierDistanceFromWall, 0)).x;
            }

            if (bottomNodeRight.x <= topNodeRight.x && bottomNodeRight.x >= topNodeLeft.x)
            {
                return StructureHelper.CalculateMiddlePoint(
                    topNodeLeft + new Vector2Int(_modifierDistanceFromWall, 0),
                    bottomNodeRight - new Vector2Int(this._corridorWidth + _modifierDistanceFromWall, 0)).x;
            }

            return -1;
        }

        private StructureHelper.RelativePosition CheckPositionStructure2AgainstStruct1()
        {
            Vector2 middlePointStructure1Temp =
                ((Vector2)_structure1.TopRightAreaCorner + _structure1.BottomLeftAreaCorner) / 2;
            Vector2 middlePointStruct2Temp =
                ((Vector2)_structure2.TopRightAreaCorner + _structure2.BottomLeftAreaCorner) / 2;
            
            float angle = CalculateAngle(middlePointStruct2Temp, middlePointStructure1Temp);

            if ((angle < 45 && angle >= 0 || (angle > -45 && angle < 0)))
            {
                return StructureHelper.RelativePosition.Right;
            }
            else if(angle > 45 && angle < 135)
            {
                return StructureHelper.RelativePosition.Up;
            }
            else if (angle > -135 && angle < -45)
                return StructureHelper.RelativePosition.Down;
            else
            {
                return StructureHelper.RelativePosition.Left;
            }
        }

        private float CalculateAngle(Vector2 middlePointStruct2Temp, Vector2 middlePointStructure1Temp)
        {
            return Mathf.Atan2(middlePointStruct2Temp.y - middlePointStructure1Temp.y,
                middlePointStruct2Temp.x - middlePointStructure1Temp.x) * Mathf.Rad2Deg;
        }
    }
}