using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
public class SelectionIndicator : MonoBehaviour
{
    public VoxelSelector voxelSelector;

    private List<Vector3Int> m_data;
    private MeshFilter m_meshFilter;
    public void Awake()
    {
        m_data = new List<Vector3Int>();
        m_meshFilter = GetComponent<MeshFilter>();
    }
    private void Update()
    {
        m_data.Clear();
        foreach (var pair in voxelSelector.hitPointDict)
        {
            foreach (var v in pair.Value)
            {
                Vector3Int worldIntPos = v.position + pair.Key.basePoint;
                //Do not need repeated points
                if (!m_data.Contains(worldIntPos))
                {
                    m_data.Add(worldIntPos);
                }
            }
        }
        m_meshFilter.sharedMesh = GenerateMesh();
    }
    private Mesh GenerateMesh()
    {

        List<Vector3> totalVertices = new List<Vector3>();
        List<int> totalIndices = new List<int>();


        foreach (var v in m_data)
        {
            for (int i = 0; i < 6; i++)
            {
                if (!m_data.Contains(v + ObjectData.NORMALS[i]))
                {
                    foreach (var p in ObjectData.QUAD_VERTS[i])
                    {
                        totalIndices.Add(totalIndices.Count);
                        totalVertices.Add(v+p);
                    }
                }

            }
        }

        Mesh mesh = new Mesh();
        mesh.SetVertices(totalVertices);
        mesh.SetIndices(totalIndices, MeshTopology.Quads, 0);
        mesh.Optimize();
        mesh.RecalculateNormals();
        mesh.RecalculateTangents();

        return mesh;
    }
}
