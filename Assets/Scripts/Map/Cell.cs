using System.Collections.Generic;
using csDelaunay;
using UnityEngine;

namespace Map
{
    public class Cell
    {
        public List<Vector3> Vertices { get; set; }
        public List<Edge> Edges { get; set; }
        public int[] VertexIndices { get; set; }
        public CellType CellType { get; set; }
        public int[] NeighborIndexes { get; set; }
        public int Index { get; set; }
        public float Elevation { get; set; }
        public Vector3 CenterPoint { get; set; }
    }
}
        
