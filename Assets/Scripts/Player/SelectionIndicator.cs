using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
public class SelectionIndicator : MonoBehaviour
{
    public VoxelSelector voxelSelector;

    private List<Vector3Int> data;
    private MeshFilter meshFilter;
    public void Awake()
    {
        data = new List<Vector3Int>();
        meshFilter = GetComponent<MeshFilter>();
    }
    private void Update()
    {
        foreach (var pair in voxelSelector.hitPointDict)
        {
            foreach (var v in pair.Value)
            {
                Vector3Int worldIntPos = v.position + pair.Key.basePoint;
                if (!data.Contains(worldIntPos))
                {
                    data.Add(v.position);
                }
            }
        }
        meshFilter.sharedMesh = GenerateMesh();
    }
    private Mesh GenerateMesh()
    {

        List<Vector3> totalVertices = new List<Vector3>();
        List<int> totalIndices = new List<int>();
        foreach (var v in data)
        {
            totalIndices.Add(totalIndices.Count);
            totalVertices.Add(v);
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
