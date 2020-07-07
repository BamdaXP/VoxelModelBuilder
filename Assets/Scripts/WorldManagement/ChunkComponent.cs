using UnityEngine;
[RequireComponent(typeof(MeshFilter),typeof(MeshRenderer))]
public class ChunkComponent : MonoBehaviour
{
    //The data responding to the chunk
    public ChunkData chunkData;

    public MeshFilter voxelMeshFilter;
    public MeshRenderer voxelMeshRenderer;
    public MeshCollider voxelCollider;
    public void UpdateChunk()
    {
        Mesh voxelMesh;
        Material[] mats;
        chunkData.GenerateMeshAndMats(out voxelMesh,out mats);

        voxelMeshFilter.mesh = voxelMesh;
        voxelMeshRenderer.materials = mats;
        voxelCollider.sharedMesh = voxelMesh;   
    }
}