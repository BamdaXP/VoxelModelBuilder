using System.Collections.Generic;
using UnityEngine;


public static class VoxelInfoLibrary
{
    private static Dictionary<string, VoxelInfo> NameLibrary;
    static VoxelInfoLibrary()
    {
        NameLibrary = new Dictionary<string, VoxelInfo>();
        VoxelInfo[] assets = Resources.LoadAll<VoxelInfo>("VoxelAssets/");
        foreach (var t in assets)
        {
            NameLibrary.Add(t.name, t);
        }

    }
    public static VoxelInfo GetTile(string name)
    {
        return NameLibrary[name];
    }
}
