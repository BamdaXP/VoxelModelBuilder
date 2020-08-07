using UnityEngine;
public class ObjectManipulator : MonoBehaviour
{
    public ObjectSelector objectSelector;

    private void Update()
    {
        //Copy
        if (Input.GetKeyDown(KeyCode.C))
        {
            foreach (var o in objectSelector.selectedObjects)
            {
                WorldDataManager.Instance.ActiveWorld.CopyObject(o);
            }
        }

        //Merge
        if (objectSelector.selectedObjects.Count > 1)
        {
            if (Input.GetKey(KeyCode.M))
            {
                if (Input.GetKeyDown(KeyCode.Alpha1))
                {
                    for (int i = 1; i < objectSelector.selectedObjects.Count; i++)
                    {
                        WorldDataManager.Instance.ActiveWorld.MergeTwoObjects(
                            objectSelector.selectedObjects[0],
                            objectSelector.selectedObjects[i],
                            WorldData.MergeType.Or);
                    }
                }
                if (Input.GetKeyDown(KeyCode.Alpha2))
                {
                    for (int i = 1; i < objectSelector.selectedObjects.Count; i++)
                    {
                        WorldDataManager.Instance.ActiveWorld.MergeTwoObjects(
                            objectSelector.selectedObjects[0],
                            objectSelector.selectedObjects[i],
                            WorldData.MergeType.And);
                    }
                }
                if (Input.GetKeyDown(KeyCode.Alpha3))
                {
                    for (int i = 1; i < objectSelector.selectedObjects.Count; i++)
                    {
                        WorldDataManager.Instance.ActiveWorld.MergeTwoObjects(
                            objectSelector.selectedObjects[0],
                            objectSelector.selectedObjects[i],
                            WorldData.MergeType.Not);
                    }
                }
            }
        }

        //Move
        Vector3Int delta = new Vector3Int();
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            delta.x = -1;
        }
        else
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            delta.x = 1;
        }
        else
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            delta.z = -1;
        }
        else
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            delta.z = 1;
        }
        else
        if (Input.GetKeyDown(KeyCode.PageDown))
        {
            delta.y = -1;
        }
        else
        if (Input.GetKeyDown(KeyCode.PageUp))
        {
            delta.y = 1;
        }
        foreach (var o in objectSelector.selectedObjects)
        {
            o.basePoint += delta;
        }
    }
}