﻿using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TerrainCreator))]
public class TerrainCreatorInspector : Editor
{
    private TerrainCreator creator;

    private void OnEnable()
    {
        creator = target as TerrainCreator;
        Undo.undoRedoPerformed += RefreshCreator;
    }

    private void OnDisable()
    {
        Undo.undoRedoPerformed -= RefreshCreator;
    }

    private void RefreshCreator()
    {
        creator.SetHeights();
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        
        if(GUILayout.Button("Apply"))
        {
            RefreshCreator();
        }
    }
}