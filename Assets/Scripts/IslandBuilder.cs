using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using csDelaunay;
using Maps;
using UnityEngine;
using Random = UnityEngine.Random;
using Unity.Jobs;

public class IslandBuilder : MonoBehaviour
{
    public int size = 100;
    public int treeNodeCount = 50;
    public float treeMaxGrowthLength = 10f;
    public Rect treeBounds;
    public TreeRenderer treeRenderer;
    public int voronoiPointCount = 100;
    public int voronoiRelaxation = 0;
    public int mapTextureSize = 512;
    public bool applyBlur = true;
    public bool applyNoise = true;
    public MeshRenderer mapTexturePreviewRenderer;
    public int seaFloorElevation = -2;
    public Terrain targetTerrain;

    private Map _map;
    private Texture2D _mapTexture;
    private Tree _tree;
    private Voronoi _voronoi;
    
    public void BuildIslandAndInitRenderers()
    {
        _tree = RandomTreeBuilder.GetRandomTree(treeNodeCount, treeMaxGrowthLength, treeBounds);
        
        var voronoiBounds = new Rectf(
            0,
            0,
            size,
            size
        );
        var points = new List<Vector2>(GetRandomPoints(voronoiPointCount, voronoiBounds));
        // Throw the tree points in for fun
        points.AddRange(_tree.Nodes.Select(n => n.position));
        var pointsF = points.Select(v => new Vector2f(v.x, v.y)).ToList();
        _voronoi = new Voronoi(pointsF, voronoiBounds, voronoiRelaxation);

        _map = MapHelper.MapFromVoronoi(_voronoi);
        MapHelper.CalculateCellTypes(_map, _tree, treeMaxGrowthLength, seaFloorElevation);
        _map.SetCornerElevations();
        
        if (treeRenderer != null)
            treeRenderer.SetTree(_tree);
        
        UpdateMapTexture();      
    }

    public void UpdateMapTexture()
    {
        if (_map == null)
        {
            // Debug.LogWarning($"{MethodBase.GetCurrentMethod().Name}: No map set, aborting.");
            return;
        }
        
        _mapTexture = MapTextureHelper.RenderMapToTexture(_map, size, mapTextureSize);
        
        if (applyBlur)
            for (int i = 0; i < 15; i++)
                BlurMapTexture(_mapTexture);

        if (applyNoise)
            MapTextureHelper.ApplyNoiseToMapTexture(_mapTexture);

        if (mapTexturePreviewRenderer != null)
            mapTexturePreviewRenderer.sharedMaterial.mainTexture = _mapTexture;
        
        if (targetTerrain != null)
        {
            SetTerrainHeights(targetTerrain, _mapTexture);
            SetTerrainAlphamapValues(targetTerrain, _mapTexture);
        }
    } 

    private void SetTerrainAlphamapValues(Terrain terrain, Texture2D heightMap)
    {
        var terrainData = terrain.terrainData;
        var alphaMapResolution = terrainData.alphamapWidth;
        var alphaMap = new float[alphaMapResolution, alphaMapResolution, terrainData.alphamapLayers];
        var alphaStepSize = 1f / terrainData.alphamapWidth;
        //var layerSpacing = 1f / terrainData.alphamapLayers;
        var layerSpacing = 0.1f;

        // For each point on the alphamap...
        for (var y = 0; y < alphaMapResolution; y++)
        {
            for (var x = 0; x < alphaMapResolution; x++)
            {
                var u = (x + 0.5f) * alphaStepSize;
                var v = (y + 0.5f) * alphaStepSize;
                var elevation = heightMap.GetPixelBilinear(u, v).r;
                for (var i = 0; i < terrainData.alphamapLayers; i++)
                {
                    if (i + 1 < terrainData.alphamapLayers)
                    {
                        alphaMap[y, x, i] =
                            elevation >= i * layerSpacing && elevation < (i + 1) * layerSpacing ? 1f : 0f;
                    }
                    else
                    {
                        alphaMap[y, x, i] = elevation >= i * layerSpacing ? 1f : 0f;
                    }
                }
            }
        }

        terrainData.SetAlphamaps(0, 0, alphaMap);
    }

    private static void SetTerrainHeights(Terrain terrain, Texture2D heightMap)
    {
        var terrainData = terrain.terrainData;
        var terrainResolution = terrainData.heightmapResolution;
        var heightArray = new float[terrainResolution, terrainResolution];

        var stepSize = 1f / terrainResolution;
        for (var i = 0; i < terrainResolution; i++)
        {
            for (var j = 0; j < terrainResolution; j++)
            {
                var u = (i + 0.5f) * stepSize;
                var v = (j + 0.5f) * stepSize;

                heightArray[j, i] = heightMap.GetPixelBilinear(u, v).r;
            }
        }

        terrainData.SetHeights(0, 0, heightArray);
    }

    private static void BlurMapTexture(Texture2D texture)
    {
        if (texture.format != TextureFormat.RFloat)
            throw new UnityException($"Wrong format. Texture is not {TextureFormat.RGB24} but {texture.format}");
        
        var job = new BlurRFloatJob(texture.GetRawTextureData<RFloat>(), texture.width);
        var rawData = texture.GetRawTextureData<RFloat>();
        job.Schedule(rawData.Length, texture.width).Complete();
        
        texture.Apply();
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