﻿using System.Linq;
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
            RenderTexture renderTexture = RenderTexture.GetTemporary(textureSize, textureSize);

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

        public static Texture2D RenderCellsToTexture(Cell[] cells, Map map, int mapSize, int textureSize, bool useVertices = false)
        {
            CreateDrawingMaterial();
            
            var renderTexture = CreateRenderTexture(textureSize, Color.black);
            
            _drawingMaterial.SetPass(0);
            GL.PushMatrix();
            GL.LoadPixelMatrix(0, mapSize, 0, mapSize);
            GL.Viewport(new Rect(0, 0, textureSize, textureSize));
            
            GL.Begin(GL.TRIANGLES);
            foreach (var cell in cells)
            {
                //var randomCellColor = new Color(Random.value, Random.value, Random.value);

                foreach (var edge in cell.Edges)
                {
                    var left = edge.LeftVertex;
                    var right = edge.RightVertex;

                    GL.Color(ColorFromElevation(map.VerticesHeightsByIndex[left.VertexIndex]));
                    GL.Vertex3(left.x, left.y, 0);
                    GL.Color(ColorFromElevation(map.VerticesHeightsByIndex[right.VertexIndex]));
                    GL.Vertex3(right.x, right.y, 0);

                    var averageHeight = cell.VertexIndices.Average(vi => map.VerticesHeightsByIndex[vi]);
                    GL.Color(ColorFromElevation(averageHeight));
                    GL.Vertex3(cell.CenterPoint.x, cell.CenterPoint.y, 0);
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
            Texture2D newTexture = new Texture2D(textureSize, textureSize);
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