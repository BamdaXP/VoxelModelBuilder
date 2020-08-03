using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class HUD : MonoBehaviour
{
    public WorldEditor editor;

    public TextMeshProUGUI modeText;
    public TextMeshProUGUI voxelText;
    public TextMeshProUGUI integerText;
    public TextMeshProUGUI selectionText;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        modeText.text = editor.mode.ToString();

        string t = "";
        foreach (var v in editor.voxelArgs)
        {
            t += v.name + "; ";
        }
        voxelText.text = t;

        t = "";
        foreach (var i in editor.integerArgs)
        {
            t += i + "; ";
        }
        integerText.text = t;

        t = "";
        foreach (var p in editor.selectionArgs)
        {
            t += MathHelper.WorldPosToWorldIntPos(p.Position-p.Normal/2);
            t += ";";
        }
        selectionText.text = t;
    }
}
