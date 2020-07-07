using UnityEngine;
using System.Collections.Generic;

public class VoxelSelector : MonoBehaviour
{
    public SelectionType selectionType;
    public float selectionRange = 10f;
    public VoxelInfo selectedVoxel =>
        WorldDataManager.Instance.ActiveWorld.GetVoxelAt(selectedPosition);
    public Vector3 selectedPosition;
    public Vector3 selectedNormal;
    public bool selecting { get; private set; }
    public enum SelectionType
    {
        OnHit,
        FixRange,
        FixRangeOrOnHit,
    }

    private void Update()
    {
        SelectVoxel();
    }

    public void SelectVoxel()
    {
        selectedPosition = Vector3.zero;
        selectedNormal = Vector3.zero;
        switch (selectionType)
        {
            case SelectionType.OnHit:
                RaycastHit hit;
                Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit);
                if (hit.collider)
                {
                    selecting = true;
                    selectedPosition = hit.point;
                    selectedNormal = hit.normal;
                }
                break;
            case SelectionType.FixRange:
                selecting = true;
                Ray r = Camera.main.ScreenPointToRay(Input.mousePosition);
                selectedPosition = r.origin + r.direction.normalized * selectionRange;
                break;
            case SelectionType.FixRangeOrOnHit:
                RaycastHit rh;
                Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out rh, selectionRange);
                if (rh.collider)
                {
                    selecting = true;
                    selectedPosition = rh.point;
                    selectedNormal = rh.normal;
                }
                break;
            default:
                break;
        }
    }
}