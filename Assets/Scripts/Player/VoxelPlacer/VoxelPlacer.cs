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
                foreach (var o in objectSelector.selectedObjects)
                {
                    WorldDataManager.Instance.ActiveWorld.SetVoxelAt(
                        o, 
                        hitPointReader.hitPoint.position + hitPointReader.hitPoint.normal / 2, 
                        voxelArg);
                    o.UpdateObjectMesh();
                }

            }
            //Erase
            else if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                foreach (var o in objectSelector.selectedObjects)
                {
                    WorldDataManager.Instance.ActiveWorld.DeleteVoxelAt(o, 
                        hitPointReader.hitPoint.position - hitPointReader.hitPoint.normal / 2);
                    o.UpdateObjectMesh();
                }
            }
        }
        
    }
}
