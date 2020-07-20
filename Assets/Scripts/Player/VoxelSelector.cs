using UnityEngine;

[System.Serializable]
public struct SelectionPoint
{
    public Vector3 Position;
    public Vector3 Normal;
}
public class VoxelSelector : MonoBehaviour
{
    public SelectionType selectionType;
    public float selectionRange = 10f;
    public SelectionPoint selection;
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

    private void SelectVoxel()
    {
        selecting = false;
        selection.Position = Vector3.zero;
        selection.Normal = Vector3.zero;
        switch (selectionType)
        {
            case SelectionType.OnHit:
                RaycastHit hit;
                Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit);
                if (hit.collider)
                {
                    selecting = true;
                    selection.Position = hit.point;
                    selection.Normal = hit.normal;
                }
                break;
            case SelectionType.FixRange:
                selecting = true;
                Ray r = Camera.main.ScreenPointToRay(Input.mousePosition);
                selection.Position = r.origin + r.direction.normalized * selectionRange;
                break;
            case SelectionType.FixRangeOrOnHit:
                RaycastHit rh;
                Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out rh, selectionRange);
                if (rh.collider)
                {
                    selecting = true;
                    selection.Position = rh.point;
                    selection.Normal = rh.normal;
                }
                break;
            default:
                break;
        }
    }
}