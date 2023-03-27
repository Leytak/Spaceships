using UnityEngine;

public static class Percents
{
    public static float Add(float baseValue, int percents)
    {
        var multiplier = 1 + Mathf.Clamp01(percents / 100f);
        return baseValue * multiplier;
    }

    public static float Sub(float baseValue, int percents)
    {
        var multiplier = 1 - Mathf.Clamp01(percents / 100f);
        return baseValue * multiplier;
    }
}
