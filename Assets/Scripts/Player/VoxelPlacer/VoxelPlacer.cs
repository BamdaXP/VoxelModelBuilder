using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoxelPlacer : MonoBehaviour
{
    public HitPointReader hitPointReader;
    public ObjectSelector objectSelector;
    // Start is called before the first frame update
    public Voxel voxelArg;
    // Update is called once per frame
    private void Start()
    {
        //Setup voxel arg
        voxelArg = new Voxel()
        {
            voxel = VoxelInfoLibrary.GetVoxel("Stone"),
            color = Color.white
        };
    }
    void Update()
    {
        if (hitPointReader.hitting)
        {
            //Place
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                /*
                foreach (var o in objectSelector.selectedObjects)
                {
                    WorldDataManager.Instance.ActiveWorld.SetVoxelAt(
                        o, 
                        hitPointReader.hitPoint.position + hitPointReader.hitPoint.normal / 2, 
                        voxelArg);
                    o.UpdateObjectMesh();
                }
                */
                List<ObjectComponent> Objects = new List<ObjectComponent>();
                foreach (var o in WorldDataManager.Instance.ActiveWorld.ObjectList)
                {
                    if (o.IsNearVoxel(hitPointReader.hitPoint.position + hitPointReader.hitPoint.normal / 2)&&!o.voxelObjectData.isStatic)
                    {
                        WorldDataManager.Instance.ActiveWorld.SetVoxelAt(
                        o,
                        hitPointReader.hitPoint.position + hitPointReader.hitPoint.normal / 2,
                        voxelArg);
                        o.UpdateObjectMesh();
                        Objects.Add(o);
                    }
                }
                if (Objects.Count == 0)
                {
                    var o= WorldDataManager.Instance.ActiveWorld.
                        CreateNewObject(MathHelper.WorldPosToWorldIntPos(hitPointReader.hitPoint.position + hitPointReader.hitPoint.normal / 2));
                    WorldDataManager.Instance.ActiveWorld.SetVoxelAt(
                       o,
                       hitPointReader.hitPoint.position + hitPointReader.hitPoint.normal / 2,
                       voxelArg);
                    o.UpdateObjectMesh();
                }
                else
                {
                    var firstObj = Objects[0];
                    for (int i = 1; i < Objects.Count; i++)
                    {
                        WorldDataManager.Instance.ActiveWorld.MergeTwoObjects(firstObj, Objects[i], WorldData.MergeType.Or);
                    }
                }
            }

            
            //Erase
            else if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                List<ObjectComponent> deleteObjects = new List<ObjectComponent>();
                foreach (var o in objectSelector.selectedObjects)
                {
                    WorldDataManager.Instance.ActiveWorld.DeleteVoxelAt(o, 
                        hitPointReader.hitPoint.position - hitPointReader.hitPoint.normal / 2);
                    if (o.voxelObjectData.VoxelDataDict.Count == 0) deleteObjects.Add(o);
                    o.UpdateObjectMesh();
                }
                for (int i = 0; i < deleteObjects.Count;i++)
                {
                    WorldDataManager.Instance.ActiveWorld.DeleteObject(deleteObjects[i]);
                }
            }
        }
        
    }
}
