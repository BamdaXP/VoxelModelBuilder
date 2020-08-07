using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct SelectionPoint
{
    public Vector3Int position;
    public Vector3 normal;
}
public class FaceSelector : MonoBehaviour
{
    public HitPointReader hitPointReader;
    public ObjectSelector objectSelector;

    public Dictionary<ObjectComponent, List<Vector3Int>> selectionPointDict;

    public Vector3? normal;

    //The point when left mouse clicked down
    private HitPoint? m_downPoint;
    //The point when left mouse release up
    private HitPoint? m_upPoint;

    // Start is called before the first frame update
    private void Awake()
    {
        normal = null;
        m_downPoint = null;
        m_upPoint = null;
        selectionPointDict = new Dictionary<ObjectComponent, List<Vector3Int>>();
    }


    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            m_upPoint = null;
            m_downPoint = null;
            if (hitPointReader.hitting)
            {
                m_downPoint = hitPointReader.hitPoint;
                normal = m_downPoint.Value.normal;
            }
        }
        if (Input.GetKey(KeyCode.Mouse0))
        {
            HitPoint currentPoint = hitPointReader.hitPoint;
            if (m_downPoint != null && hitPointReader.hitting)
            {
                //Must be same normal face
                if (currentPoint.normal == m_downPoint.Value.normal &&
                    Vector3.Dot(currentPoint.position - m_downPoint.Value.position, currentPoint.normal) == 0)
                {
                    m_upPoint = currentPoint;

                    Vector3 down = m_downPoint.Value.position - m_downPoint.Value.normal / 2;
                    Vector3 up = m_upPoint.Value.position - m_upPoint.Value.normal / 2;
                    Vector3Int min = MathHelper.WorldPosToWorldIntPos(
                        new Vector3(
                            Mathf.Min(down.x, up.x),
                            Mathf.Min(down.y, up.y),
                            Mathf.Min(down.z, up.z)
                            )
                        );
                    Vector3Int max = MathHelper.WorldPosToWorldIntPos(
                        new Vector3(
                            Mathf.Max(down.x, up.x),
                            Mathf.Max(down.y, up.y),
                            Mathf.Max(down.z, up.z)
                            )
                        );

                    selectionPointDict.Clear();
                    foreach (var o in objectSelector.selectedObjects)
                    {
                        List<Vector3Int> points = new List<Vector3Int>();

                        for (int x = min.x; x <= max.x; x++)
                        {
                            for (int y = min.y; y <= max.y; y++)
                            {
                                for (int z = min.z; z <= max.z; z++)
                                {
                                    Vector3Int pos = new Vector3Int(x, y, z) - o.basePoint;
                                    if (o.voxelObjectData.GetVoxelAt(pos).voxel != null)
                                    {
                                        points.Add(pos);
                                    }
                                }
                            }
                        }
                        selectionPointDict.Add(o, points);
                    }
                }
            }


        }
    }



}
