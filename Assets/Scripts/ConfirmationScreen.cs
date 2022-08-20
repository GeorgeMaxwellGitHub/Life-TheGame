using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConfirmationScreen : MonoBehaviour
{
    public static ConfirmationScreen instance;

    [SerializeField] GameObject confirmationBubble;
    [SerializeField] Text textWithQuestion;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
    }

    public void Activate(string text)
    {
        confirmationBubble.SetActive(true);
        textWithQuestion.text = text;
        AudioManager.instance.PlayObjectsSFX(6, true);
    }

    public void Deactivate()
    {
        confirmationBubble.SetActive(false);
        textWithQuestion.text = "";
    }
}
