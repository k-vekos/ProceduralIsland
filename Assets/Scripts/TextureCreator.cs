using Noise;
using UnityEngine;

public class TextureCreator : MonoBehaviour
{
    [Range(2, 512)] public int resolution = 256;

    public float scale = 1f;

    public float frequency = 1f;
    
    /*[Range(1, 3)]
    public int dimensions = 3;*/
    
    //public NoiseMethodType type;

    //private Texture2D texture;
    
    [Range(1, 8)]
    public int octaves = 1;

    [Range(1f, 4f)]
    public float lacunarity = 2f;

    [Range(0f, 1f)]
    public float persistence = 0.5f;

    public bool expandRange = true;
    
    //public Gradient coloring;

    public void FillTexture()
    {
        var texture = NoiseTextureHelper.PerlinNoise(resolution, scale, frequency, octaves, lacunarity, persistence);
        
        if (expandRange)
            NoiseTextureHelper.ExpandRange(texture);
        
        GetComponent<MeshRenderer>().sharedMaterial.mainTexture = texture;
    }
}