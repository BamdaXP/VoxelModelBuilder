using UnityEngine;
using System.Collections.Generic;
public class ChunkUpdator : MonoBehaviour
{
    public Vector3Int positionInt;
    public Vector2Int chunkPos;
    public Vector3Int certainPos;
    [Range(0,5)]
    public int renderRange = 1;

    private void Start()
    {
        //Start the world
        WorldDataManager.Instance.CreateNewWorld("new world");
        WorldDataManager.Instance.ActivateWorld("new world");
        SendChunkRequest();

        WorldDataManager.Instance.ActiveWorld.RefreshAllActiveChunks();
    }
    private void Update()
    {
        SendChunkRequest();
    }

    public void SendChunkRequest()
    { 
        Vector3 position = transform.position;
        positionInt = MathHelper.WorldPosToWorldIntPos(position);
        chunkPos = MathHelper.WorldPosToChunkPos(position);
        certainPos =MathHelper.WorldPosToCertainPos(position);


        List<Vector2Int> requiredChunks = new List<Vector2Int>();
        for (int x = -renderRange; x <= renderRange; x++)
        {
            for (int y = -renderRange; y <= renderRange; y++)
            {
                requiredChunks.Add(new Vector2Int(x, y) + chunkPos);
            }
        }

        WorldDataManager.Instance.ActiveWorld.UpdateWorld(requiredChunks);
    }
}
