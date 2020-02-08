using UnityEngine;

public static class Noise
{
    private static int[] hash =
    {
        0, 1, 2, 3, 4, 5, 6, 7
    };
    
    public static float Value (Vector3 point, float frequency)
    {
        point *= frequency;
        int i = Mathf.FloorToInt(point.x);
        // "Because our array has a length of eight, if we limit ourselves to the three least significant bits of i,
        // the index will wrap around exactly when it needs to."
        i &= 7;
        return hash[i] / 7f;
    }
}