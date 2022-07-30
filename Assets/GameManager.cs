using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [SerializeField] float maxTime;
    float timeLeft;

    [SerializeField] Transform fogSpriteMask;
    [SerializeField] float minFogMaskRadious = 6;
    [SerializeField] float maxFogMaskRadious = 23;

    //Temp. Delete after adding the slider
    [SerializeField] Text timeLeftText;

    void Start()
    {
        instance = this;
        InitiacteGame();
    }

    private void InitiacteGame()
    {
        timeLeft = maxTime;
    }

    public void ReduceTime(float value)
    {
        timeLeft -= value;

        if (timeLeft <= 0)
        {
            timeLeft = 0;
            EndGame();
        }
    }

    private void EndGame()
    {
        print("Game Ends");
    }

    void Update()
    {
        timeLeftText.text = timeLeft.ToString();
    }

    public void IncreaseFogMaskRadius(float value)
    {
        if (fogSpriteMask.localScale.x < minFogMaskRadious)
        {
            throw new System.Exception("Minum fog radius is " + minFogMaskRadious + ", but current fog radius is " + fogSpriteMask.localScale.x + "!!!");
        }

        if (fogSpriteMask.localScale.x >= maxFogMaskRadious)
        {
            fogSpriteMask.localScale = new Vector3(maxFogMaskRadious, maxFogMaskRadious, fogSpriteMask.localScale.z);
            return;
        }

        fogSpriteMask.localScale = new Vector3(fogSpriteMask.localScale.x + value,
                                                fogSpriteMask.localScale.y + value,
                                                fogSpriteMask.localScale.z);
    }
}
