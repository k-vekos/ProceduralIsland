using System.Collections.Generic;
using System.Linq;
using Noise;
using UnityEngine;

namespace Maps
{
    public static class MapTextureHelper
    {
        private static Material _drawingMaterial;
        
        public static Dictionary<CellType, Color> CellTypeColors = new Dictionary<CellType, Color>
        {
            {CellType.Land, Color.white},
            {CellType.Coast, Color.white},
            {CellType.Water, Color.gray},
            {CellType.Sea, Color.black}
        };

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
                for (var i = 0; i < cell.Corners.Count; i++)
                {
                    var first = cell.Corners[i];
                    var second = cell.Corners[(i + 1) % cell.Corners.Count];
                    var firstElevation = map.GetNormalizedElevation(first.Elevation);
                    var secondElevation = map.GetNormalizedElevation(second.Elevation);
                    
                    GL.Color(ColorFromElevation(firstElevation));
                    GL.Vertex3(first.Position.x, first.Position.y, 0);
                    GL.Color(ColorFromElevation(secondElevation));
                    GL.Vertex3(second.Position.x, second.Position.y, 0);

                    var averageX = cell.Corners.Average(c => c.Position.x);
                    var averageY = cell.Corners.Average(c => c.Position.y);
                    var averageHeight = cell.Corners.Average(c => c.Elevation);
                    averageHeight = map.GetNormalizedElevation(averageHeight);
                    
                    // TODO Move this to somewhere less obscure
                    cell.Elevation = averageHeight;
                    
                    GL.Color(ColorFromElevation(averageHeight));
                    GL.Vertex3(averageX, averageY, 0);
                }
            }
            GL.End();
            
            GL.PopMatrix();

            return CreateTextureFromRenderTexture(textureSize, renderTexture);
        }

        public static Texture2D RenderMapToVoronoiTexture(Map map, int mapSize, int textureSize,
            bool colorByCellElevation = false)
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
                for (var i = 0; i < cell.Corners.Count; i++)
                {
                    var first = cell.Corners[i];
                    var second = cell.Corners[(i + 1) % cell.Corners.Count];
                    var averageX = cell.Corners.Average(c => c.Position.x);
                    var averageY = cell.Corners.Average(c => c.Position.y);

                    GL.Color(colorByCellElevation ? ColorFromElevation(cell.Elevation) : CellTypeColors[cell.CellType]);

                    GL.Vertex3(first.Position.x, first.Position.y, 0);
                    GL.Vertex3(second.Position.x, second.Position.y, 0);
                    GL.Vertex3(averageX, averageY, 0);
                }
            }
            GL.End();
            
            GL.PopMatrix();

            return CreateTextureFromRenderTexture(textureSize, renderTexture);
        }

        public static void ApplyNoiseToMapTexture(Texture2D mapTexture, PerlinNoiseSettings settings,
            float noiseOffsetFactor)
        {
            var noiseTexture =
                NoiseTextureHelper.PerlinNoise(mapTexture.width, settings);
            
            var mapColors = mapTexture.GetPixels();
            var noiseColors = noiseTexture.GetPixels();

            Debug.Assert(mapColors.Length == noiseColors.Length,
                "Cannot use textures with different resolutions!");
            
            for (var i = 0; i < mapColors.Length; ++i)
            {
                mapColors[i].r -= noiseColors[i].r * noiseOffsetFactor;
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