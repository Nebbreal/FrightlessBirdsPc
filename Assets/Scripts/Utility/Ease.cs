using UnityEngine;

public static class Ease {
    public static float InLinearOutCubic(float x)
    {
        x = (Mathf.Min(x, 1f) - 0.5f) * 2f;
        if (x < 0)
        {
            return -x;
        }

        return OutCubic(x);
    }

    public static float InOutCubicNegInverted(float t)
    {
        if (Mathf.Abs(t) < 0.5)
        {
            return Mathf.Sign(t) * 4f * Mathf.Pow(t, 3);
        }

        return 1 - (Mathf.Pow(-2 * Mathf.Abs(t) + 2, 3) / 2f);
    }

    public static float OutCubic(float x)
    {
        return 1f - Mathf.Pow(1f - x, 3f);
    }
    public static float InOutBack(float x) {
        // Easing function values from Easings.net
        float c1 = 1.70158f;
        float c2 = c1 * 1.525f;

        if (x >= 1f)
        {
            return 1f;
        }

        return x < 0.5
        ? (Mathf.Pow(2f * x, 2f) * ((c2 + 1) * 2f * x - c2)) / 2f
        : (Mathf.Pow(2f * x - 2f, 2f) * ((c2 + 1f) * (x * 2f - 2f) + c2) + 2f) / 2f;
    }

    public static float OutExpo(float x)
    {
        return x >= 1f ? 1f : 1f - Mathf.Pow(2f, -10f * x);
    }

    public static float InOutCubic(float x)
    {
        return x < 0.5f ? 4 * Mathf.Pow(x,3) : 1 - Mathf.Pow(-2 * x + 2, 3) / 2;
    }
}