using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolManager : Singleton<ToolManager>
{
    public VoxelPlacer voxelPlacer;
    public FaceStretcher faceStretcher;
    public ObjectManipulator objectManipulator;
    public enum ToolMode
    {
        PlaceVoxel,
        FaceStretch,
        ObjectManipulation
    }
    public ToolMode mode;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            mode = ToolMode.PlaceVoxel;
        }
        else if (Input.GetKeyDown(KeyCode.F2))
        {
            mode = ToolMode.FaceStretch;
        }
        else if (Input.GetKeyDown(KeyCode.F3))
        {
            mode = ToolMode.ObjectManipulation;
        }

        switch (mode)
        {
            case ToolMode.PlaceVoxel:
                voxelPlacer.gameObject.SetActive(true);
                faceStretcher.gameObject.SetActive(false);
                objectManipulator.gameObject.SetActive(false);
                break;
            case ToolMode.FaceStretch:
                voxelPlacer.gameObject.SetActive(false);
                faceStretcher.gameObject.SetActive(true);
                objectManipulator.gameObject.SetActive(false);
                break;
            case ToolMode.ObjectManipulation:
                voxelPlacer.gameObject.SetActive(false);
                faceStretcher.gameObject.SetActive(false);
                objectManipulator.gameObject.SetActive(true);
                break;
            default:
                break;
        }

        
    }
}
