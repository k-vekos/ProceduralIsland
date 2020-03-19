using UnityEngine;

namespace Noise
{
    public static class NoiseTextureHelper
    {
        public static Texture2D PerlinNoise(int resolution, PerlinNoiseSettings settings)
        {
            var texture = new Texture2D(resolution, resolution, TextureFormat.RFloat, false, true);
            texture.name = "Perlin Noise Texture";
            texture.wrapMode = TextureWrapMode.Clamp;
            texture.filterMode = FilterMode.Trilinear;
            texture.anisoLevel = 9;
            
            var method = NoiseHelper.noiseMethods[(int)NoiseMethodType.Perlin][2];
            
            var point00 = new Vector3(0, 0);
            var point10 = new Vector3(1f, 0);
            var point01 = new Vector3(0, 1f);
            var point11 = new Vector3(1f, 1f);
        
            var stepSize = 1f / resolution;
            for (var y = 0; y < resolution; y++) 
            {
                var point0 = Vector3.Lerp(point00, point01, (y + 0.5f) * stepSize);
                var point1 = Vector3.Lerp(point10, point11, (y + 0.5f) * stepSize);
                for (var x = 0; x < resolution; x++) 
                {
                    var point = Vector3.Lerp(point0, point1, (x + 0.5f) * stepSize);
                    var sample = NoiseHelper.Sum(method, point, settings);
                    sample = sample * 0.5f + 0.5f;
                
                    texture.SetPixel(x, y, new Color(sample, sample, sample));
                }
            }

            texture.Apply();
            
            return texture;
        }

        public static void ExpandRange(Texture2D texture)
        {
            var min = 1f;
            var max = 0f;
            
            // Find min and max
            for (var x = 0; x < texture.width; x++)
            {
                for (var y = 0; y < texture.height; y++)
                {
                    var color = texture.GetPixel(x, y);
                    var value = color.r;

                    if (value < min)
                        min = value;
                    if (value > max)
                        max = value;
                }
            }
            
            // Remap texture value range from min to max to min to 1f
            for (var x = 0; x < texture.width; x++)
            {
                for (var y = 0; y < texture.height; y++)
                {
                    var color = texture.GetPixel(x, y);
                    var value = color.r;

                    var normal = Mathf.InverseLerp(min, max, value);
                    value = Mathf.Lerp(min, 1f, normal);
                    texture.SetPixel(x, y, new Color(value, value, value));
                }
            }
            
            texture.Apply();
        }
    }
}