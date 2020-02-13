using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate float MaskMethod(float distance);

public enum MaskTypes {
    None,
    Square
}

public static class Masks
{
    public static MaskMethod[] maskMethods =
    {
        SquareGradient
    };

    public static float ApplyMask(MaskMethod method, float distance)
    {
        return method(distance);
    }
    
    public static float SquareGradient(float distance)
    {
        return 1f - distance;
    }
}
