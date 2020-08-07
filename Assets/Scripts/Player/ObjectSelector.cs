using UnityEngine;
using System.Collections.Generic;
public class ObjectSelector : MonoBehaviour
{
    public HitPointReader hitPointReader;
    public List<ObjectComponent> selectedObjects;
    private void Awake()
    {
        selectedObjects = new List<ObjectComponent>();
    }
    private void Start()
    {
        selectedObjects.Add(WorldDataManager.Instance.ActiveWorld.GetVoxelObject(0));
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (!Input.GetKeyDown(KeyCode.LeftControl))
            {
                selectedObjects.Clear();
            }
            ObjectComponent[] os = 
                WorldDataManager.Instance.ActiveWorld.GetVoxelObjectsAt(
                    hitPointReader.hitPoint.position - hitPointReader.hitPoint.normal / 2);
            selectedObjects.AddRange(os);

            Debug.Log("Selected Object " + selectedObjects);
        }
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            var last = selectedObjects[0];
            selectedObjects.Clear();
            selectedObjects.Add(WorldDataManager.Instance.ActiveWorld.GetNextObject(last));
            Debug.Log("Selected Object " + selectedObjects);
        }
    }
}
