using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Rendering;

public class ChunkData
{
    public const int CHUNK_SIZE = 16;
    public const int CHUNK_HEIGHT = 256;
    public static readonly Vector3[][] QUAD_VERTS = new Vector3[][] { 
        //y-1
        new Vector3[] {
            new Vector3(0,0,1),
            new Vector3(0,0,0),
            new Vector3(1,0,0),
            new Vector3(1,0,1)
        },
        //y+1
        new Vector3[] {
            new Vector3(0,1,0),
            new Vector3(0,1,1),
            new Vector3(1,1,1),
            new Vector3(1,1,0)
        },

        //z-1
        new Vector3[] {
            new Vector3(1,1,0),
            new Vector3(1,0,0),
            new Vector3(0,0,0),
            new Vector3(0,1,0)
        },
        //z+1
        new Vector3[] {
            new Vector3(0,1,1),
            new Vector3(0,0,1),
            new Vector3(1,0,1),
            new Vector3(1,1,1)
        },
        //x-1
        new Vector3[] {
            new Vector3(0,1,0),
            new Vector3(0,0,0),
            new Vector3(0,0,1),
            new Vector3(0,1,1)
        },
        //x+1
        new Vector3[] {
            new Vector3(1,1,1),
            new Vector3(1,0,1),
            new Vector3(1,0,0),
            new Vector3(1,1,0)
        }
    };
    public GridData3D<VoxelInfo> voxelGrid;
    public ChunkData()
    {
        voxelGrid = new GridData3D<VoxelInfo>(CHUNK_SIZE, CHUNK_HEIGHT, CHUNK_SIZE);
        VoxelInfo v = VoxelInfoLibrary.GetVoxel("Stone");
        for (int x = 0; x < CHUNK_SIZE; x++)
        {
            for (int z = 0; z < CHUNK_SIZE; z++)
            {
                voxelGrid.SetDataAt(x, 64, z, v);
            }
        }
    }
   
    public void GenerateMeshAndMats(out Mesh chunkMesh, out Material[] chunkMats)
    {
        Dictionary<VoxelInfo, List<Vector3>> voxelVertListDict = new Dictionary<VoxelInfo, List<Vector3>>();

        List<Material> voxelMatsList = new List<Material>();

        for (int x = 0; x < CHUNK_SIZE; x++)
        {
            for (int y = 64; y < 65; y++)
            {
                for (int z = 0; z < CHUNK_SIZE; z++)
                {
                    VoxelInfo v = voxelGrid.GetDataAt(x, y, z);
                    
                    if (v != null)
                    {
                        //Add new if not contains
                        if (!voxelVertListDict.ContainsKey(v))
                        {
                            voxelVertListDict.Add(v, new List<Vector3>());
                            voxelMatsList.Add(v.material);
                        }
                            

                        Vector3 voxelPos = new Vector3(x, y, z);
                        //Find corresponding lists
                        List<Vector3> voxelVertsList = voxelVertListDict[v];


                        //y-1
                        if (y - 1 < 0 || voxelGrid.GetDataAt(x, y - 1, z) == null)
                        {
                            foreach (Vector3 dp in QUAD_VERTS[0])
                            {
                                voxelVertsList.Add(dp + voxelPos);
                            }
                        }
                        //y+1
                        if (y + 1 >= CHUNK_HEIGHT || voxelGrid.GetDataAt(x, y + 1, z) == null)
                        {
                            foreach (Vector3 dp in QUAD_VERTS[1])
                            {
                                voxelVertsList.Add(dp + voxelPos);
                            }
                        }

                        //z-1
                        if (z - 1 < 0 || voxelGrid.GetDataAt(x, y, z - 1) == null)
                        {
                            foreach (Vector3 dp in QUAD_VERTS[2])
                            {
                                voxelVertsList.Add(dp + voxelPos);
                            }
                        }
                        //z+1
                        if (z + 1 >= CHUNK_SIZE || voxelGrid.GetDataAt(x, y, z + 1) == null)
                        {
                            foreach (Vector3 dp in QUAD_VERTS[3])
                            {
                                voxelVertsList.Add(dp + voxelPos);
                            }
                        }
                        //x-1
                        if (x - 1 < 0 || voxelGrid.GetDataAt(x - 1, y, z) == null)
                        {
                            foreach (Vector3 dp in QUAD_VERTS[4])
                            {
                                voxelVertsList.Add(dp + voxelPos);
                            }
                        }
                        //x+1
                        if (x + 1 >= CHUNK_SIZE || voxelGrid.GetDataAt(x + 1, y, z) == null)
                        {
                            foreach (Vector3 dp in QUAD_VERTS[5])
                            {
                                voxelVertsList.Add(dp + voxelPos);
                            }
                        }
                    }



                }
            }
        }

        //Combine verts and indices and setup submesh descriptors
        List<Vector3> totalVertices = new List<Vector3>();
        List<int> totalIndices = new List<int>();
        List<SubMeshDescriptor> subMeshDescList = new List<SubMeshDescriptor>();
        foreach (var verts in voxelVertListDict.Values)
        {
            //Add a descriptor for each vert list
            subMeshDescList.Add(new SubMeshDescriptor(totalIndices.Count, verts.Count, MeshTopology.Quads));
            //Append to total lists
            foreach (var vert in verts)
            {
                totalIndices.Add(totalIndices.Count);
                totalVertices.Add(vert);
            }
        }

        //Output voxelmesh
        chunkMesh = new Mesh();
        chunkMesh.SetVertices(totalVertices);//Set vertex buffer
        chunkMesh.SetIndexBufferParams(totalIndices.Count, IndexFormat.UInt32);//Set index buffer format and data,since the max count is 65565*4*6 so must be int32
        chunkMesh.SetIndexBufferData(totalIndices, 0, 0, totalIndices.Count);

        //Set submesh description
        int i = 0;
        foreach (var desc in subMeshDescList)
        {
            chunkMesh.SetSubMesh(i, desc);
            i++;
        }
        //optimization
        chunkMesh.Optimize();
        chunkMesh.RecalculateNormals();
        chunkMesh.RecalculateTangents();


        //Output voxel material array
        chunkMats = voxelMatsList.ToArray();

    }

}