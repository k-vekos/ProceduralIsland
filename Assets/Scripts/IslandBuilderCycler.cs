using UnityEngine;

[RequireComponent(typeof(IslandBuilder))]
public class IslandBuilderCycler : MonoBehaviour
{
    public float updateIntervalSeconds = 5f;
    
    private IslandBuilder _islandBuilder;
    private float _timeElapsed;

    void Start()
    {
        _islandBuilder = GetComponent<IslandBuilder>();
    }

    void Update()
    {
        _timeElapsed += Time.deltaTime;

        if (_timeElapsed > updateIntervalSeconds)
        {
            _timeElapsed = 0f;
            _islandBuilder.BuildIslandAndInitRenderers();
        }
    }
}
