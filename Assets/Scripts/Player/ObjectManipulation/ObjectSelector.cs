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
        //click to select pointing object 
        if (Input.GetKeyDown(KeyCode.Mouse0) && hitPointReader.hitting)
        {
            //holding contorl to addon to exsiting ones
            if (!Input.GetKey(KeyCode.LeftControl))
            {
                selectedObjects.Clear();
            }

            ObjectComponent[] os =
                WorldDataManager.Instance.ActiveWorld.GetVoxelObjectsAt(
                    hitPointReader.hitPoint.position - hitPointReader.hitPoint.normal / 2);


            //holding shift to only get first one
            if (Input.GetKey(KeyCode.LeftShift))
            {
                if (!selectedObjects.Contains(os[0]))
                {
                    selectedObjects.Add(os[0]);
                }

            }
            else
            {
                foreach (var o in os)
                {
                    if (!selectedObjects.Contains(os[0]))
                    {
                        selectedObjects.Add(o);
                    }
                }
            }


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
