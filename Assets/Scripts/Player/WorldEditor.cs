using System.Collections.Generic;
using System.Configuration;
using System.Runtime.InteropServices.ComTypes;
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
        Sphere,
        Stretching,
        MoveObject,
        CopyObject,
        Select
    }
    public EditorAction action;

    public List<VoxelInfo> voxelArgs;

    public ObjectData nowObject;

    public ObjectData virtualObject;

    public Vector3 preMousePos=new Vector3();

    public Vector3 moveVector = new Vector3();

    public List<ObjectData> SelectedObjects = new List<ObjectData>();
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
        voxelArgs.Add(VoxelInfoLibrary.GetVoxel("Virtual"));
        voxelArgs.Add(VoxelInfoLibrary.GetVoxel("Selected"));
        action = EditorAction.Place;
    }
    private void ChangeAction()
    {
        if (virtualObject != null)
            virtualObject.Destroy();
        virtualObject = null;
        nowObject = null;
        foreach (var obj in SelectedObjects)
        {
            obj.BackToOriginal();
        }
    }
    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.Alpha1) && action != EditorAction.Stretching)
        {
            ChangeAction();
            action = EditorAction.Stretching;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha1) && action == EditorAction.Stretching)
        {
            ChangeAction();
            action = EditorAction.Place;
        }
        if (Input.GetKeyDown(KeyCode.Alpha2) && action != EditorAction.MoveObject)
        {
            ChangeAction();
            action = EditorAction.MoveObject;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2) && action == EditorAction.MoveObject)
        {
            ChangeAction();
            action = EditorAction.Place;
        }
        if (Input.GetKeyDown(KeyCode.Alpha3) && action != EditorAction.CopyObject)
        {
            ChangeAction();
            action = EditorAction.CopyObject;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3) && action == EditorAction.CopyObject)
        {
            ChangeAction();
            action = EditorAction.Place;
        }
        if (Input.GetKeyDown(KeyCode.Alpha4) && action != EditorAction.Select)
        {
            ChangeAction();
            action = EditorAction.Select;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4) && action == EditorAction.Select)
        {
            ChangeAction();
            action = EditorAction.Place;
        }
        Act();
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

            WorldDataManager.Instance.ActiveWorld.UpdateObjects(worldPos, voxelArgs[0]);
            //WorldDataManager.Instance.ActiveWorld.UpdateChunkAt(
                //WorldDataManager.Instance.ActiveWorld.GetChunkPosition(worldPos));

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
            WorldDataManager.Instance.ActiveWorld.UpdateObjects(worldPos, null, true);
        //WorldDataManager.Instance.ActiveWorld.UpdateChunkAt(
         //WorldDataManager.Instance.ActiveWorld.GetChunkPosition(worldPos));

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
    public void MoveObject()
    {
        if (Input.GetMouseButtonDown(0))
        {
            selectionArgs.Clear();
            nowObject = null;
            TakePoint();
            if (selectionArgs.Count > 0)
            {
                foreach (var obj in WorldDataManager.Instance.ActiveWorld.ObjectList)
                {
                    if (obj.Object == selectionArgs[0].Object)
                    {
                        preMousePos = obj.StartPoint;
                        nowObject = obj;
                        break;
                    }
                }
            }

            moveVector = Vector3.zero;
        }
        else if (Input.GetMouseButton(0))
        {
           
            if (nowObject != null && selectionArgs.Count > 0)
            {
                var pixelZ = Camera.main.WorldToScreenPoint(nowObject.StartPoint).z;
                var inverseProMatrix = Camera.main.projectionMatrix.inverse;
                Vector3 pixelPos = Input.mousePosition;
                pixelPos.x = 2 * (pixelPos.x - Camera.main.pixelWidth / 2) / Camera.main.pixelWidth;
                pixelPos.y = 2 * (pixelPos.y - Camera.main.pixelHeight / 2) / Camera.main.pixelHeight;
                pixelPos.z = 0.6f;
                Vector3 ViewPos = inverseProMatrix.MultiplyPoint(pixelPos);
                float w = Vector3.Dot(inverseProMatrix.GetRow(3), pixelPos) + inverseProMatrix.GetRow(3).w;
                ViewPos /= w;
                ViewPos = ViewPos.normalized * pixelZ;
                Vector3 worldPos = Camera.main.cameraToWorldMatrix.MultiplyPoint(ViewPos);
                
                
                moveVector += worldPos - preMousePos;
                if (moveVector.magnitude > 1) { nowObject.Move(moveVector); moveVector = Vector3.zero; }
                preMousePos = worldPos;
                
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            moveVector = Vector3.zero;
        }
    }
    public void Stretch()
    {
        if (Input.GetMouseButtonDown(0))
        {
            selectionArgs.Clear();
            nowObject = null;
            TakePoint();
            if (selectionArgs.Count > 0 && voxelArgs.Count > 0)
            {
                foreach (var obj in WorldDataManager.Instance.ActiveWorld.ObjectList)
                {
                    if (obj.Object == selectionArgs[0].Object)
                    {
                        nowObject = obj;
                        break;
                    }
                }
            }
        }
        if (nowObject != null&&selectionArgs.Count>0)
        {
            bool isAdd = true;
            Vector3Int Direction = new Vector3Int((int)selectionArgs[0].Normal.x, (int)selectionArgs[0].Normal.y, (int)selectionArgs[0].Normal.z);
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                isAdd = false;
                var temp = selectionArgs[0];
                temp.Position = nowObject.Stretch(Direction, selectionArgs[0].Position, 1, isAdd);
                selectionArgs[0] = temp;
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                isAdd = true;
                var temp = selectionArgs[0];
                temp.Position = nowObject.Stretch(Direction, selectionArgs[0].Position, 1, isAdd);
                selectionArgs[0] = temp;
            }
     
        }

    }

    public void CopyObject()
    {
        if (Input.GetMouseButtonDown(0) && virtualObject == null)
        {
            selectionArgs.Clear();
            TakePoint();
            if (selectionArgs.Count > 0)
            {
                foreach (var obj in WorldDataManager.Instance.ActiveWorld.ObjectList)
                {
                    if (obj.Object == selectionArgs[0].Object)
                    {
                        virtualObject = new ObjectData(obj, voxelArgs[1]);
                        nowObject = obj;
                        preMousePos = virtualObject.StartPoint;
                        break;
                    }
                }
            }
        }
        else if (Input.GetMouseButtonDown(0) && virtualObject != null)
        {
            selectionArgs.Clear();
            virtualObject.CopyFrom(nowObject);
            WorldDataManager.Instance.ActiveWorld.ObjectList.Add(virtualObject);
            virtualObject = null;
            nowObject = null;
        }
        if (virtualObject != null )
        {
            var pixelZ = Camera.main.WorldToScreenPoint(virtualObject.StartPoint).z;
            var inverseProMatrix = Camera.main.projectionMatrix.inverse;
            Vector3 pixelPos = Input.mousePosition;
            pixelPos.x = 2 * (pixelPos.x - Camera.main.pixelWidth / 2) / Camera.main.pixelWidth;
            pixelPos.y = 2 * (pixelPos.y - Camera.main.pixelHeight / 2) / Camera.main.pixelHeight;
            pixelPos.z = 0.6f;
            Vector3 ViewPos = inverseProMatrix.MultiplyPoint(pixelPos);
            float w = Vector3.Dot(inverseProMatrix.GetRow(3), pixelPos) + inverseProMatrix.GetRow(3).w;
            ViewPos /= w;
            ViewPos = ViewPos.normalized * pixelZ;
            Vector3 worldPos = Camera.main.cameraToWorldMatrix.MultiplyPoint(ViewPos);
            
            
            moveVector += worldPos - preMousePos;
            if (moveVector.magnitude > 1) { virtualObject.Move(moveVector); moveVector = Vector3.zero; }
            preMousePos = worldPos;
            
        }
    }
    public void Select()
    {
        if (Input.GetMouseButtonDown(0))
        {
            selectionArgs.Clear();
            TakePoint();
            if (selectionArgs.Count > 0)
            {
                foreach (var obj in WorldDataManager.Instance.ActiveWorld.ObjectList)
                {
                    if (obj.Object == selectionArgs[0].Object)
                    {
                        obj.TemporaryChangeVoxelInfo(voxelArgs[2]);
                        SelectedObjects.Add(obj);
                        break;
                    }
                }
            }
        }
        if (SelectedObjects.Count > 0 && Input.GetKeyDown(KeyCode.LeftArrow))
        {
            var sameVoxels = ObjectData.GetSameVoxels(SelectedObjects);
            for (int i = 0; i < SelectedObjects.Count; i++)
            {
                if (!SelectedObjects[i].SubVoxels(sameVoxels)) WorldDataManager.Instance.ActiveWorld.ObjectList.RemoveAt(i);
            }
            SelectedObjects.Clear();
        }
        else if (SelectedObjects.Count > 0 && Input.GetKeyDown(KeyCode.RightArrow))
        {
            for (int i = 1; i < SelectedObjects.Count; i++)
            {
                SelectedObjects[0] += SelectedObjects[i];
                SelectedObjects[i].Destroy();
                WorldDataManager.Instance.ActiveWorld.ObjectList.RemoveAt(i);
            }
            SelectedObjects.Clear();
        }
    }
    public void Act()
    {
        switch (action)
        {
            case EditorAction.Place:
                if (Input.GetMouseButtonDown(0))
                {
                    selectionArgs.Clear();
                    TakePoint();
                    Place();
                }
                else if (Input.GetKeyDown(KeyCode.Space))
                {
                    selectionArgs.Clear();
                    TakePoint();
                    Erase();
                }
                break;
            case EditorAction.TwoPointCube:
                TwoPointCube();
                break;
            case EditorAction.Sphere:
                Sphere();
                break;
            case EditorAction.Stretching:
                Stretch();
                break;
            case EditorAction.MoveObject:
                MoveObject();
                break;
            case EditorAction.CopyObject:
                CopyObject();
                break;
            case EditorAction.Select:
                Select();
                break;
            default:
                break;
        }
    }
}
