using UnityEngine;
using System.Collections.Generic;
public class WorldDataManager : Singleton<WorldDataManager>
{
    private List<WorldData> m_availableWorlds;
    public WorldData ActiveWorld { get; private set; }
    private void Start()
    {
        m_availableWorlds = new List<WorldData>();
    }

    public void CreateNewWorld(string name)
    {
        WorldData world = new WorldData(name);
        m_availableWorlds.Add(world);
    }
    public void ActivateWorld(string name)
    {
        ActiveWorld = m_availableWorlds.Find(x => x.name == name);
    }

    public WorldData[] GetAvailableWorlds()
    {
        return m_availableWorlds.ToArray();
    }

}