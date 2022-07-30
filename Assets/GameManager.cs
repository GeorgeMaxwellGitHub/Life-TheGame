using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [SerializeField] float maxTime;
    float timeLeft;

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
}
