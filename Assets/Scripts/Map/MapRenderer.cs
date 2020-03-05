using csDelaunay;
using UnityEngine;

namespace Map
{
    [RequireComponent(typeof(MeshFilter))]
    public class MapRenderer : MonoBehaviour
    {
        private MeshFilter _meshFilter;

        private MeshFilter MeshFilter
        {
            get
            {
                if (_meshFilter == null)
                    _meshFilter = GetComponent<MeshFilter>();

                return _meshFilter;
            }
        }
        
        public void SetMap(Map map)
        {
            var mesh = MapMeshCreator.MeshFromCells(map.Cells);
            MeshFilter.mesh = mesh;
        }
    }
}