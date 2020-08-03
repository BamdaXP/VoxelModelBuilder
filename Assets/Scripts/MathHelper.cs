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
    static public Vector2Int WorldPosToChunkPos(Vector3 worldPos)
    {
        return new Vector2Int(
            Mathf.FloorToInt(worldPos.x / ChunkData.CHUNK_SIZE),
            Mathf.FloorToInt(worldPos.z / ChunkData.CHUNK_SIZE));

    }
    static public Vector3Int WorldPosToCertainPos(Vector3 worldPos)
    {

        Vector3Int value = new Vector3Int(
            Mathf.FloorToInt(worldPos.x),
            Mathf.FloorToInt(worldPos.y),
            Mathf.FloorToInt(worldPos.z));
        Vector2Int chunkPos = WorldPosToChunkPos(worldPos);
        return value -= new Vector3Int(chunkPos.x, 0, chunkPos.y) * ChunkData.CHUNK_SIZE;
    }
}