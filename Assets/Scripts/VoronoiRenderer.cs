using System.Collections.Generic;
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
    
    public void CreateAndSetMesh(Vector2[] points, Rectf bounds, int relax = 0)
    {
        var mesh = VoronoiMeshCreator.MeshFromPoints(points, bounds, relax);
        //var mesh = VoronoiMeshCreator.MeshFromPointsTriangleNet(points);
        //var mesh = VoronoiMeshCreator.MeshFromPointsDelaunator(points);
        MeshFilter.mesh = mesh;
    }
}