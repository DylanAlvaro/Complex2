using Dungeon_Generation;
using UnityEditor;
using UnityEngine;

namespace DefaultNamespace
{
    [CustomEditor(typeof(DelaunayDungeon))]
    public class DungeonDeluanyEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            DelaunayDungeon dungeonCreator = (DelaunayDungeon)target;
        
            if (GUILayout.Button("Generate Delunay Dungeon"))
            {
                dungeonCreator.CreateRooms();
            }
        }
    }
}