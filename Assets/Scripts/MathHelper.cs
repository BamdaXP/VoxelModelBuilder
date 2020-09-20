using UnityEngine;
public static class MathHelper
{
    static public Vector3Int WorldPosToWorldIntPos(Vector3 worldPos)
    {
        Vector3Int value = new Vector3Int(
            Mathf.FloorToInt(worldPos.x),
            Mathf.FloorToInt(worldPos.y),
            Mathf.FloorToInt(worldPos.z));
        return value;
    }
    static public Vector3 IntPosScaleByFloat(Vector3Int IntPos,float scale)
    {
        Vector3 value = new Vector3(
            IntPos.x * scale,
            IntPos.y * scale,
            IntPos.z * scale);
        return value;
    }
}