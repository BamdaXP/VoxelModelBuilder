﻿using UnityEngine;
using System.Collections.Generic;


public class WorldData
{
    public static Transform worldTransform => GameObject.Find("WorldObject").GetComponent<Transform>();
    public const int WORLD_SIZE = 1024;
    public string name;
    public int WorldSeed { get; private set; }


    public Dictionary<Vector2Int, ChunkComponent> ActiveChunksDict;
    public Dictionary<Vector2Int, ChunkComponent> GeneratedChunksDict;
    public WorldData(string name)
    {
        this.name = name;

        ActiveChunksDict = new Dictionary<Vector2Int, ChunkComponent>();
        GeneratedChunksDict = new Dictionary<Vector2Int, ChunkComponent>();
    }


    public VoxelInfo GetVoxelAt(Vector3 worldPos)
    {
        Vector2Int chunkPos = GetChunkPosition(worldPos);
        Vector3Int voxelPos = GetVoxelPositionInChunk(worldPos,chunkPos);
        return GetTileAt(chunkPos, voxelPos);
    }
    public VoxelInfo GetTileAt(Vector2Int chunkPos, Vector3Int voxelPos)
    {
        return GeneratedChunksDict[chunkPos].chunkData.voxelGrid.GetDataAt(voxelPos.x, voxelPos.y, voxelPos.z);
    }
    public void SetVoxelAt(Vector3 worldPos, VoxelInfo v)
    {
        Vector2Int chunkPos = GetChunkPosition(worldPos);
        Vector3Int voxelPos = GetVoxelPositionInChunk(worldPos,chunkPos);
        SetVoxelAt(chunkPos, voxelPos, v);
    }
    public void SetVoxelAt(Vector2Int chunkPos, Vector3Int voxelPosInChunk, VoxelInfo v)
    {
        if (GeneratedChunksDict.ContainsKey(chunkPos))
        {
            GeneratedChunksDict[chunkPos].chunkData.voxelGrid.SetDataAt(voxelPosInChunk.x, voxelPosInChunk.y, voxelPosInChunk.z, v);
        }
        else
        {
            Debug.LogWarning(chunkPos + "has not been generated yet!");
        }
        
    }

    public void UpdateWorld(List<Vector2Int> requiredChunks)
    {
        //Load newly needed chunks
        foreach (var l in requiredChunks)
        {
            if (!ActiveChunksDict.ContainsKey(l))
            {
                if (GeneratedChunksDict.ContainsKey(l))
                {
                    EnableChunkAt(l);
                }
                else
                {
                    GenerateChunkAt(l);
                }
            }
        }
        //Get active chunks including new chunks
        List<Vector2Int> activeChunks = new List<Vector2Int>();
        foreach (var l in ActiveChunksDict.Keys)
        {
            activeChunks.Add(l);
        }

        //Remove new chunks from the list
        foreach (var l in requiredChunks)
        {
            activeChunks.Remove(l);
        }

        //The rest are the chunks not needed 
        foreach (var l in activeChunks)
        {
            DisableChunkAt(l);
        }

    }
    public void RefreshAllActiveChunks()
    {
        foreach (var c in ActiveChunksDict.Values)
        {
            c.UpdateChunk();
        }
        Debug.Log("Update ok");
    }
    public void GenerateChunkAt(Vector2Int pos)
    {
        //Instantiate the chunk object under the world object and get the chunk component of it
        ChunkComponent component = GameObject.Instantiate(
            Resources.Load<GameObject>("Prefabs/ChunkObject"),
            new Vector3(pos.x * ChunkData.CHUNK_SIZE, 0, pos.y * ChunkData.CHUNK_SIZE),
            Quaternion.Euler(0, 0, 0),
            worldTransform).GetComponent<ChunkComponent>();

        //Initialize the chunk data of the component
        component.chunkData = new ChunkData();
        //Add to global dict
        GeneratedChunksDict.Add(pos, component);
        ActiveChunksDict.Add(pos, component);
        //Update chunk after generated
        component.UpdateChunk();
    }

    public void DisableChunkAt(Vector2Int pos)
    {
        ActiveChunksDict[pos].gameObject.SetActive(false);
        ActiveChunksDict.Remove(pos);
    }
    public void EnableChunkAt(Vector2Int pos)
    {
        ActiveChunksDict.Add(pos, GeneratedChunksDict[pos]);
        ActiveChunksDict[pos].gameObject.SetActive(true);
    }
    public void UpdateChunkAt(Vector2Int position)
    {
        ActiveChunksDict[position].UpdateChunk();
    }

    public Vector2Int GetChunkPosition(Vector3 worldPos)
    {

        Vector2Int value =  new Vector2Int(
            Mathf.FloorToInt(worldPos.x / ChunkData.CHUNK_SIZE),
            Mathf.FloorToInt(worldPos.z / ChunkData.CHUNK_SIZE));
        return value;
    }
    public Vector3Int GetGlobalVoxelPosition(Vector3 worldPos)
    {
        Vector3Int value = new Vector3Int(
            Mathf.FloorToInt(worldPos.x),
            Mathf.FloorToInt(worldPos.y),
            Mathf.FloorToInt(worldPos.z));
        return value;
    }
    public Vector3Int GetVoxelPositionInChunk(Vector3 worldPos,Vector2Int chunkPos)
    {
        Vector3Int value = new Vector3Int(
            Mathf.FloorToInt(worldPos.x),
            Mathf.FloorToInt(worldPos.y),
            Mathf.FloorToInt(worldPos.z));
        return value-new Vector3Int(chunkPos.x,0,chunkPos.y)*ChunkData.CHUNK_SIZE;
    }
}