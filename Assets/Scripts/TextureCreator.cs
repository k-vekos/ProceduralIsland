﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureCreator : MonoBehaviour
{
    [Range(2, 512)] public int resolution = 256;

    public float frequency = 1f;
    
    [Range(1, 3)]
    public int dimensions = 3;
    
    public NoiseMethodType type;

    public MaskTypes upperMask;
    
    public MaskTypes lowerMask;

    private Texture2D texture;
    
    [Range(1, 8)]
    public int octaves = 1;

    [Range(1f, 4f)]
    public float lacunarity = 2f;

    [Range(0f, 1f)]
    public float persistence = 0.5f;
    
    public Gradient coloring;
    
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

        float maxDistance = Vector3.Distance(point00, point11) * 0.5f;

        NoiseMethod method = Noise.noiseMethods[(int)type][dimensions - 1];

        MaskMethod lowerMaskMethod = Masks.maskMethods[(int)lowerMask];
        MaskMethod upperMaskMethod = Masks.maskMethods[(int)upperMask];
        
        var stepSize = 1f / resolution;
        for (int y = 0; y < resolution; y++) {
            Vector3 point0 = Vector3.Lerp(point00, point01, (y + 0.5f) * stepSize);
            Vector3 point1 = Vector3.Lerp(point10, point11, (y + 0.5f) * stepSize);
            for (int x = 0; x < resolution; x++) {
                Vector3 point = Vector3.Lerp(point0, point1, (x + 0.5f) * stepSize);
                float sample = Noise.Sum(method, point, frequency, octaves, lacunarity, persistence);

                if (type != NoiseMethodType.Value) {
                    sample = sample * 0.5f + 0.5f;
                }
                
                // Apply mask(s)
                float distance = Vector3.Distance(point, transform.position);
                distance /= maxDistance;
                /*sample = lowerMaskMethod(distance) + sample;
                sample = sample - upperMaskMethod(1 - distance);*/

                //sample = upperMaskMethod(1 - distance) - lowerMaskMethod(distance);
                
                sample = lowerMaskMethod(distance) + sample * (upperMaskMethod(distance) - lowerMaskMethod(distance));
                
                texture.SetPixel(x, y, coloring.Evaluate(sample));
            }
        }

        texture.Apply();
    }
}