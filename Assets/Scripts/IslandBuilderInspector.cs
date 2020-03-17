using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(IslandBuilder))]
public class IslandBuilderInspector : Editor
{
    private IslandBuilder _builder;

    private void OnEnable()
    {
        _builder = target as IslandBuilder;
    }

    public override void OnInspectorGUI()
    {
        //EditorGUI.BeginChangeCheck();
        DrawDefaultInspector();
        //if (EditorGUI.EndChangeCheck()) { }
        
        if(GUILayout.Button("Update Map Texture"))
        {
            _builder.UpdateMapTexture();
        }
        
        if(GUILayout.Button("Generate"))
        {
            _builder.BuildIslandAndInitRenderers();
        }
    }
}