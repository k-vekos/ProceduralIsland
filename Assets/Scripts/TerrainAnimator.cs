using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TerrainCreator))]
public class TerrainAnimator : MonoBehaviour
{
    public float updateIntervalSeconds = 5f;
    
    private TerrainCreator _terrainCreator;
    private float _timeElapsed;
    //private Random _random;

    // Start is called before the first frame update
    void Start()
    {
        _terrainCreator = GetComponent<TerrainCreator>();
        //_random = new Random();
    }

    // Update is called once per frame
    void Update()
    {
        _timeElapsed += Time.deltaTime;

        if (_timeElapsed > updateIntervalSeconds)
        {
            _timeElapsed = 0f;
            _terrainCreator.zOffset = Random.Range(-1000f, 1000f);
            _terrainCreator.SetHeights();
        }
    }
}
