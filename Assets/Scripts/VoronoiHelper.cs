using System.Collections.Generic;
using System.Linq;
using csDelaunay;
using UnityEngine;

public class VoronoiHelper
{
    private int[] _indices;
    private Vector3[] _verts;
    
    public Mesh MeshFromPoints(List<Vector2> points, float size, int relax = 0)
    {
        var pointsF = points.Select(v => new Vector2f(v.x, v.y)).ToList();
        
        var bounds = new Rectf(0, 0, size, size);

        var voronoi = new Voronoi(pointsF, bounds);

        if (relax > 0)
        {
            voronoi.LloydRelaxation(relax);
        }

        IndicesFromEdges(voronoi.Edges);

        var colors = _indices.Select(x => Color.black).ToArray();

        var mesh = new Mesh {
            vertices = _verts,
            colors = colors
        };
        
        mesh.SetIndices(_indices, MeshTopology.Lines, 0, true);

        return mesh;
    }

    private void IndicesFromEdges(List<Edge> edges)
    {
        var verts = new List<Vector3>();
        var index = 0;
        foreach (var edge in edges)
        {
            // if the edge doesn't have clippedEnds, if was not within the bounds, dont draw it
            if (edge.ClippedEnds == null) continue;
            
            // Add each edge point as entirely new vertex
            var left = edge.ClippedEnds[LR.LEFT];
            var right = edge.ClippedEnds[LR.RIGHT];
            verts.Add(new Vector3(left.x, 0, left.y));
            verts.Add(new Vector3(right.x, 0, right.y));
        }
        
        _indices = Enumerable.Range(0, verts.Count).ToArray();
        _verts = verts.ToArray();
    }
}
