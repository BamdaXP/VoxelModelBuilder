using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BluePrint : MonoBehaviour
{
    public WorldEditor editor;
    private MeshRenderer m_renderer;
    // Start is called before the first frame update
    void Start()
    {
        m_renderer = GetComponent<MeshRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        m_renderer.enabled = false;
        if (editor.selector.selecting)
        {
            m_renderer.enabled = true;
            switch (editor.mode)
            {
                case WorldEditor.EditorMode.Place:
                case WorldEditor.EditorMode.OnePointCube:
                case WorldEditor.EditorMode.PlaceBuffer:
                    transform.position = MathHelper.WorldPosToWorldIntPos(
                        editor.selector.selection.Position + editor.selector.selection.Normal / 2) + 
                        new Vector3(0.5f, 0.5f, 0.5f);//Mesh offset
                    break;
                default:
                    transform.position = MathHelper.WorldPosToWorldIntPos(
                        editor.selector.selection.Position - editor.selector.selection.Normal/2) + 
                        new Vector3(0.5f, 0.5f, 0.5f);//Mesh offset
                    break;
            }
        }
            


    }
}
