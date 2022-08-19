using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AskScreen : MonoBehaviour
{
    public static AskScreen instance;

    [SerializeField] GameObject askBubble;
    [SerializeField] Text askText;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
    }

    public void Activate(string text)
    {
        askBubble.SetActive(true);
        askText.text = text;
        AudioManager.instance.PlayObjectsSFX(6, true);
    }

    public void Deactivate()
    {
        askBubble.SetActive(false);
        askText.text = "";
    }
}
