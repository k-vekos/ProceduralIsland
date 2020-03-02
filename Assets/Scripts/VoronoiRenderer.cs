using System.Collections.Generic;
using csDelaunay;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class VoronoiRenderer : MonoBehaviour
{
    private MeshFilter _meshFilter;

    private MeshFilter MeshFilter
    {
        get
        {
            if (_meshFilter == null)
                _meshFilter = GetComponent<MeshFilter>();

            return _meshFilter;
        }
    }
    
    public void SetVoronoi(Voronoi voronoi)
    {
        var mesh = VoronoiMeshCreator.MeshFromVoronoi(voronoi);
        //var mesh = VoronoiMeshCreator.MeshFromPointsTriangleNet(points);
        //var mesh = VoronoiMeshCreator.MeshFromPointsDelaunator(points);
        MeshFilter.mesh = mesh;
    }
}