using System.Collections.Generic;

using UnityEngine;

namespace Scripts.Dungeon.Pathfinder
{
	public abstract class Node2
	{
		public bool visited { get; set; }

		private List<Node2> _nodeList;
		public Node2 Parent { get; set; }

		public List<Node2> NodeList
		{
			get => _nodeList;
		}
		
		public Vector2Int BottomLeftAreaCorner { get; set; }
		public Vector2Int BottomRightAreaCorner { get; set; }
		public Vector2Int TopRightAreaCorner { get; set; }
		public Vector2Int TopLeftAreaCorner { get; set; }
		
		public Node2(Node2 parentNode)
		{
			_nodeList = new List<Node2>();
			this.Parent = parentNode;
			if(parentNode != null)
			{
				parentNode.AddChild(this);
			}
		}
		
		
		public void AddChild(Node2 node)
		{
			_nodeList.Add(node);
		}

		public void RemoveChild(Node2 node)
		{
			_nodeList.Add(node);
		}
	}
}