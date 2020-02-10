using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainCreator : MonoBehaviour
{
    public float frequency = 1f;
    public NoiseMethodType type;
    [Range(1, 8)]
    public int octaves = 1;
    [Range(1f, 4f)]
    public float lacunarity = 2f;
    [Range(0f, 1f)]
    public float persistence = 0.5f;
    public float scale = 1f;
    
    private Terrain _terrain;
    private TerrainData _terrainData;

    public void SetHeights()
    {
        _terrain = GetComponent<Terrain>();
        _terrainData = _terrain.terrainData;
        
        var resolution = _terrainData.heightmapResolution;
        
        var size = _terrainData.size * scale; // remember: y is height

        var point00 = transform.TransformPoint(new Vector3(0, 0));
        var point10 = transform.TransformPoint(new Vector3(size.x, 0));
        var point01 = transform.TransformPoint(new Vector3(0, size.z));
        var point11 = transform.TransformPoint(new Vector3(size.x, size.z));

        // 2D noise
        NoiseMethod method = Noise.noiseMethods[(int)type][2];

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
                
                heightArray[x, y] = sample;
            }
        }

        _terrainData.SetHeights(0, 0, heightArray);
    }
}