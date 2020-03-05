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
            var map = new Map();
            map.Cells = new Cell[voronoi.SitesIndexedByLocation.Count];
            
            foreach (var pair in voronoi.SitesIndexedByLocation)
            {
                var site = pair.Value;
                var region = site.Region(voronoi.PlotBounds);
                
                var cell = new Cell
                {
                    Vertices = region.Select(corner => new Vector2(corner.x, corner.y)).ToList(),
                    NeighborIndexes = site.NeighborSites().Select(s => s.SiteIndex).ToArray()
                };
                
                map.Cells[site.SiteIndex] = cell;
            }

            return map;
        }

        public static void CalculateCellTypes(Map map, Tree tree, float maxTreeNodeDistance)
        {
            var cells = map.Cells;
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