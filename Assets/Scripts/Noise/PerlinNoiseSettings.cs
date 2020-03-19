using UnityEngine;

namespace Noise
{
    [System.Serializable]
    public class PerlinNoiseSettings
    {
        public float frequency = 1f;
        [Range(1, 8)]
        public int octaves = 6;
        [Range(1f, 4f)]
        public float lacunarity = 2f;
        [Range(0f, 1f)]
        public float persistence = 0.5f;
    }
}