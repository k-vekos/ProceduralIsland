using System.Collections.Generic;
using System.Linq;
using csDelaunay;
using Map;
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
        map.SetCornerElevations();
        
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
                MapTextureHelper.RenderMapToTexture(map, size, mapTextureSize);

            for (int i = 0; i < 15; i++)
            {
                //BlurMapTexture(texture);
            }
            
            //MapTextureHelper.ApplyNoiseToMapTexture(texture);

            mapTexturePreviewRenderer.material.mainTexture = texture;
            
            if (targetTerrain != null)
            {
                var terrainResolution = targetTerrain.terrainData.heightmapResolution;
                var heightArray = new float[terrainResolution, terrainResolution];
                var stepSize = 1f / terrainResolution;
                
                for (int i = 0; i < terrainResolution; i++)
                {
                    for (int j = 0; j < terrainResolution; j++)
                    {
                        /*var stepSize = 1f / resolution;
                        for (var y = 0; y < resolution; y++) 
                        {
                            var point0 = Vector3.Lerp(point00, point01, (y + 0.5f) * stepSize);
                            var point1 = Vector3.Lerp(point10, point11, (y + 0.5f) * stepSize);
                            for (var x = 0; x < resolution; x++) 
                            {
                                var point = Vector3.Lerp(point0, point1, (x + 0.5f) * stepSize);*/
                        
                        //var u = Mathf.Lerp(0, texture.width, (float) i / terrainResolution);
                        //var v = Mathf.Lerp(0, texture.width, (float) j / terrainResolution);

                        var u = (i + 0.5f) * stepSize;
                        var v = (j + 0.5f) * stepSize;
                        
                        heightArray[i, j] = texture.GetPixelBilinear(u, v).r;
                    }
                }
                
                targetTerrain.terrainData.SetHeights(0, 0, heightArray);
            }
        }        
    }

    private static void BlurMapTexture(Texture2D texture)
    {
        if (texture.format != TextureFormat.RGBA32)
            throw new UnityException($"Wrong format. Texture is not {TextureFormat.RGB24} but {texture.format}");
        
        var job = new BlurRGBA32Job(texture.GetRawTextureData<RGBA32>(), texture.width);
        var rawData = texture.GetRawTextureData<RGBA32>();
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