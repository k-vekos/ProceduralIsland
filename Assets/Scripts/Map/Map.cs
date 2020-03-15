using System.Collections.Generic;
using System.Linq;
using csDelaunay;
using UnityEngine;

namespace Map
{
    public class Map
    {
        public Cell[] Cells { get; set; }
        public Cell[] EdgeCells { get; set; }
        public Dictionary<int, List<Cell>> CellsByVertexIndex { get; set; } = new Dictionary<int, List<Cell>>();
        public Voronoi Voronoi { get; set; }
        public Dictionary<int, Corner> CornersByIndex { get; set; } = new Dictionary<int, Corner>();

        private new readonly Dictionary<int, List<Corner>> _cornerMap = new Dictionary<int, List<Corner>>();

        public Corner CreateCornerForCell(float x, float y, Cell cell)
        {
            var corner = CreateCorner(x, y);
            
            if (!CellsByVertexIndex.ContainsKey(corner.Index))
                CellsByVertexIndex[corner.Index] = new List<Cell>();
            CellsByVertexIndex[corner.Index].Add(cell);

            return corner;
        }
        
        private Corner CreateCorner(float x, float y)
        {
            // Try to find if this corner already exists
            for (var b = (int)x - 1; b <= (int)x + 1; b++)
            {
                if (!_cornerMap.ContainsKey(b))
                    continue;
                
                foreach (var c in _cornerMap[b])
                {
                    var dx = x - c.Position.x;
                    var dy = y - c.Position.y;
                    
                    if (dx*dx + dy*dy < 1e-6)
                        return c;
                }
            }
            
            // Otherwise create it
            var bucket = (int) x;
            
            if (!_cornerMap.ContainsKey(bucket))
                _cornerMap[bucket] = new List<Corner>();
            
            var q = new Corner();
            
            q.Index = CornersByIndex.Count;
            CornersByIndex.Add(q.Index, q);
            
            q.Position = new Vector2(x, y);
            //q.border = (point.x == 0 || point.x == SIZE || point.y == 0 || point.y == SIZE);
            //q.touches = new Vector.<Center>();
            //q.protrudes = new Vector.<Edge>();
            //q.adjacent = new Vector.<Corner>();
            
            _cornerMap[bucket].Add(q);
            
            return q;
        }

        public void SetCornerElevations()
        {
            foreach (var corner in CornersByIndex.Values)
            {
                corner.Elevation = CellsByVertexIndex[corner.Index].Average(c => c.Elevation);
            }
        }
    }
}