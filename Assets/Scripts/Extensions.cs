using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Extensions
{
    public static Color SetAlpha(this Color c, float a)
    {
        c.a = a;
        return c;
    }
    public static float Random(this Vector2 vec2)
    {
        return (float)(UnityEngine.Random.value * (vec2.y - vec2.x) + vec2.x);
    }
}
