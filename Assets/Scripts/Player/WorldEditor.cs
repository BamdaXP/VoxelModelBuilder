using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEditor;
using UnityEngine;

public class WorldEditor : MonoBehaviour
{
    public VoxelSelector selector;
    public enum EditorMode
    {
        Place,
        Erase,
        PlaceBuffer,
        TakeSampleBuffer,
        TakePoint,
        OnePointCube,
        TwoPointCuboid,
        Sphere
    }
    public EditorMode mode;

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
        integerArgs.Add(3);
        voxelArgs.Add(VoxelInfoLibrary.GetVoxel("Stone"));
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            switch (mode)
            {
                case EditorMode.Place:
                    TakePoint();
                    Place(selectionArgs[selectionArgs.Count - 1], voxelArgs[voxelArgs.Count-1]);
                    selectionArgs.Clear();
                    break;
                case EditorMode.Erase:
                    TakePoint();
                    Erase(selectionArgs[selectionArgs.Count - 1]);
                    selectionArgs.Clear();
                    break;
                case EditorMode.TakeSampleBuffer:
                    TakePoint();
                    TakeSampleBuffer(selectionArgs[selectionArgs.Count - 1], selectionArgs[selectionArgs.Count - 2]);
                    selectionArgs.Clear();
                    break;
                case EditorMode.PlaceBuffer:
                    TakePoint();
                    PlaceBuffer(selectionArgs[selectionArgs.Count - 1], GridBuffer);
                    selectionArgs.Clear();
                    break;
                case EditorMode.TakePoint:
                    TakePoint();
                    break;
                case EditorMode.OnePointCube:
                    TakePoint();
                    OnePointCube(selectionArgs[selectionArgs.Count - 1], integerArgs[integerArgs.Count-1], voxelArgs[voxelArgs.Count - 1]);
                    selectionArgs.Clear();
                    break;
                case EditorMode.TwoPointCuboid:
                    TakePoint();
                    TwoPointCuboid(selectionArgs[selectionArgs.Count-1], selectionArgs[selectionArgs.Count - 2], voxelArgs[voxelArgs .Count- 1]);
                    selectionArgs.Clear();
                    break;
                case EditorMode.Sphere:
                    TakePoint();
                    Sphere(selectionArgs[selectionArgs.Count - 1], integerArgs[integerArgs.Count - 1], voxelArgs[voxelArgs.Count - 1]);
                    selectionArgs.Clear();
                    break;
                default:
                    break;
            }
 
        }
        else if (Input.GetKeyDown(KeyCode.Space))
        {
        }
        else if(Input.mouseScrollDelta.y < 0)
        {
            mode += mode >= EditorMode.Sphere ? 0 : 1;
        }
        else if (Input.mouseScrollDelta.y > 0)
        {
            mode -= mode <= EditorMode.Place ? 0 : 1;
        }


    }

    public void TakePoint()
    {
        if (selector.selecting)
        {
            selectionArgs.Add(selector.selection);
        }
    }
    public void TakeSample(SelectionPoint p)
    {

        Vector3 worldPos = p.Position - p.Normal / 2;
        VoxelInfo v = WorldDataManager.Instance.ActiveWorld.GetVoxelAt(worldPos);
        voxelArgs.Add(v);

        selectionArgs.Clear();

    }
    public void TakeSampleBuffer(SelectionPoint p1, SelectionPoint p2)
    {
        Vector3 a = p1.Position - p1.Normal / 2;
        Vector3 b = p2.Position - p2.Normal / 2;

        Vector3Int min = MathHelper.WorldPosToWorldIntPos(
            new Vector3(
                Mathf.Min(a.x, b.x),
                Mathf.Min(a.y, b.y),
                Mathf.Min(a.z, b.z)
                )
            );
        Vector3Int max = MathHelper.WorldPosToWorldIntPos(
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

    }
    public void Place(SelectionPoint p, VoxelInfo v)
    {
        Vector3 worldPos = selectionArgs[0].Position + selectionArgs[0].Normal / 2;

        Vector2Int chunkPos = MathHelper.WorldPosToChunkPos(worldPos);
        Vector3Int certainPos = MathHelper.WorldPosToCertainPos(worldPos);
        WorldDataManager.Instance.ActiveWorld.SetVoxelAt(chunkPos,certainPos,voxelArgs[0]);
        WorldDataManager.Instance.ActiveWorld.UpdateChunkAt(chunkPos);
    }
    public void Erase(SelectionPoint p)
    {

        Vector3 worldPos = p.Position - p.Normal / 2;

        Vector2Int chunkPos = MathHelper.WorldPosToChunkPos(worldPos);
        Vector3Int certainPos = MathHelper.WorldPosToCertainPos(worldPos);
        WorldDataManager.Instance.ActiveWorld.SetVoxelAt(chunkPos,certainPos,null);
        WorldDataManager.Instance.ActiveWorld.UpdateChunkAt(chunkPos);
    }
    public void PlaceBuffer(SelectionPoint p, GridData3D<VoxelInfo> buffer)
    {

        Vector3 pos = p.Position - p.Normal;
        int width = buffer.Width;
        int height = buffer.Height;
        int length = buffer.Length;
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                for (int z = 0; z < length; z++)
                {
                    WorldDataManager.Instance.ActiveWorld.SetVoxelAt(
                        pos + new Vector3(x, y, z),
                        buffer.GetDataAt(x, y, z)
                        );
                }
            }
        }
        WorldDataManager.Instance.ActiveWorld.RefreshAllActiveChunks();
    }
    public void OnePointCube(SelectionPoint p, int a, VoxelInfo v)
    {
        Vector3 pos = p.Position + p.Normal/2;

        for (int x = 0; x < a; x++)
        {
            for (int y = 0; y < a; y++)
            {
                for (int z = 0; z < a; z++)
                {
                    WorldDataManager.Instance.ActiveWorld.SetVoxelAt(new Vector3(x, y, z)+pos, v);
                }
            }
        }
        WorldDataManager.Instance.ActiveWorld.RefreshAllActiveChunks();
    }
    public void TwoPointCuboid(SelectionPoint p1, SelectionPoint p2,VoxelInfo v)
    {
        Vector3 a = p1.Position - p1.Normal / 2;
        Vector3 b = p2.Position - p2.Normal / 2;

        Vector3Int min = MathHelper.WorldPosToWorldIntPos(
            new Vector3(
                Mathf.Min(a.x, b.x),
                Mathf.Min(a.y, b.y),
                Mathf.Min(a.z, b.z)
                )
            );
        Vector3Int max = MathHelper.WorldPosToWorldIntPos(
            new Vector3(
                Mathf.Max(a.x, b.x),
                Mathf.Max(a.y, b.y),
                Mathf.Max(a.z, b.z)
                )
            );
        for (int x = min.x; x <= max.x; x++)
        {
            for (int y = min.y; y <= max.y; y++)
            {
                for (int z = min.z; z <= max.z; z++)
                {
                    WorldDataManager.Instance.ActiveWorld.SetVoxelAt(new Vector3(x,y,z), v);
                }
            }
        }
        WorldDataManager.Instance.ActiveWorld.RefreshAllActiveChunks();
    }
    public void Sphere(SelectionPoint p,int radius,VoxelInfo v)
    {
        Vector3Int a = MathHelper.WorldPosToWorldIntPos(p.Position - p.Normal/2);
        for (int x = -radius; x <= radius; x++)
        {
            for (int y =  -radius; y <= radius; y++)
            {
                for (int z =  -radius; z <= radius; z++)
                {
                    if (x * x + y * y + z * z <= radius * radius)
                        WorldDataManager.Instance.ActiveWorld.SetVoxelAt(new Vector3(x, y, z)+a, v);
                }
            }
        }
        WorldDataManager.Instance.ActiveWorld.RefreshAllActiveChunks();
    }

}
