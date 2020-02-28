using System.Collections.Generic;
using System.Linq;
using DelaunatorSharp;
using DelaunatorSharp.Interfaces;
using TriangleNet.Geometry;
using TriangleNet.Topology;
using TriangleNet.Voronoi;
using UnityEngine;
using Point = DelaunatorSharp.Models.Point;

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

    public void SetMesh2(List<Vector2> points, float size, int relax = 0)
    {
        // Scale the points
        var scaledPoints = points.Select(p => p * size).ToList();
        
        Polygon polygon = new Polygon();
        for (var i = 0; i < scaledPoints.Count; i++) {
            polygon.Add(new Vertex(scaledPoints[i].x, scaledPoints[i].y));
        }
        
        // ConformingDelaunay is false by default; this leads to ugly long polygons at the edges
        // because the algorithm will try to keep the mesh convex
        var options = new TriangleNet.Meshing.ConstraintOptions
        {
            ConformingDelaunay = true
        };
        
        var triNetMesh = (TriangleNet.Mesh) polygon.Triangulate(options);
        
        //var voronoi = new BoundedVoronoi(triNetMesh);
        var voronoi = new BoundedVoronoi(triNetMesh);
        voronoi.ResolveBoundaryEdges();
        var voronoiVerts = voronoi.Vertices.Select(v => new Vector3((float) v.x, 0, (float) v.y)).ToArray();
        var voronoiEdges = voronoi.Edges.SelectMany(e => new[] {e.P0, e.P1}).ToArray();
        
        //voronoi.Faces.Select(f => f.)
        
        //var colors = indices.Select(x => Color.black).ToArray();

        var uvs = triNetMesh.Vertices.Select(v => new Vector2((float) v.x, (float) v.y)).ToArray();
        var triangles = triNetMesh.Triangles.SelectMany(t => t.vertices.Select(v => v.id).Reverse()).ToArray();
        var edges = triNetMesh.Edges.SelectMany(e => new[] {e.P0, e.P1}).ToArray();

        /*var mesh = new Mesh {
            vertices = triNetMesh.Vertices.Select(v => new Vector3((float) v.x, Random.Range(0f, 1f), (float) v.y)).ToArray(),
            uv = uvs,
            triangles = triNetMesh.Triangles.SelectMany(t => t.vertices.Select(v => v.id).Reverse()).ToArray()
        };*/
        //mesh.RecalculateNormals();

        var voronoiMesh = new Mesh
        {
            vertices = voronoiVerts
        };
        
        voronoiMesh.SetIndices(voronoiEdges, MeshTopology.Lines, 0, true);
        
        

        //mesh.SetIndices(indices, MeshTopology.Lines, 0, true);*/

        MeshFilter.mesh = voronoiMesh;
    }

    public void SetMesh3(List<Vector2> points, float size, int relax = 0)
    {
        var p = points.Select(x => new Point(x.x, x.y) as IPoint);
        var delaunator = new Delaunator(p);

        var verts = new List<Vector2>();
        
        delaunator.ForEachVoronoiCell(cell =>
        {
            var v = cell.Points.Select(point => new Vector2((float) point.X, (float) point.Y));
            verts.AddRange(v);
        });

        var vertices = new List<Vector3>();
        var indices = new List<int>();
        Triangulation.triangulate(verts, new List<List<Vector2>>(), 0f, out indices, out vertices);
        
        var colors = indices.Select(x => Color.black).ToArray();

        var mesh = new Mesh {
            vertices = vertices.ToArray(),
            colors = colors
        };
        
        mesh.SetIndices(indices, MeshTopology.Lines, 0, true);

        MeshFilter.mesh = mesh;
    }

    public void SetMesh(List<Vector2> points, float size, int relax = 0)
    {
        var scaledPoints = points.Select(p => p * size).ToList();
        
        var helper = new VoronoiHelper();
        var mesh = helper.MeshFromPoints(scaledPoints, size, relax);

        MeshFilter.mesh = mesh;
    }
}
