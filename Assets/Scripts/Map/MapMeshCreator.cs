using System.Collections.Generic;
using System.Linq;
using TriangleNet.Geometry;
using UnityEngine;

namespace Map
{
    public static class MapMeshCreator
    {
        /*public static Dictionary<CellType, Color> CellTypeColors = new Dictionary<CellType, Color>
        {
            {CellType.Land, Color.green},
            {CellType.Water, new Color(0, 0.64f, 0.91f)},
            {CellType.Sea, Color.blue}
        };*/
        
        public static Dictionary<CellType, Color> CellTypeColors = new Dictionary<CellType, Color>
        {
            {CellType.Land, Color.white},
            {CellType.Water, Color.black},
            {CellType.Sea, Color.black}
        };
        
        /*public static Mesh MeshFromCells(Cell[] cells)
        {
            var options = new TriangleNet.Meshing.ConstraintOptions
            {
                ConformingDelaunay = true
            };
            
            var vertices = new List<Vector3>();
            var verticesIndex = 0;
            var triangles = new List<int>();
            var colors = new List<Color>();
            
            foreach (var cell in cells)
            {
                var polygon = new Polygon();
                foreach (var vertex in cell.Vertices)
                {
                    polygon.Add(new Vertex(vertex.x, vertex.y));
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
            
                // Assign same color to all vertices in cell
                var regionColor = ColorFromCellType(cell.CellType);
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
        }*/

        private static Color ColorFromCellType(CellType cellType)
        {
            return CellTypeColors[cellType];
        }
    }
}