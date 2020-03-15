using System.Linq;
using csDelaunay;
using Noise;
using UnityEngine;

namespace Map
{
    public static class MapTextureHelper
    {
        private static Material _drawingMaterial;

        private static void CreateDrawingMaterial()
        {
            if (!_drawingMaterial)
            {
                // Unity has a built-in shader that is useful for drawing
                // simple colored things.
                Shader shader = Shader.Find("Hidden/Internal-Colored");
                _drawingMaterial = new Material(shader);
                _drawingMaterial.hideFlags = HideFlags.HideAndDontSave;
                // Turn on alpha blending
                _drawingMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                _drawingMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                // Turn backface culling off
                _drawingMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
                // Turn off depth writes
                _drawingMaterial.SetInt("_ZWrite", 0);
            }
        }
        
        private static RenderTexture CreateRenderTexture(int textureSize, Color color)
        {
            // get a temporary RenderTexture //
            RenderTexture renderTexture =
                RenderTexture.GetTemporary(textureSize, textureSize, 0, RenderTextureFormat.RFloat);

            // set the RenderTexture as global target (that means GL too)
            RenderTexture.active = renderTexture;

            // clear GL //
            GL.Clear(false, true, color);
            GL.sRGBWrite = false;
        
            return renderTexture;
        }

        private static Color ColorFromElevation(float elevation)
        {
            return new Color(elevation, elevation, elevation);
        }

        public static Texture2D RenderMapToTexture(Map map, int mapSize, int textureSize)
        {
            CreateDrawingMaterial();
            
            var renderTexture = CreateRenderTexture(textureSize, Color.black);
            
            _drawingMaterial.SetPass(0);
            GL.PushMatrix();
            GL.LoadPixelMatrix(0, mapSize, 0, mapSize);
            GL.Viewport(new Rect(0, 0, textureSize, textureSize));
            
            GL.Begin(GL.TRIANGLES);
            foreach (var cell in map.Cells)
            {
                //var randomCellColor = new Color(Random.value, Random.value, Random.value);
                var randomCellColor = Color.white;

                for (var i = 0; i < cell.Corners.Count; i++)
                {
                    var first = cell.Corners[i];
                    var second = cell.Corners[(i + 1) % cell.Corners.Count];
                    
                    GL.Color(ColorFromElevation(first.Elevation) * randomCellColor);
                    GL.Vertex3(first.Position.x, first.Position.y, 0);
                    GL.Color(ColorFromElevation(second.Elevation) * randomCellColor);
                    GL.Vertex3(second.Position.x, second.Position.y, 0);


                    var averageX = cell.Corners.Average(c => c.Position.x);
                    var averageY = cell.Corners.Average(c => c.Position.y);
                    var averageHeight = cell.Corners.Average(c => c.Elevation);
                    GL.Color(ColorFromElevation(averageHeight) * randomCellColor);
                    GL.Vertex3(averageX, averageY, 0);
                }
            }
            GL.End();
            
            GL.PopMatrix();

            return CreateTextureFromRenderTexture(textureSize, renderTexture);
        }

        public static void ApplyNoiseToMapTexture(Texture2D mapTexture, float scale = 6f, float frequency = 1f, int octaves = 6,
            float lacunarity = 2, float persistence = 0.5f)
        {
            var noiseTexture =
                NoiseTextureHelper.PerlinNoise(mapTexture.width, scale, frequency, octaves, lacunarity, persistence);
            
            var mapColors = mapTexture.GetPixels();
            var noiseColors = noiseTexture.GetPixels();

            Debug.Assert(mapColors.Length == noiseColors.Length, "Cannot use textures with different resolutions!");
            
            for (var i = 0; i < mapColors.Length; ++i)
            {
                mapColors[i] *= noiseColors[i];
            }
            mapTexture.SetPixels(mapColors);
            mapTexture.Apply();
        }
        
        private static Texture2D CreateTextureFromRenderTexture(int textureSize, RenderTexture renderTexture)
        {
            // read the active RenderTexture into a new Texture2D //
            Texture2D newTexture = new Texture2D(textureSize, textureSize, TextureFormat.RFloat, 0, true);
            newTexture.ReadPixels(new Rect(0, 0, textureSize, textureSize), 0, 0);

            // apply pixels and compress //
            bool applyMipsmaps = false;
            newTexture.Apply(applyMipsmaps);
            //bool highQuality = true;
            //newTexture.Compress(highQuality);

            // clean up
            RenderTexture.active = null;
            RenderTexture.ReleaseTemporary(renderTexture);
            
            return newTexture;
        }
    }
}