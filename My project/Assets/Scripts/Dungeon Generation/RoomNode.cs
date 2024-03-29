﻿using System.Collections.Generic;
using UnityEngine;

namespace Dungeon_Generation
{
    public class RoomNode : Node
    {
        /// <summary>
        /// Storing information about the size of the rooms in vec2int
        /// 
        /// </summary>
        /// <param name="bottomLeftAreaCorner"></param>
        /// <param name="topRightAreaCorner"></param>
        /// <param name="parentNode"></param>
        /// <param name="index"></param>
        public RoomNode(Vector2Int bottomLeftAreaCorner, Vector2Int topRightAreaCorner, Node parentNode, int index) :
            base(parentNode)
        {
            this.BottomLeftAreaCorner = bottomLeftAreaCorner;
            this.TopRightAreaCorner = topRightAreaCorner;
            this.BottomRightAreaCorner = new Vector2Int(topRightAreaCorner.x, bottomLeftAreaCorner.y);
            this.TopLeftAreaCorner = new Vector2Int(bottomLeftAreaCorner.x, topRightAreaCorner.y);
            this.TreeLayerIndex = index;
        }
        
        public int Width
        {
            get => (int)(TopRightAreaCorner.x - BottomLeftAreaCorner.x);
        }
        public int Length
        {
            get => (int)(TopRightAreaCorner.y - BottomLeftAreaCorner.x);
        }
    }
}