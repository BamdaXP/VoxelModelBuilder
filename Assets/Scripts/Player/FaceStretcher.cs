using UnityEngine;
using System.Collections.Generic;
using System.Drawing;

public class FaceStretcher : MonoBehaviour
{
    public VoxelSelector voxelSelector;
    public SelectionIndicator selectionIndicator;
    public int stretchResult;
    private Vector3? m_downMousePoint;
    private Vector3? m_upMousePoint;

    public Dictionary<ObjectComponent,List<Vector3Int>> stretchedPointDict;
    private void Awake()
    {
        stretchedPointDict = new Dictionary<ObjectComponent, List<Vector3Int>>();
        stretchResult = 0;
        m_downMousePoint = null;
        m_upMousePoint = null;
    }
    private void Update()
    {
        //Stretch
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            m_upMousePoint = null;
            m_downMousePoint = Input.mousePosition;
        }
        if (Input.GetKey(KeyCode.Mouse1))
        {
            if (m_downMousePoint != null && selectionIndicator.data.Count > 0 && selectionIndicator.voxelSelector.normal != null)
            {
                var normal = new Vector3Int(
                    (int)voxelSelector.normal.Value.x,
                    (int)voxelSelector.normal.Value.y,
                    (int)voxelSelector.normal.Value.z);


                m_upMousePoint = Input.mousePosition;
                Camera camera = Camera.main;
                Vector3 p1 = camera.ScreenToWorldPoint(new Vector3(m_upMousePoint.Value.x, m_upMousePoint.Value.y, camera.nearClipPlane));
                Vector3 p2 = camera.ScreenToWorldPoint(new Vector3(m_downMousePoint.Value.x, m_downMousePoint.Value.y, camera.nearClipPlane));

                float result = Vector3.Dot(p1 - p2, normal);
                stretchResult = (int)(result * 50);
            }

        }


        //Make stretched data
        if (voxelSelector.normal != null)
        {
            var normal = new Vector3Int(
                (int)voxelSelector.normal.Value.x,
                (int)voxelSelector.normal.Value.y,
                (int)voxelSelector.normal.Value.z);

            stretchedPointDict.Clear();

            foreach (var pair in voxelSelector.selectionPointDict)
            {
                List<Vector3Int> points = new List<Vector3Int>();
                //Add 
                if (stretchResult > 0)
                {
                    for (int i = 0; i <= stretchResult; i++)
                    {
                        foreach (var p in pair.Value)
                        {
                            //Stretch out
                            points.Add(p + i * normal);
                        }
                    }
                }
                //Substract
                else
                {
                    for (int i = stretchResult; i <= 0; i++)
                    {
                        foreach (var p in pair.Value)
                        {
                            //Stretch out
                            points.Add(p + i * normal);
                        }
                    }
                }
                stretchedPointDict.Add(pair.Key, points);
            }
        }

        //Apply
        if (Input.GetKeyUp(KeyCode.Mouse1) && voxelSelector.normal != null)
        {
            var normal = new Vector3Int(
                (int)voxelSelector.normal.Value.x,
                (int)voxelSelector.normal.Value.y,
                (int)voxelSelector.normal.Value.z);
            //Add
            if (stretchResult > 0)
            {
                foreach (var pair in voxelSelector.selectionPointDict)
                {
                    foreach (var p in pair.Value)
                    {
                        Voxel v = pair.Key.voxelObjectData.GetVoxelAt(p);
                        for (int i = 1; i <= stretchResult; i++)
                        {
                            pair.Key.voxelObjectData.SetVoxelAt(p + normal * i, v);
                        }
                    }
                    pair.Key.UpdateObjectMesh();
                }
            }
            //Substract
            else
            {
                foreach (var pair in voxelSelector.selectionPointDict)
                {
                    foreach (var p in pair.Value)
                    {
                        for (int i = stretchResult; i <= 0; i++)
                        {
                            //Delete voxel
                            pair.Key.voxelObjectData.DeleteVoxelAt(p + normal * i);
                        }
                    }
                    pair.Key.UpdateObjectMesh();
                }
            }
            
            voxelSelector.selectionPointDict.Clear();
            stretchResult = 0;
        }
    }
}