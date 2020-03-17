﻿using UnityEngine;

namespace Maps
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
        
        public void SetMesh(Mesh mesh)
        {
            MeshFilter.mesh = mesh;
        }
    }
}