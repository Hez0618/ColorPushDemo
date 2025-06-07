using UnityEngine;

public static class ColorUtils
{
    public static bool IsSimilar(Color a, Color b, float tolerance = 0.03f)
    {
        float diffR = Mathf.Abs(a.r - b.r);
        float diffG = Mathf.Abs(a.g - b.g);
        float diffB = Mathf.Abs(a.b - b.b);
        float diffA = Mathf.Abs(a.a - b.a);

        return diffR < tolerance && diffG < tolerance && diffB < tolerance && diffA < tolerance;
    }



}

