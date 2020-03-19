using System;
using System.Collections.Generic;
using System.Linq;
using csDelaunay;
using UnityEngine;

namespace Maps
{
    public class Map
    {
        public Cell[] Cells { get; set; }
        public Cell[] EdgeCells { get; set; }
        public Dictionary<int, List<Cell>> CellsByVertexIndex { get; set; } = new Dictionary<int, List<Cell>>();
        public Voronoi Voronoi { get; set; }
        public Dictionary<int, Corner> CornersByIndex { get; set; } = new Dictionary<int, Corner>();
        public int MaxElevation { get; private set; } = 0;
        public int MinElevation { get; private set; } = 0;

        private readonly Dictionary<int, List<Corner>> _cornerMap = new Dictionary<int, List<Corner>>();

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

        public float GetNormalizedElevation(float elevation)
        {
            var x = Mathf.InverseLerp(MinElevation, MaxElevation, elevation);
            return x;
        }

        public void SetElevations(int[] coastCellIndexes, int minElevation)
        {
            var visited = new List<int>();
            visited.AddRange(coastCellIndexes);
            
            // <cell index, distance from coast>
            var queue = new Queue<Tuple<int, int>>();
            
            foreach (var i in coastCellIndexes)
            {
                var cell = Cells[i];
                cell.CellType = CellType.Coast;
                cell.Elevation = 0;

                foreach (var n in cell.NeighborIndexes)
                {
                    if (visited.Contains(n))
                        continue;
                    
                    // Away from the island equals negative distance
                    var distance = Cells[n].CellType == CellType.Sea ? -1 : 1;
                    
                    queue.Enqueue(new Tuple<int, int>(n, distance));
                    visited.Add(n);
                }
            }

            while (queue.Count > 0)
            {
                var tuple = queue.Dequeue();
                var cell = Cells[tuple.Item1];
                var distanceFromCoast = tuple.Item2;
                
                // Apply min elevation constraint
                var elevation = Math.Max(minElevation, distanceFromCoast);

                // Record min and max elevations
                if (elevation < MinElevation)
                    MinElevation = elevation;
                if (elevation > MaxElevation)
                    MaxElevation = elevation;

                cell.Elevation = elevation;

                foreach (var n in cell.NeighborIndexes)
                {
                    if (visited.Contains(n))
                        continue;
                    
                    // Away from the island equals negative distance
                    var distance = Cells[n].CellType == CellType.Sea ? -1 : 1;
                    
                    queue.Enqueue(new Tuple<int, int>(n, elevation + distance));
                    visited.Add(n);
                }
            }
            
            Debug.Log($"MinElevation = {MinElevation}; MaxElevation = {MaxElevation}");
        }
    }
}