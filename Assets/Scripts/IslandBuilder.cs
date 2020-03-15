using System.Collections.Generic;
using System.Linq;
using csDelaunay;
using Map;
using UnityEngine;
using Random = UnityEngine.Random;

public class IslandBuilder : MonoBehaviour
{
    public int size = 100;
    public int treeNodeCount = 50;
    public float treeMaxGrowthLength = 10f;
    public Rect treeBounds;
    public TreeRenderer treeRenderer;
    public int voronoiPointCount = 100;
    public int voronoiRelaxation = 0;
    public VoronoiRenderer voronoiRenderer;
    public MapRenderer mapRenderer;
    public int mapTextureSize = 512;
    public MeshRenderer mapTexturePreviewRenderer;
    public Terrain targetTerrain;
    public float terrainHeightScale = 10f;

    public void Start()
    {
        BuildIslandAndInitRenderers();
    }
    
    public void BuildIslandAndInitRenderers()
    {
        var tree = RandomTreeBuilder.GetRandomTree(treeNodeCount, treeMaxGrowthLength, treeBounds);
        
        var voronoiBounds = new Rectf(
            0,
            0,
            size,
            size
        );
        var points = new List<Vector2>(GetRandomPoints(voronoiPointCount, voronoiBounds));
        // Throw the tree points in for fun
        points.AddRange(tree.Nodes.Select(n => n.position));
        var pointsF = points.Select(v => new Vector2f(v.x, v.y)).ToList();
        var voronoi = new Voronoi(pointsF, voronoiBounds, voronoiRelaxation);

        var map = MapHelper.MapFromVoronoi(voronoi);
        MapHelper.CalculateCellTypes(map, tree, treeMaxGrowthLength);
        map.SetVerticesHeights();
        
        if (treeRenderer != null)
            treeRenderer.SetTree(tree);

        if (voronoiRenderer != null)
            voronoiRenderer.SetVoronoi(voronoi);

        /*if (mapRenderer != null)
        {
            var mapMesh = MapMeshCreator.MeshFromCells(map.Cells);
            mapRenderer.SetMesh(mapMesh);
        }*/
        
        if (mapTexturePreviewRenderer != null)
        {
            var texture =
                MapTextureHelper.RenderCellsToTexture(
                    map.Cells.Where(c => c.CellType == CellType.Land || c.CellType == CellType.Coast).ToArray(),
                    map, size, mapTextureSize);
            
            //MapTextureHelper.ApplyNoiseToMapTexture(texture);
            
            mapTexturePreviewRenderer.material.mainTexture = texture;
            
            if (targetTerrain != null)
            {
                var pixelValues = texture.GetPixels().Select(c => c.r * terrainHeightScale).ToArray();
                var heightArray = new float[mapTextureSize, mapTextureSize];
                for(var i = 0; i < mapTextureSize; i++)
                    for (var j = 0; j < mapTextureSize; j++)
                        heightArray[j, i] = pixelValues[j * mapTextureSize + i]; 
                
                targetTerrain.terrainData.SetHeights(0, 0, heightArray);
            }
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