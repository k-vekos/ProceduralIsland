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

    private static Mesh LineMeshFromVoronoi(Voronoi voronoi)
    {
        var vertices = GetVerticesFromEdges(voronoi.Edges);
        var indices = Enumerable.Range(0, vertices.Length).ToArray();
        
        var colors = indices.Select(x => Color.black).ToArray();

        var mesh = new Mesh {
            vertices = vertices,
            colors = colors
        };
        
        mesh.SetIndices(indices, MeshTopology.Lines, 0, true);

        return mesh;
    }
    
    public static Mesh MeshFromPointsTriangleNet(List<Vector2> points)
    {
        var polygon = new TriangleNetGeo.Polygon();
        for (var i = 0; i < points.Count; i++) {
            polygon.Add(new TriangleNetGeo.Vertex(points[i].x, points[i].y));
        }
        
        // ConformingDelaunay is false by default; this leads to ugly long polygons at the edges
        // because the algorithm will try to keep the mesh convex
        var options = new TriangleNet.Meshing.ConstraintOptions
        {
            ConformingDelaunay = true
        };

        var triNetMesh = (TriangleNet.Mesh) polygon.Triangulate(options);
        
        var voronoi = new TriangleNetVoronoi.BoundedVoronoi(triNetMesh);
        voronoi.ResolveBoundaryEdges();
        var voronoiVerts = voronoi.Vertices.Select(v => new Vector3((float) v.x, 0, (float) v.y)).ToArray();
        var voronoiEdges = voronoi.Edges.SelectMany(e => new[] {e.P0, e.P1}).ToArray();

        var uvs = triNetMesh.Vertices.Select(v => new Vector2((float) v.x, (float) v.y)).ToArray();
        //var triangles = triNetMesh.Triangles.SelectMany(t => t.vertices.Select(v => v.id).Reverse()).ToArray();
        //var edges = triNetMesh.Edges.SelectMany(e => new[] {e.P0, e.P1}).ToArray();

        var mesh = new Mesh {
            vertices = triNetMesh.Vertices.Select(v => new Vector3((float) v.x, Random.Range(0f, 1f), (float) v.y)).ToArray(),
            uv = uvs,
            triangles = triNetMesh.Triangles.SelectMany(t => t.vertices.Select(v => v.id).Reverse()).ToArray()
        };
        mesh.RecalculateNormals();

        return mesh;
    }
    
    public static Mesh MeshFromPointsDelaunator(List<Vector2> points)
    {
        var delPoints =
            points.Select(p => new DelaunatorSharp.Models.Point(p.x, p.y) as DelaunatorSharp.Interfaces.IPoint);
        var delaunator = new DelaunatorSharp.Delaunator(delPoints);
        
        var vertices = new List<Vector3>();
        delaunator.ForEachVoronoiEdge(edge =>
        {
            vertices.Add(new Vector3((float) edge.P.X, 0, (float) edge.P.Y));
            vertices.Add(new Vector3((float) edge.Q.X, 0, (float) edge.Q.Y));
        });
        
        var indices = Enumerable.Range(0, vertices.Count).ToArray();

        var mesh = new Mesh {
            vertices = vertices.ToArray()
        };
        
        mesh.SetIndices(indices, MeshTopology.Lines, 0, true);

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
