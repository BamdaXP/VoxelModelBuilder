using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Assertions.Must;

public struct SelectionPoint
{
    public Vector3Int position;
    public Vector3 normal;
}
public class VoxelSelector : MonoBehaviour
{
    public HitPointReader hitPointReader;
    public ObjectSelector objectSelector;

    public GameObject selectionIndicator;

    public Dictionary<ObjectComponent, List<SelectionPoint>> hitPointDict;

    //The point when left mouse clicked down
    private HitPoint? downPoint;
    //The point when left mouse release up
    private HitPoint? upPoint;

    // Start is called before the first frame update
    private void Awake()
    {
        downPoint = null;
        upPoint = null;
        hitPointDict = new Dictionary<ObjectComponent, List<SelectionPoint>>();
    }


    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            upPoint = null;
            downPoint = null;
            if (hitPointReader.hitting)
            {
                downPoint = hitPointReader.hitPoint;
            }
        }
        if (Input.GetKey(KeyCode.Mouse0))
        {
            HitPoint currentPoint = hitPointReader.hitPoint;
            if (downPoint != null && hitPointReader.hitting)
            {
                //Must be same normal face
                if (currentPoint.normal == downPoint.Value.normal &&
                    Vector3.Dot(currentPoint.position - downPoint.Value.position, currentPoint.normal) == 0)
                {
                    upPoint = currentPoint;

                    Vector3 down = downPoint.Value.position - downPoint.Value.normal / 2;
                    Vector3 up = upPoint.Value.position - upPoint.Value.normal / 2;
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

                    hitPointDict.Clear();
                    foreach (var o in objectSelector.selectedObjects)
                    {
                        List<SelectionPoint> points = new List<SelectionPoint>();

                        for (int x = min.x; x <= max.x; x++)
                        {
                            for (int y = min.y; y <= max.y; y++)
                            {
                                for (int z = min.z; z <= max.z; z++)
                                {
                                    Vector3Int pos = new Vector3Int(x, y, z);
                                    if (o.voxelObjectData.GetVoxelAt(pos-o.basePoint).voxel != null)
                                    {
                                        points.Add(new SelectionPoint() { position = pos,normal = upPoint.Value.normal});
                                    }
                                }
                            }
                        }
                        hitPointDict.Add(o, points);
                    }
                }
            }


        }
    }



}
