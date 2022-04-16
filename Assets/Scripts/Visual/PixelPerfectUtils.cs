using UnityEngine;

public static class PixelPerfectUtils
{
    public static Vector3 SnapToPixel(Vector3 position, float pixelsPerUnit)
    {
        Vector3 result = new Vector3(Mathf.Round(position.x * pixelsPerUnit) / pixelsPerUnit, Mathf.Round(position.y * pixelsPerUnit) / pixelsPerUnit, position.z);
        return result;
    }
}