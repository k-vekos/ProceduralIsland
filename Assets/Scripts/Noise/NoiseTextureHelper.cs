using UnityEngine;

namespace Noise
{
    public static class NoiseTextureHelper
    {
        public static Texture2D PerlinNoise(int resolution, float scale, float frequency, int octaves, float lacunarity, float persistence)
        {
            var texture = new Texture2D(resolution, resolution, TextureFormat.RGB24, true);
            texture.name = "Perlin Noise Texture";
            texture.wrapMode = TextureWrapMode.Clamp;
            texture.filterMode = FilterMode.Trilinear;
            texture.anisoLevel = 9;
            
            var method = NoiseHelper.noiseMethods[(int)NoiseMethodType.Perlin][2];
            
            var point00 = new Vector3(0, 0);
            var point10 = new Vector3(scale, 0);
            var point01 = new Vector3(0, scale);
            var point11 = new Vector3(scale, scale);
        
            var stepSize = 1f / resolution;
            for (var y = 0; y < resolution; y++) 
            {
                var point0 = Vector3.Lerp(point00, point01, (y + 0.5f) * stepSize);
                var point1 = Vector3.Lerp(point10, point11, (y + 0.5f) * stepSize);
                for (var x = 0; x < resolution; x++) 
                {
                    var point = Vector3.Lerp(point0, point1, (x + 0.5f) * stepSize);
                    var sample = NoiseHelper.Sum(method, point, frequency, octaves, lacunarity, persistence);
                    sample = sample * 0.5f + 0.5f;
                
                    texture.SetPixel(x, y, new Color(sample, sample, sample));
                }
            }

            texture.Apply();
            
            return texture;
        }
    }
}