using System.Collections.Generic;
using UnityEngine;

namespace Map
{
    public class Cell
    {
        public List<Vector2> Vertices { get; set; }
        public CellType CellType { get; set; }
        public int[] NeighborIndexes { get; set; }
    }
}
        
