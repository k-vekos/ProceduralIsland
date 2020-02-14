using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate float MaskMethod(float distance);

public enum MaskTypes {
    None,
    SquareLower,
    SquareUpper
}

public static class Masks
{
    public static MaskMethod[] maskMethods =
    {
        None,
        SquareGradientLower,
        SquareGradientUpper
    };

    public static float ApplyMask(MaskMethod method, float distance)
    {
        return method(distance);
    }
    
    public static float None(float distance)
    {
        return distance;
    }
    
    public static float SquareGradientLower(float distance)
    {
        return 1f - distance;
    }
    
    public static float SquareGradientUpper(float distance)
    {
        return 2f - distance;
    }
}
