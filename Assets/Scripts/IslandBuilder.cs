using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class IslandBuilder : MonoBehaviour
{
    public float size = 100;
    public int treeNodeCount = 50;
    public float treeMaxGrowthLength = 10f;
    public TreeRenderer treeRenderer;
    public int voronoiPointCount = 100;
    public float voronoiSizeScale = 1f;
    public int voronoiRelaxation = 0;
    public VoronoiRenderer voronoiRenderer;

    private Tree _tree;

    public void Start()
    {
        _tree = RandomTreeBuilder.GetRandomTree(treeNodeCount, treeMaxGrowthLength, size);
        
        // Set tree on TreeRenderer
        if (treeRenderer != null)
        {
            treeRenderer.SetTree(_tree);
        }
        
        // Set points on VoronoiRenderer
        if (voronoiRenderer != null)
        {
            var voronoiSize = size * voronoiSizeScale;

            float scaleDifference = (size * voronoiSizeScale) - size;
            
            var bounds = new Rectf(
                0 - scaleDifference * 0.5f,
                0 - scaleDifference * 0.5f,
                voronoiSize,
                voronoiSize
                );
            
            var points = new List<Vector2>(GetRandomPoints(voronoiPointCount, bounds));
            
            // Throw the tree points in for fun
            points.AddRange(_tree.Nodes.Select(n => n.position));

            voronoiRenderer.CreateAndSetMesh(points.ToArray(), bounds, voronoiRelaxation);
        }
    }

    private static Vector2[] GetRandomPoints(int count, Rectf bounds)
    {
        var points = new List<Vector2>();
        for (var i = 0; i < count; i++)
        {
            points.Add(new Vector2(
                Random.Range(bounds.left, bounds.right),
                Random.Range(bounds.bottom, bounds.top))
            );
        }

        return points.ToArray();
    }
}