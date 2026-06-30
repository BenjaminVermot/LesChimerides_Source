public static class MathExtensions
{
    /// <summary>
    /// Projette une valeur d'une plage (fromMin à fromMax) vers une autre plage (toMin à toMax).
    /// </summary>
    public static float MapRange(this float value, float fromMin, float fromMax, float toMin, float toMax)
    {
        return toMin + (value - fromMin) * (toMax - toMin) / (fromMax - fromMin);
    }

    /// <summary>
    /// Même chose que Map, mais s'assure que le résultat reste bloqué entre toMin et toMax.
    /// </summary>
    public static float MapClamped(this float value, float fromMin, float fromMax, float toMin, float toMax)
    {
        float mapped = value.MapRange(fromMin, fromMax, toMin, toMax);

        // On gère le cas où toMin est plus grand que toMax (inversion)
        float min = System.Math.Min(toMin, toMax);
        float max = System.Math.Max(toMin, toMax);

        return System.Math.Clamp(mapped, min, max);
    }
}