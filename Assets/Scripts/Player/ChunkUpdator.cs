﻿using UnityEngine;
using System.Collections.Generic;
public class ChunkUpdator : MonoBehaviour
{
    public Vector3Int positionInt;
    public Vector2Int chunkPos;
    public Vector3Int positionInChunk;
    [Range(0,5)]
    public int renderRange = 1;

    private void Start()
    {
        WorldDataManager.Instance.CreateNewWorld("new world");
        WorldDataManager.Instance.ActivateWorld("new world");
        SendChunkRequest();

        VoxelInfo v = VoxelInfoLibrary.GetTile("Stone");
        if (WorldDataManager.Instance == null)
            Debug.Log("null");
        for (int x = -10; x < 10; x++)
        {
            for (int y = -10; y < 10; y++)
            {
                WorldDataManager.Instance.ActiveWorld.SetVoxelAt(new Vector3(x, 0, y), v);
            }
        }
        WorldDataManager.Instance.ActiveWorld.RefreshAllActiveChunks();
    }
    private void Update()
    {
        SendChunkRequest();
    }

    public void SendChunkRequest()
    { 
        Vector3 position = transform.position;
        positionInt = WorldDataManager.Instance.ActiveWorld.GetGlobalVoxelPosition(position);
        chunkPos = WorldDataManager.Instance.ActiveWorld.GetChunkPosition(position);
        positionInChunk = WorldDataManager.Instance.ActiveWorld.GetVoxelPositionInChunk(position,chunkPos);


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
