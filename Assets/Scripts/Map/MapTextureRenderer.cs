using UnityEngine;

namespace Map
{
    public class MapTextureRenderer : MonoBehaviour
    {
        public MeshRenderer targetMeshRenderer;
        public Material drawMeshMaterial;

        public void RenderMeshToTexture(Mesh mesh)
        {
            var rt = RenderTexture.GetTemporary(512, 512);
            Graphics.SetRenderTarget(rt);
            
            GL.Clear(true, true, Color.black);
            
            // save the last camera state
            //GL.PushMatrix(); 
            // load an orthographic camera 
            //GL.LoadOrtho();

            if (drawMeshMaterial.SetPass(0))
            {
                Debug.LogError("Couldn't set materials pass...");
                return;
            }
            
            Graphics.DrawMeshNow(mesh, Matrix4x4.identity);

            // reset state
            Graphics.SetRenderTarget(null);

            targetMeshRenderer.material.mainTexture = ToTexture2D(rt);
            
            RenderTexture.ReleaseTemporary(rt);
            //GL.PopMatrix();
        }
        
        Texture2D ToTexture2D(RenderTexture rTex)
        {
            Texture2D tex = new Texture2D(rTex.width, rTex.height, TextureFormat.RGB24, false);
            RenderTexture.active = rTex;
            tex.ReadPixels(new Rect(0, 0, rTex.width, rTex.height), 0, 0);
            tex.Apply();
            return tex;
        }
    }
}