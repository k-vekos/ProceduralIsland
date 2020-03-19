using Noise;
using UnityEngine;

public class TextureCreator : MonoBehaviour
{
    [Range(2, 512)]
    public int resolution = 256;
    public PerlinNoiseSettings settings;

    public void FillTexture()
    {
        var texture = NoiseTextureHelper.PerlinNoise(resolution, settings);
        GetComponent<MeshRenderer>().sharedMaterial.mainTexture = texture;
    }
}