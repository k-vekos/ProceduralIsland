using System;
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
            var map = new Map
            {
                Cells = new Cell[voronoi.SitesIndexedByLocation.Count],
                CellsByVertexIndex = new Dictionary<int, List<Cell>>(),
                Voronoi = voronoi
            };

            var edgeCells = new List<Cell>();
            foreach (var pair in voronoi.SitesIndexedByLocation)
            {
                var site = pair.Value;
                var region = site.Region(voronoi.PlotBounds);
                var isEdgeCell = site.Edges.Any(e => e.IsPartOfConvexHull());
                
                var cell = new Cell
                {
                    NeighborIndexes = site.NeighborSites().Select(s => s.SiteIndex).ToArray(),
                    Index = site.SiteIndex,
                    Edges = site.Edges
                };

                /* "The Voronoi library generates multiple Point objects for corners, and we need to canonicalize to one
                Corner object. To make lookup fast, we keep an array of Points, bucketed by x value, and then we only
                have to look at other Points in nearby buckets. When we fail to find one, we'll create a new
                Corner object." */
                var corners = 
                    region.Select(point => map.CreateCornerForCell(point.x, point.y, cell));
                cell.Corners = corners.ToList();

                map.Cells[site.SiteIndex] = cell;

                if (isEdgeCell)
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
                foreach (var corner in cell.Corners)
                {
                    var closestNode = tree.GetClosestNode(corner.Position);
                    var distance = Vector2.Distance(closestNode.position, corner.Position);
                    
                    if (distance > maxTreeNodeDistance)
                    {
                        cell.CellType = CellType.Water;
                        break;
                    }
                }
            }
            
            // Flood fill from some edge cell to find Sea cells and get back coast cells
            var coastCellIndexes = FloodFillCellType(map, map.EdgeCells[0].Index, CellType.Sea);

            // Set cell elevations
            SetElevations(map, coastCellIndexes);
        }

        private static void SetElevations(Map map, int[] coastCellIndexes, float elevationStepSize = 0.1f)
        {
            var visited = new List<int>();
            visited.AddRange(coastCellIndexes);
            
            // <cell index, distance from coast>
            var queue = new Queue<Tuple<int, int>>();
            
            foreach (var i in coastCellIndexes)
            {
                var cell = map.Cells[i];
                cell.CellType = CellType.Coast;
                cell.Elevation = elevationStepSize;

                foreach (var n in cell.NeighborIndexes)
                {
                    if (map.Cells[n].CellType == CellType.Sea || map.Cells[n].CellType == CellType.Water)
                        continue;

                    if (!visited.Contains(n))
                    {
                        queue.Enqueue(new Tuple<int, int>(n, 1));
                        visited.Add(n);
                    }
                }
            }

            while (queue.Count > 0)
            {
                var tuple = queue.Dequeue();
                var cell = map.Cells[tuple.Item1];
                var distanceFromCoast = tuple.Item2;

                cell.Elevation = (distanceFromCoast + 1) * elevationStepSize;

                foreach (var n in cell.NeighborIndexes)
                {
                    if (visited.Contains(n))
                        continue;
                    if (map.Cells[n].CellType == CellType.Sea || map.Cells[n].CellType == CellType.Water)
                        continue;
                    
                    queue.Enqueue(new Tuple<int, int>(n, distanceFromCoast + 1));
                    visited.Add(n);
                }
            }
        }

        private static int[] FloodFillCellType(Map map, int cellIndex, CellType cellType)
        {
            var visited = new List<int>();
            visited.Add(cellIndex);
            
            var queue = new Queue<int>();

            var borderCellIndexes = new List<int>();

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
                    if (visited.Contains(n) || queue.Contains(n))
                        continue;
                    if (map.Cells[n].CellType == replaceType)
                        queue.Enqueue(n);
                    else if (!borderCellIndexes.Contains(n))
                        borderCellIndexes.Add(n);
                }
                
                visited.Add(index);
            }

            return borderCellIndexes.ToArray();
        }
    }
}