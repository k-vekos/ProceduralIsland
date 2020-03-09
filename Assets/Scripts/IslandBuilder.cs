using System.Collections.Generic;
using System.Linq;
using csDelaunay;
using Map;
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
    public MapRenderer mapRenderer;
    public MapTextureRenderer mapTextureRenderer;

    public void Start()
    {
        BuildIslandAndInitRenderers();
    }
    
    public void BuildIslandAndInitRenderers()
    {
        var tree = RandomTreeBuilder.GetRandomTree(treeNodeCount, treeMaxGrowthLength, size);
        
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
        points.AddRange(tree.Nodes.Select(n => n.position));
        var pointsF = points.Select(v => new Vector2f(v.x, v.y)).ToList();
        var voronoi = new Voronoi(pointsF, bounds, voronoiRelaxation);

        var map = MapHelper.MapFromVoronoi(voronoi);
        MapHelper.CalculateCellTypes(map, tree, treeMaxGrowthLength);
        
        if (treeRenderer != null)
            treeRenderer.SetTree(tree);

        if (voronoiRenderer != null)
            voronoiRenderer.SetVoronoi(voronoi);

        var mapMesh = MapMeshCreator.MeshFromCells(map.Cells);
        
        if (mapRenderer != null)
            mapRenderer.SetMesh(mapMesh);

        if (mapTextureRenderer != null)
            mapTextureRenderer.RenderCellsToTexture(map.Cells.Where(c => c.CellType == CellType.Land).ToArray());

        /*if (mapTextureRenderer != null)
            mapTextureRenderer.RenderMeshToTexture(mapMesh);*/
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