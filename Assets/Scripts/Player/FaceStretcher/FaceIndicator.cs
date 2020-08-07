using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
public class FaceIndicator : MonoBehaviour
{
    public FaceSelector voxelSelector;
    public FaceStretcher faceStretcher;

    public List<Vector3Int> data;

    private MeshFilter m_meshFilter;
    public void Awake()
    {
        data = new List<Vector3Int>();
        m_meshFilter = GetComponent<MeshFilter>();
    }
    private void Update()
    {
        data.Clear();
        foreach (var pair in faceStretcher.stretchedPointDict)
        {
            foreach (var v in pair.Value)
            {
                Vector3Int p = v + pair.Key.basePoint;


                //Do not need repeated points
                if (!data.Contains(p))
                {
                    data.Add(p);
                }
            }
        }
        m_meshFilter.sharedMesh = GenerateMesh();
    }
    private Mesh GenerateMesh()
    {

        List<Vector3> totalVertices = new List<Vector3>();
        List<int> totalIndices = new List<int>();


        foreach (var v in data)
        {
            for (int i = 0; i < 6; i++)
            {
                Vector3Int p = v + ObjectData.NORMALS[i];
                if (!data.Contains(p))
                {
                    foreach (var dp in ObjectData.QUAD_VERTS[i])
                    {
                        totalIndices.Add(totalIndices.Count);
                        totalVertices.Add(v+dp);
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
