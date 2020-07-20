using System.Collections.Generic;
using System.Configuration;
using UnityEngine;

public class WorldEditor : MonoBehaviour
{
    public VoxelSelector selector;
    public enum EditorAction
    {
        Place,
        PlaceBuffer,
        AddPoint,
        TwoPointCube,
        Sphere
    }
    public EditorAction action;

    public List<VoxelInfo> voxelArgs;

    private GridData3D<VoxelInfo> m_gridBuffer;
    public GridData3D<VoxelInfo> GridBuffer
    {
        get
        {
            return m_gridBuffer;
        }
        set
        {
            if (m_gridBuffer != null)
            {
                m_gridBuffer.Dispose();
            }
            m_gridBuffer = value;
        }
    }
    public List<SelectionPoint> selectionArgs;
    public List<int> integerArgs;

    private void Awake()
    {
        voxelArgs = new List<VoxelInfo>();
        selectionArgs = new List<SelectionPoint>();
        integerArgs = new List<int>();
        voxelArgs.Add(VoxelInfoLibrary.GetVoxel("Stone"));
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            TakePoint();
            Place();
        }else if (Input.GetKeyDown(KeyCode.Space))
        {
            TakePoint();
            Erase();
        }
    }

    public void TakePoint()
    {
        if (selector.selecting)
        {
            selectionArgs.Add(selector.selection);
        }
    }
    public void TakeSample()
    {
        if (selectionArgs.Count > 0)
        {
            Vector3 worldPos = selectionArgs[0].Position - selectionArgs[0].Normal / 2;
            VoxelInfo v = WorldDataManager.Instance.ActiveWorld.GetVoxelAt(worldPos);
            voxelArgs.Add(v);

            selectionArgs.Clear();
        }
    }
    public void TakeSampleBuffer()
    {
        if (selectionArgs.Count > 1)
        {
            Vector3 a = selectionArgs[0].Position - selectionArgs[0].Normal / 2;
            Vector3 b = selectionArgs[1].Position - selectionArgs[1].Normal / 2;

            Vector3Int min = WorldDataManager.Instance.ActiveWorld.GetGlobalVoxelPosition(
                new Vector3(
                    Mathf.Min(a.x, b.x),
                    Mathf.Min(a.y, b.y),
                    Mathf.Min(a.z, b.z)
                    )
                );
            Vector3Int max = WorldDataManager.Instance.ActiveWorld.GetGlobalVoxelPosition(
                new Vector3(
                    Mathf.Max(a.x, b.x),
                    Mathf.Max(a.y, b.y),
                    Mathf.Max(a.z, b.z)
                    )
                );
            int width = max.x - min.x + 1;
            int height = max.y - min.y + 1;
            int length = max.z - min.z + 1;
            GridBuffer = new GridData3D<VoxelInfo>(width, height, length);
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    for (int z = 0; z < length; z++)
                    {
                        VoxelInfo v =
                            WorldDataManager.Instance.ActiveWorld.GetVoxelAt(
                                new Vector3(min.x + x, min.y + y, min.z + z));

                        GridBuffer.SetDataAt(x, y, z, v);
                    }
                }
            }

            selectionArgs.Clear();
        }
    }
    public void Place()
    {
        if (selectionArgs.Count > 0 && voxelArgs.Count > 0)
        {
            Vector3 worldPos = selectionArgs[0].Position + selectionArgs[0].Normal / 2;
            WorldDataManager.Instance.ActiveWorld.SetVoxelAt(
                worldPos,
                voxelArgs[0]);
            WorldDataManager.Instance.ActiveWorld.UpdateChunkAt(
                WorldDataManager.Instance.ActiveWorld.GetChunkPosition(worldPos));

            selectionArgs.Clear();
        }
    }
    public void Erase()
    {
        if (selectionArgs.Count > 0)
        {
            Vector3 worldPos = selectionArgs[0].Position - selectionArgs[0].Normal / 2;
            WorldDataManager.Instance.ActiveWorld.SetVoxelAt(
                worldPos,
                null);
            WorldDataManager.Instance.ActiveWorld.UpdateChunkAt(
                WorldDataManager.Instance.ActiveWorld.GetChunkPosition(worldPos));

            selectionArgs.Clear();
        }
    }
    public void PlaceBuffer()
    {
        if (GridBuffer != null && selectionArgs.Count>0)
        {
            Vector3 pos = selectionArgs[0].Position - selectionArgs[0].Normal;
            int width = GridBuffer.Width;
            int height = GridBuffer.Height;
            int length = GridBuffer.Length;
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    for (int z = 0; z < length; z++)
                    {
                        WorldDataManager.Instance.ActiveWorld.SetVoxelAt(
                            pos + new Vector3(x, y, z), 
                            GridBuffer.GetDataAt(x, y, z)
                            );
                    }
                }
            }
            selectionArgs.Clear();
        }
    }
    public void TwoPointCube()
    {

    }
    public void Sphere()
    {

    }
    public void Act()
    {
        switch (action)
        {
            case EditorAction.Place:
                Place();
                break;
            case EditorAction.TwoPointCube:
                TwoPointCube();
                break;
            case EditorAction.Sphere:
                Sphere();
                break;
            default:
                break;
        }
    }
}
