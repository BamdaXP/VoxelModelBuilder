
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HUD : MonoBehaviour
{
    public TextMeshProUGUI toolText;

    // Update is called once per frame
    void Update()
    {
        toolText.text = ToolManager.Instance.mode.ToString();
    }
}
