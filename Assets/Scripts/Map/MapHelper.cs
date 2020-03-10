using System.Collections.Generic;
using System.Linq;
using csDelaunay;
using UnityEngine;

namespace Map
{
    public static class MapHelper
    {
        public static Map MapFromVoronoi(Voronoi voronoi)
        {
            var map = new Map();
            map.Cells = new Cell[voronoi.SitesIndexedByLocation.Count];

            var edgeCells = new List<Cell>();
            foreach (var pair in voronoi.SitesIndexedByLocation)
            {
                var site = pair.Value;
                var region = site.Region(voronoi.PlotBounds);

                var cell = new Cell
                {
                    Vertices = region.Select(corner => new Vector2(corner.x, corner.y)).ToList(),
                    NeighborIndexes = site.NeighborSites().Select(s => s.SiteIndex).ToArray(),
                    Index = site.SiteIndex
                };
                
                map.Cells[site.SiteIndex] = cell;

                if (site.Edges.Any(e => e.IsPartOfConvexHull()))
                    edgeCells.Add(cell);
            }

            map.EdgeCells = edgeCells.ToArray();

            return map;
        }

        public static void CalculateCellTypes(Map map, Tree tree, float maxTreeNodeDistance)
        {
            var cells = map.Cells;
            
            // First, find Land cells
            foreach (var cell in cells)
            {
                cell.CellType = CellType.Land;
                
                // TODO Also mark as land if there is a node inside the area of the cell
                // Set as land if all vertices within range, else set water
                foreach (var vertex in cell.Vertices)
                {
                    var closestNode = tree.GetClosestNode(vertex);
                    var distance = Vector2.Distance(closestNode.position, vertex);
                    
                    if (distance > maxTreeNodeDistance)
                    {
                        cell.CellType = CellType.Water;
                        break;
                    }
                }
            }
            
            // Second, flood fill from some edge cell to find Sea cells
            FloodFillCellType(map, map.EdgeCells[0].Index, CellType.Sea);
        }

        private static void FloodFillCellType(Map map, int cellIndex, CellType cellType)
        {
            var visited = new List<int>();
            visited.Add(cellIndex);
            
            var queue = new Queue<int>();

            var startCell = map.Cells[cellIndex];
            var replaceType = startCell.CellType;
            startCell.CellType = cellType;

            foreach (var n in startCell.NeighborIndexes)
            {
                queue.Enqueue(n);
            }

            while (queue.Count > 0)
            {
                var index = queue.Dequeue();
                var cell = map.Cells[index];
                
                cell.CellType = cellType;

                foreach (var n in cell.NeighborIndexes)
                {
                    if (!visited.Contains(n) && !queue.Contains(n) && map.Cells[n].CellType == replaceType)
                        queue.Enqueue(n);
                }
                
                visited.Add(index);
            }
        }
    }
}