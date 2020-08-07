using UnityEngine;

[System.Serializable]
public struct HitPoint
{
    public Vector3 position;
    public Vector3 normal;
}
public class HitPointReader : MonoBehaviour
{
    public HitPoint hitPoint;
    public bool hitting { get; private set; }

    private void Update()
    {
        SelectVoxel();
    }

    private void SelectVoxel()
    {
        hitting = false;
        hitPoint.position = Vector3.zero;
        hitPoint.normal = Vector3.zero;

        RaycastHit hit;
        Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit);
        if (hit.collider)
        {
            hitting = true;
            hitPoint.position = hit.point;
            hitPoint.normal = hit.normal;
        }

    }
}