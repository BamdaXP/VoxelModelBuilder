using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseIndicator : MonoBehaviour
{
    public HitPointReader hitPointReader;

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
        if (hitPointReader.hitting)
        {
            m_renderer.enabled = true;
            transform.position = MathHelper.WorldPosToWorldIntPos(
                hitPointReader.hitPoint.position - hitPointReader.hitPoint.normal / 2) +
                new Vector3(0.5f, 0.5f, 0.5f);//Mesh offset
        }
    }
}
