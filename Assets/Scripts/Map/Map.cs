using System.Collections.Generic;

namespace Map
{
    public class Map
    {
        public Cell[] Cells { get; set; }
        public Cell[] EdgeCells { get; set; }
        //public Dictionary<CellType, List<Cell>> CellsIndexedByCellType { get; set; }
    }
}