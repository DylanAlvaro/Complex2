using System.Collections;
using System.Collections.Generic;
using Dungeon_Generation;
using UnityEditor;
using UnityEditor.TerrainTools;
using UnityEngine;

[CustomEditor(typeof(DungeonCreator))]

public class DungeonEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        DungeonCreator dungeonCreator = (DungeonCreator)target;
        
        if (GUILayout.Button("Generate BSP Dungeon"))
        {
            dungeonCreator.CreateDungeon();
        }
    }
    
}
