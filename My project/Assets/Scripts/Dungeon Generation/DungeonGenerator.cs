using System.Collections.Generic;
using System.Linq;
using Algorithms;
using Graphs;
using Math;
using UnityEngine;

namespace Dungeon_Generation
{
    public class DungeonGenerator : MonoBehaviour
    {
        public int dungeonWidth;
        public int dungeonLength;
        public int dungeonHeight;
        public List<RoomNode> AllNodeCollection = new List<RoomNode>();
        public Node rootNode;
        public List<Rooms> rooms;

        public DungeonGenerator(int dungeonWidth, int dungeonLength)
        {
            this.dungeonWidth = dungeonWidth;
            this.dungeonLength = dungeonLength;
        }
        /// <summary>
        /// important information for creating the rooms and corridors used in dungeon creator script
        /// </summary>
        /// <param name="maxIterations"></param>
        /// <param name="roomWidthMin"></param>
        /// <param name="roomLengthMin"></param>
        /// <param name="roomBottomCornerModifier"></param>
        /// <param name="roomTopCornerMidifier"></param>
        /// <param name="roomOffset"></param>
        /// <param name="corridorWidth"></param>
        /// <returns></returns>
        public List<Node> CalculateRooms(int maxIterations, int roomWidthMin, int roomLengthMin, float roomBottomCornerModifier, float roomTopCornerMidifier, int roomOffset, int corridorWidth)
        {
            BinarySpace bsp = new BinarySpace(dungeonWidth, dungeonLength);
            AllNodeCollection = bsp.PrepareNodesCollection(maxIterations, roomWidthMin, roomLengthMin);
           List<Node> roomSpaces = StructureHelper.TraverseGraphToExtractLowestLeafes(bsp.rootNode);

           RoomGenerator roomGenerator = new RoomGenerator(maxIterations, roomLengthMin, roomWidthMin);
            List<RoomNode> roomList = roomGenerator.GenerateRoomsInGivenSpaces(roomSpaces, roomBottomCornerModifier, roomTopCornerMidifier, roomOffset);

            CorridorsGenerator corridorsGenerator = new CorridorsGenerator();
            var corridorList = corridorsGenerator.CreateCorridor(AllNodeCollection, corridorWidth);
            
            return new List<Node>(roomList).Concat(corridorList).ToList();
        }
    }
}