using System.Collections.Generic;
using System.Linq;
using csDelaunay;
using TriangleNetGeo = TriangleNet.Geometry;
using TriangleNetVoronoi = TriangleNet.Voronoi;
using static TriangleNet.Geometry.ExtensionMethods;
using UnityEngine;

public static class VoronoiMeshCreator
{
    public static Mesh MeshFromVoronoi(Voronoi voronoi)
    {
        var options = new TriangleNet.Meshing.ConstraintOptions
        {
            ConformingDelaunay = true
        };

        var vertices = new List<Vector3>();
        var verticesIndex = 0;
        var triangles = new List<int>();
        var colors = new List<Color>();

        var regions = voronoi.Regions();
        for (var i = 0; i < regions.Count; i++)
        {
            var region = regions[i];
            
            var polygon = new TriangleNetGeo.Polygon();
            foreach (var corner in region)
            {
                polygon.Add(new TriangleNetGeo.Vertex(corner.x, corner.y));
            }
            
            var cellMesh = (TriangleNet.Mesh) polygon.Triangulate(options);

            vertices.AddRange(
                cellMesh.Vertices.Select(v => new Vector3((float) v.x, 0, (float) v.y))
            );

            triangles.AddRange(
                cellMesh.Triangles.SelectMany(
                    
                    t => t.vertices.Select(v => v.id + verticesIndex)
                        .Reverse() // Reverse triangles so they're facing the right way
                )
            );
            
            // Update index so the next batch of triangles point to the correct vertices
            verticesIndex = vertices.Count;
            
            // Assign same color to all vertices in region
            var regionColor = new Color(Random.Range(0, 1f), Random.Range(0, 1f), Random.Range(0, 1f));
            colors.AddRange(cellMesh.Vertices.Select(v => regionColor));
        }

        // Just make world-space UVs for now
        var uvs = vertices.Select(v => new Vector2(v.x, v.y));

        var mesh = new Mesh {
            vertices = vertices.ToArray(),
            colors = colors.ToArray(),
            uv = uvs.ToArray(),
            triangles = triangles.ToArray()
        };
        mesh.RecalculateNormals();

        return mesh;
    }

    private static Vector3[] GetVerticesFromEdges(List<Edge> edges)
    {
        var vertices = new List<Vector3>();
        foreach (var edge in edges)
        {
            // if the edge doesn't have clippedEnds, if was not within the bounds, dont draw it
            if (edge.ClippedEnds == null) continue;
            
            // Add each edge point as entirely new vertex
            var left = edge.ClippedEnds[LR.LEFT];
            var right = edge.ClippedEnds[LR.RIGHT];
            vertices.Add(new Vector3(left.x, 0, left.y));
            vertices.Add(new Vector3(right.x, 0, right.y));
        }

        return vertices.ToArray();
    }
}
