using UnityEngine;

public static class DamageFalloff
{
    // 거리가 0이면 baseDamage, range에 닿으면 baseDamage * minRatio가 되도록 선형으로 줄인다.
    public static float Calculate(float baseDamage, float distance, float range, float minRatio)
    {
        if (range <= 0f)
        {
            return baseDamage;
        }
        float t = Mathf.Clamp01(distance / range);
        return baseDamage * Mathf.Lerp(1f, minRatio, t);
    }
}
