using System.Collections.Generic;
using System.Linq;
using csDelaunay;
using UnityEditor.UI;

namespace Map
{
    public class Map
    {
        public Cell[] Cells { get; set; }
        public Cell[] EdgeCells { get; set; }
        public Dictionary<int, List<Cell>> CellsByVertexIndex { get; set; }
        public Voronoi Voronoi { get; set; }
        public Dictionary<int, Vertex> Vertices => Voronoi.Vertices;
        public Dictionary<int, float> VerticesHeightsByIndex { get; set; }

        public void SetVerticesHeights()
        {
            VerticesHeightsByIndex = new Dictionary<int, float>();
            
            foreach (var vertex in Vertices.Values)
            {
                var elevation = CellsByVertexIndex[vertex.VertexIndex].Average(c => c.Elevation);
                VerticesHeightsByIndex[vertex.VertexIndex] = elevation;
            }
        }
    }
}