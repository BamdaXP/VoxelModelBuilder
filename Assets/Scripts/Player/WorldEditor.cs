using System.Collections.Generic;
using UnityEngine;

public class WorldEditor : MonoBehaviour
{
    public VoxelSelector selector;
    public enum EditType
    {
        Place,
        TwoPointCube,
        Sphere
    }

    public EditType editType;

    public VoxelInfo voxelArg;
    public List<Vector3> pointArgs;
    public List<int> integerArgs;

    private void Awake()
    {
        voxelArg = null;
        pointArgs = new List<Vector3>();
        integerArgs = new List<int>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Apply();
        }
    }

    public void Place()
    {
        WorldData world = WorldDataManager.Instance.ActiveWorld;
        Debug.Log("Place" + selector.selectedPosition + selector.selectedNormal);
        Vector3 selectionPos = selector.selectedPosition + (selector.selectedNormal / 2) * (voxelArg == null ? -1 : 1);
        world.SetVoxelAt(
            selectionPos,
            voxelArg);
        world.UpdateChunkAt(world.GetChunkPosition(selectionPos));
    }
    public void TwoPointCube()
    {

    }
    public void Sphere()
    {

    }
    public void Apply()
    {
        if (selector.selecting)
        {
            switch (editType)
            {
                case EditType.Place:
                    Place();
                    break;
                case EditType.TwoPointCube:
                    TwoPointCube();
                    break;
                case EditType.Sphere:
                    Sphere();
                    break;
                default:
                    break;
            }
        }
    }
}
