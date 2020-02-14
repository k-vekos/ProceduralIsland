using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainCreator : MonoBehaviour
{
    public float frequency = 1f;
    public NoiseMethodType type;
    public MaskTypes upperMask;
    public MaskTypes lowerMask;
    [Range(1, 8)]
    public int octaves = 1;
    [Range(1f, 4f)]
    public float lacunarity = 2f;
    [Range(0f, 1f)]
    public float persistence = 0.5f;
    public float scale = 1f;
    public float heightScale = 0.1f;
    public float zOffset = 0f;
    
    private Terrain _terrain;
    private TerrainData _terrainData;

    public void SetHeights()
    {
        _terrain = GetComponent<Terrain>();
        _terrainData = _terrain.terrainData;
        
        var resolution = _terrainData.heightmapResolution;
        
        var size = _terrainData.size * scale; // remember: y is height

        var point00 = transform.TransformPoint(new Vector3(0, 0, zOffset));
        var point10 = transform.TransformPoint(new Vector3(size.x, 0, zOffset));
        var point01 = transform.TransformPoint(new Vector3(0, size.z, zOffset));
        var point11 = transform.TransformPoint(new Vector3(size.x, size.z, zOffset));
        
        float maxDistance = Vector3.Distance(point00, point11) * 0.5f;
        var center = transform.TransformPoint(new Vector3(size.x * 0.5f, size.z * 0.5f, zOffset)); 

        // 3D noise
        NoiseMethod method = Noise.noiseMethods[(int)type][2];
        
        MaskMethod lowerMaskMethod = Masks.maskMethods[(int)lowerMask];
        MaskMethod upperMaskMethod = Masks.maskMethods[(int)upperMask];

        var heightArray = new float[resolution, resolution];
        var stepSize = 1f / resolution;
        for (int y = 0; y < resolution; y++) 
        {
            Vector3 point0 = Vector3.Lerp(point00, point01, (y + 0.5f) * stepSize);
            Vector3 point1 = Vector3.Lerp(point10, point11, (y + 0.5f) * stepSize);
            for (int x = 0; x < resolution; x++) 
            {
                Vector3 point = Vector3.Lerp(point0, point1, (x + 0.5f) * stepSize);
                float sample = Noise.Sum(method, point, frequency, octaves, lacunarity, persistence);
                if (type != NoiseMethodType.Value) 
                {
                    sample = sample * 0.5f + 0.5f;
                }
                
                // Apply mask(s)
                float distance = Vector3.Distance(point, center);
                distance /= maxDistance;
                //sample = upperMaskMethod(distance) * sample;
                sample = lowerMaskMethod(distance) + sample * (upperMaskMethod(distance) - lowerMaskMethod(distance));

                heightArray[x, y] = sample * heightScale;
            }
        }

        _terrainData.SetHeights(0, 0, heightArray);
    }
}