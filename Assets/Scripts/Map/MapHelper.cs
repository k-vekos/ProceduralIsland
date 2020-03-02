using System;
using System.Collections.Generic;
using System.Linq;
using csDelaunay;
using UnityEngine;

namespace Map
{
    public static class MapHelper
    {
        public static Cell[] CellsFromVoronoi(Voronoi voronoi)
        {
            var regions = voronoi.Regions();
            var cells = regions.Select(region => new Cell
            {
                Vertices = region.Select(corner => new Vector2(corner.x, corner.y)).ToList()
            });
            return cells.ToArray();
        }

        public static void CalculateCellTypes(Cell[] cells, Tree tree, float maxTreeNodeDistance)
        {
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
        }
    }
}