using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Dungeon_Generation
{
    public class CorridorsGenerator : MonoBehaviour
    {
        /// <summary>
        /// adds to list of nodes
        /// </summary>
        /// <param name="nodeCollection"></param>
        /// <param name="corridorWidth"></param>
        /// <returns></returns>
        public List<Node> CreateCorridor(List<RoomNode> nodeCollection, int corridorWidth)
        {
            List<Node> corridorList = new List<Node>();
            Queue<RoomNode> structuresToCheck =
                new Queue<RoomNode>(nodeCollection.OrderByDescending(node => node.TreeLayerIndex).ToList());

            while (structuresToCheck.Count > 0)
            {
                var node = structuresToCheck.Dequeue();
                if (node.ChildNodeList.Count == 0) 
                    continue;

                CorridorNode corridorNode =
                    new CorridorNode(node.ChildNodeList[0], node.ChildNodeList[1], corridorWidth);
                corridorList.Add(corridorNode);
            }

            return corridorList;
        } 
    }
}