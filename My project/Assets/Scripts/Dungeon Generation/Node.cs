using System.Collections.Generic;
using UnityEngine;

namespace Dungeon_Generation
{
    public abstract class Node
    {
        public List<Node> childNodeList;

        public List<Node> ChildNodeList
        {
            get => childNodeList;
        }
        
        public bool Visited { get; set; }
        public Vector2Int BottomLeftAreaCorner { get; set; }
        public Vector2Int BottomRightAreaCorner { get; set; }
        public Vector2Int TopRightAreaCorner { get; set; }
        public Vector2Int TopLeftAreaCorner { get; set; }
        
        public Node Parent { get; set; }
        
        public int TreeLayerIndex { get; set; }

        public Node(Node parentNode)
        {
            childNodeList = new List<Node>();
            this.Parent = parentNode;
            if (parentNode != null)
            {
                parentNode.AddChild(this);
            }
        }

        public void AddChild(Node node)
        {
            childNodeList.Add(node);
        }

        public void RemoveChild(Node node)
        {
            childNodeList.Remove(node);
        }
    }
}