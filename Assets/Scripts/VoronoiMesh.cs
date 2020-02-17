using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class VoronoiMesh : MonoBehaviour
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

    public void SetMesh(List<Vector2> points, float size)
    {
        var helper = new VoronoiHelper();

        var scaledPoints = points.Select(p => p * size).ToList();

        var mesh = helper.MeshFromPoints(scaledPoints, size);

        MeshFilter.mesh = mesh;
    }
}
