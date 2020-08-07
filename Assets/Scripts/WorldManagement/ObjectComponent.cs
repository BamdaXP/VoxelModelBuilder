using UnityEngine;
public class ObjectComponent : MonoBehaviour
{
    public Vector3Int basePoint;
    //Local Coordinates data
    public ObjectData voxelObjectData;

    public MeshFilter meshFilter;
    public MeshRenderer meshRenderer;
    public MeshCollider meshCollider;

    private void Update()
    {
        transform.position = basePoint;
    }
    public bool IsNearVoxel(Vector3 worldPosition)
    {
        var localPos = MathHelper.WorldPosToWorldIntPos(worldPosition)-basePoint;
        foreach (var p in voxelObjectData.VoxelDataDict.Keys)
        {
            if ((localPos - p).magnitude <= 1.01f)
                return true;
        }
        return false;
    }
    public void UpdateObjectMesh()
    {
        Mesh mesh;
        Material[] mats;
        voxelObjectData.GenerateMeshAndMats(out mesh, out mats);
        meshFilter.mesh = mesh;
        meshCollider.sharedMesh = mesh;
        meshRenderer.materials = mats;
    }
}

