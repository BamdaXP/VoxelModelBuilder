using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class WorldData
{
    public static Transform WorldTransform => GameObject.Find("WorldObject").GetComponent<Transform>();
    public string name;


    public List<ObjectComponent> ObjectList;
    public WorldData(string name)
    {
        this.name = name;

        ObjectList = new List<ObjectComponent>();
    }


    public enum MergeType
    {
        Or,
        And,
        Not,
    }
    public void MergeTwoObjects(ObjectComponent o1, ObjectComponent o2, MergeType mergeType = MergeType.Or)
    {

        switch (mergeType)
        {
            case MergeType.Or:
                foreach (var pair in o2.voxelObjectData.VoxelDataDict)
                {
                    Vector3Int worldPos = pair.Key + o2.basePoint;
                    Voxel v1 = GetVoxelAt(o1, worldPos);
                    Voxel v2 = pair.Value;//o2 must has
                    //If o1 dosen't have but o2 has
                    if (v1.voxel == null)
                    {
                        //Set o1 empty ones to o2's
                        SetVoxelAt(o1, worldPos, v2);
                    }
                }
                break;
            case MergeType.And:
                foreach (var pair in o1.voxelObjectData.VoxelDataDict)
                {
                    Vector3Int worldPos = pair.Key + o1.basePoint;
                    Voxel v1 = pair.Value;//o1 must has
                    Voxel v2 = GetVoxelAt(o2, worldPos);
                    //If o1 has but o2 dosen't have
                    if (v2.voxel == null)
                    {
                        //Set empty voxel
                        SetVoxelAt(o1, worldPos, new Voxel());
                    }
                }
                break;
            case MergeType.Not:
                foreach (var pair in o2.voxelObjectData.VoxelDataDict)
                {
                    Vector3Int worldPos = pair.Key + o2.basePoint;
                    Voxel v1 = GetVoxelAt(o1, worldPos);
                    Voxel v2 = pair.Value;//o2 must has 
                    //If o1 and o2 both has
                    if (v1.voxel != null)
                    {
                        //Set empty voxel
                        SetVoxelAt(o1, worldPos, new Voxel());
                    }
                }
                break;
            default:
                break;
        }
        DeleteObject(o2);
        o1.UpdateObjectMesh();
    }
    public Voxel GetVoxelAt(ObjectComponent obj, Vector3 worldPos)
    {
        Vector3Int p = MathHelper.WorldPosToWorldIntPos(worldPos);
        return obj.voxelObjectData.GetVoxelAt(p - obj.basePoint);
    }
    public void SetVoxelAt(ObjectComponent obj, Vector3 worldPos, Voxel v)
    {
        Vector3Int p = MathHelper.WorldPosToWorldIntPos(worldPos);
        obj.voxelObjectData.SetVoxelAt(p - obj.basePoint, v);
    }
    public void UpdateAllObjects()
    {
        foreach (var o in ObjectList)
        {
            o.UpdateObjectMesh();
        }
    }
    public ObjectComponent CreateNewObject(Vector3Int basePoint)
    {

        var c = GameObject.Instantiate(
            Resources.Load<GameObject>("Prefabs/VoxelObject"),
            basePoint,
            Quaternion.Euler(new Vector3(0, 0, 0)),
            WorldTransform).GetComponent<ObjectComponent>();

        //Setup object
        c.basePoint = basePoint;
        c.voxelObjectData = new ObjectData();

        ObjectList.Add(c);

        return c;
    }
    public ObjectComponent GetVoxelObject(int index)
    {
        return ObjectList[index];
    }
    public int GetVoxelObjectIndex(ObjectComponent o)
    {
        return ObjectList.IndexOf(o);
    }
    public ObjectComponent[] GetVoxelObjectsAt(Vector3 worldPos)
    {
        List<ObjectComponent> result = new List<ObjectComponent>();
        foreach (var o in ObjectList)
        {
            Vector3Int intPos = MathHelper.WorldPosToWorldIntPos(worldPos);
            //local position
            if (o.voxelObjectData.GetVoxelAt(intPos-o.basePoint).voxel != null)
            {
                result.Add(o);
            }
        }
        return result.ToArray();
    }
    //Get the next object in the list, back to start if ended
    public ObjectComponent GetNextObject(ObjectComponent o)
    {
        int index = ObjectList.IndexOf(o);
        var result = GetVoxelObject(index + 1 >= ObjectList.Count ? 0 : index + 1);
        return result;
    }
    public void DeleteObject(int index)
    {
        GameObject.Destroy(ObjectList[index].gameObject);
        ObjectList.RemoveAt(index);
    }
    public void DeleteObject(ObjectComponent o)
    {
        GameObject.Destroy(o.gameObject);
        ObjectList.Remove(o);
    }
}