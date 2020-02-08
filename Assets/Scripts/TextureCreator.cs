using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureCreator : MonoBehaviour
{
    [Range(2, 512)] public int resolution = 256;

    public float frequency = 1f;

    private Texture2D texture;

    private void OnEnable()
    {
        if (texture == null)
        {
            texture = new Texture2D(resolution, resolution, TextureFormat.RGB24, true);
            texture.name = "Procedural Texture";
            texture.wrapMode = TextureWrapMode.Clamp;
            texture.filterMode = FilterMode.Trilinear;
            texture.anisoLevel = 9;
            GetComponent<MeshRenderer>().material.mainTexture = texture;
        }

        FillTexture();
    }

    private void Update()
    {
        if (transform.hasChanged)
        {
            transform.hasChanged = false;
            FillTexture();
        }
    }

    public void FillTexture()
    {
        if (texture.width != resolution)
        {
            texture.Resize(resolution, resolution);
        }

        var point00 = transform.TransformPoint(new Vector3(-0.5f, -0.5f));
        var point10 = transform.TransformPoint(new Vector3(0.5f, -0.5f));
        var point01 = transform.TransformPoint(new Vector3(-0.5f, 0.5f));
        var point11 = transform.TransformPoint(new Vector3(0.5f, 0.5f));

        var stepSize = 1f / resolution;
        for (var y = 0; y < resolution; y++)
        {
            var point0 = Vector3.Lerp(point00, point01, (y + 0.5f) * stepSize);
            var point1 = Vector3.Lerp(point10, point11, (y + 0.5f) * stepSize);
            for (var x = 0; x < resolution; x++)
            {
                var point = Vector3.Lerp(point0, point1, (x + 0.5f) * stepSize);
                texture.SetPixel(x, y, Color.white * Noise.Value(point, frequency));
            }
        }

        texture.Apply();
    }
}