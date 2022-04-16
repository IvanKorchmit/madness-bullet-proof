using UnityEngine;

public static class PixelPerfectUtils
{
    public const int PIXELS_PER_UNIT = 16;

    public static Vector3 SnapVectorToPixel(Vector3 position, float pixelsPerUnit)
    {
        Vector3 result = new Vector3(Mathf.Round(position.x * pixelsPerUnit) / pixelsPerUnit, Mathf.Round(position.y * pixelsPerUnit) / pixelsPerUnit, position.z);
        return result;
    }
    public static float SnapFloatToPixel(float value, float pixelsPerUnit)
    {
        float result = Mathf.RoundToInt(value * pixelsPerUnit) / pixelsPerUnit;
        return result;
    }
}

