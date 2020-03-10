﻿using UnityEngine;

namespace Map
{
    public static class MapTextureRenderer
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

        public static Texture2D RenderCellsToTexture(Cell[] cells, int mapSize, int textureSize)
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
                GL.Color(Color.white);

                var firstVertex = cell.Vertices[0];
                var vertexCoint = cell.Vertices.Count;
                for (var i = 1; i < vertexCoint; i++)
                {
                    var j = (i + 1) % vertexCoint;
                    if (j == 0)
                        break;
                    
                    var current = cell.Vertices[i];
                    var next = cell.Vertices[(i + 1) % vertexCoint];
                    
                    GL.Vertex3(current.x, current.y, 0);
                    GL.Vertex3(next.x, next.y, 0);
                    GL.Vertex3(firstVertex.x, firstVertex.y, 0);
                }
            }
            GL.End();
            
            GL.PopMatrix();

            return CreateTextureFromRenderTexture(textureSize, renderTexture);
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