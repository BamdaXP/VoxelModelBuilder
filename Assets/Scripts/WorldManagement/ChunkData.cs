using UnityEngine;
using System.Collections.Generic;
public class ChunkData
{
    public const int CHUNK_SIZE = 16;
    public const int CHUNK_HEIGHT = 256;
    public readonly Vector3[][] QUAD_VERTS = new Vector3[][] { 
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
    }

    public void GenerateMeshAndMats(out Mesh chunkMesh, out Material[] chunkMats)
    {
        Dictionary<VoxelInfo, CombineInstance> voxelSubInstanceDict = new Dictionary<VoxelInfo, CombineInstance>();
        Dictionary<VoxelInfo, List<Vector3>> voxelVertListDict = new Dictionary<VoxelInfo, List<Vector3>>();
        Dictionary<VoxelInfo, List<int>> voxelIndexListDict = new Dictionary<VoxelInfo, List<int>>();

        List<Material> voxelMatsList = new List<Material>();

        for (int x = 0; x < CHUNK_SIZE; x++)
        {
            for (int y = 0; y < CHUNK_HEIGHT; y++)
            {
                for (int z = 0; z < CHUNK_SIZE; z++)
                {
                    VoxelInfo v = voxelGrid.GetDataAt(x, y, z);
                    
                    if (v != null)
                    {
                        //Add new if not contains
                        if (!voxelSubInstanceDict.ContainsKey(v))
                        {
                            voxelSubInstanceDict.Add(v, new CombineInstance() { mesh = new Mesh()});
                            voxelVertListDict.Add(v, new List<Vector3>());
                            voxelIndexListDict.Add(v, new List<int>());
                            voxelMatsList.Add(v.material);
                        }
                            

                        Vector3 voxelPos = new Vector3(x, y, z);
                        //Find corresponding lists
                        List<Vector3> voxelVertsList = voxelVertListDict[v];
                        List<int> voxelIndsList = voxelIndexListDict[v];


                        //y-1
                        if (y - 1 < 0 || voxelGrid.GetDataAt(x, y - 1, z) == null)
                        {
                            foreach (Vector3 dp in QUAD_VERTS[0])
                            {
                                voxelIndsList.Add(voxelVertsList.Count);
                                voxelVertsList.Add(dp + voxelPos);
                            }
                        }
                        //y+1
                        if (y + 1 >= CHUNK_HEIGHT || voxelGrid.GetDataAt(x, y + 1, z) == null)
                        {
                            foreach (Vector3 dp in QUAD_VERTS[1])
                            {
                                voxelIndsList.Add(voxelVertsList.Count);
                                voxelVertsList.Add(dp + voxelPos);
                            }
                        }

                        //z-1
                        if (z - 1 < 0 || voxelGrid.GetDataAt(x, y, z - 1) == null)
                        {
                            foreach (Vector3 dp in QUAD_VERTS[2])
                            {
                                voxelIndsList.Add(voxelVertsList.Count);
                                voxelVertsList.Add(dp + voxelPos);
                            }
                        }
                        //z+1
                        if (z + 1 >= CHUNK_SIZE || voxelGrid.GetDataAt(x, y, z + 1) == null)
                        {
                            foreach (Vector3 dp in QUAD_VERTS[3])
                            {
                                voxelIndsList.Add(voxelVertsList.Count);
                                voxelVertsList.Add(dp + voxelPos);
                            }
                        }
                        //x-1
                        if (x - 1 < 0 || voxelGrid.GetDataAt(x - 1, y, z) == null)
                        {
                            foreach (Vector3 dp in QUAD_VERTS[4])
                            {
                                voxelIndsList.Add(voxelVertsList.Count);
                                voxelVertsList.Add(dp + voxelPos);
                            }
                        }
                        //x+1
                        if (x + 1 >= CHUNK_SIZE || voxelGrid.GetDataAt(x + 1, y, z) == null)
                        {
                            foreach (Vector3 dp in QUAD_VERTS[5])
                            {
                                voxelIndsList.Add(voxelVertsList.Count);
                                voxelVertsList.Add(dp + voxelPos);
                            }
                        }
                    }



                }
            }
        }

        //Setup combine instances in the dict
        foreach (VoxelInfo v in voxelSubInstanceDict.Keys)
        {
            voxelSubInstanceDict[v].mesh.SetVertices(voxelVertListDict[v]);
            voxelSubInstanceDict[v].mesh.SetIndices(voxelIndexListDict[v], MeshTopology.Quads, 0);
        }

        //Output voxelmesh,set false to get separated meshes
        chunkMesh = new Mesh();
        List<CombineInstance> cinsList = new List<CombineInstance>(voxelSubInstanceDict.Values);
        chunkMesh.CombineMeshes(cinsList.ToArray(),false);
        chunkMesh.Optimize();
        chunkMesh.RecalculateBounds();
        chunkMesh.RecalculateNormals();
        chunkMesh.RecalculateTangents();

        //Output voxel material array
        chunkMats = voxelMatsList.ToArray();

    }

}