using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [SerializeField] SpriteRenderer fadeScreen;
    [Range(0.001f, 0.01f)][SerializeField] float fadeSpeed;
    bool isFaded = false;
    bool isFadeIn;
    bool isFadeOut;

    [Header("Time Variable")]
    public float maxTime;
    float timeLeft;

    [Header("Fog Variables")]
    [SerializeField] Transform fogSpriteMask;
    [SerializeField] float minFogMaskRadious = 6;
    [SerializeField] float maxFogMaskRadious = 23;

    [Range(1f, 0f)] [SerializeField] float persantageOfTimeWhenFogIsMax;
    [Range(1f, 0f)] [SerializeField] float persantageOfTimeWhenFogIsStartDecrease;

    [Header("Saturation Variables")]
    [Range(0f, 1f)] [SerializeField] float minimumSaturationValue;
    float maximumSaturationValue = 0;


    [Header("CoinManager Variables")]
    [SerializeField] int currentCoins;

    //Temp. Delete after adding the slider
    [SerializeField] Text timeLeftText;

    [SerializeField] Animator thxTextAnimator;

    bool gameIsEnded;

    [SerializeField] Animator helloText;

    bool gameStarted;

    [SerializeField] SpriteRenderer helloScreen;

    public int GetCurrentCoins()
    {
        return currentCoins;
    }

    void Start()
    {
        instance = this;
        InitiacteGame();

        //AudioManager.instance.PlayLifeMusic();
    }

    public bool GetGameEndsStatus()
    {
        return gameIsEnded;
    }

    void Update()
    {
        //timeLeftText.text = timeLeft.ToString();
        FadedController();
        FogLogicHandler();
        SaturationLogicHandler();

        timeLeftText.text = GetCurrentCoins().ToString();
    }

    private void FadedController()
    {
        if (isFadeOut)
        {
            if (fadeScreen.color.a <= 0)
            {
                print("Fade Out Complete");
                isFaded = false;
                isFadeOut = false;
            }

            fadeScreen.color = new Color(fadeScreen.color.r, fadeScreen.color.g, fadeScreen.color.b, fadeScreen.color.a - fadeSpeed);
        }

        if (isFadeIn)
        {
            if (fadeScreen.color.a >= 1)
            {
                print("Fade In Complete");
                isFaded = true;
                isFadeIn = false;
            }

            fadeScreen.color = new Color(fadeScreen.color.r, fadeScreen.color.g, fadeScreen.color.b, fadeScreen.color.a + fadeSpeed);
        }
    }

    public bool GetFadingState()
    {
        return isFaded;
    }

    public void FadeIn()
    {
        if (isFadeOut == true)
        {
            throw new System.Exception("FadeOut process is already has been started.");
        }

        if (isFaded == true)
        {
            throw new System.Exception("Scene is already faded");
        }

        print("Starts Fade In");
        isFadeIn = true;
    }

    public void FadeOut()
    {
        if (isFadeOut == true)
        {
            throw new System.Exception("FadeIn process is already has been started. ");
        }

        if (isFaded == false)
        {
            throw new System.Exception("Scene is already doesn't faded");
        }

        print("Starts Fade Out");
        isFadeOut = true;
    }

    private void InitiacteGame()
    {
        StartCoroutine(StartGameCor(2f));
    }

    IEnumerator StartGameCor(float duration)
    {
        helloScreen.color = new Color(helloScreen.color.r, helloScreen.color.g, helloScreen.color.b, 1);
        AudioManager.instance.PlayLifeMusic();

        yield return new WaitForSeconds(3f);

        helloText.SetBool("Active", true);

        yield return new WaitForSeconds(21f);

        float currentTime = 0;
        float start = 1;
        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            helloScreen.color = new Color(helloScreen.color.r, helloScreen.color.g, helloScreen.color.b, Mathf.Lerp(start, 0, currentTime / duration));
            yield return null;
        }

        timeLeft = maxTime;

        gameStarted = true;
    }


    public bool GameStartedStatus()
    {
        return gameStarted;
    }



    private void EndGame()
    {
        gameIsEnded = true;

        StartCoroutine(EndGameCor());
        print("Game Ends");
    }

    IEnumerator EndGameCor()
    {
        if (AudioManager.instance.IsBridgeMusicPlay())
        {
            AudioManager.instance.StopBridgeMusic(false);
        }

        fadeSpeed = 0.001f;

        AudioManager.instance.PlayLifeMusic();
        yield return new WaitForSeconds(33.5f);
        PlayerController.instance.StopPlayer();
        yield return new WaitForSeconds(3f);
        FadeIn();
        yield return new WaitForSeconds(3f);
        thxTextAnimator.SetBool("Run", true);

        yield return new WaitForSeconds(10f);
    }

    //SATURATION MANAGER

    private void SaturationLogicHandler()
    {
        if (persantageOfTimeWhenFogIsStartDecrease >= persantageOfTimeWhenFogIsMax)
        {
            throw new System.Exception("Error: fog begins to decrease before it reaches its maximum!");
        }


        float formula = (1 - timeLeft) * minimumSaturationValue;
        //float formula = (((1 - timeLeft) / (maxTime)) * (maxFogMaskRadious - minFogMaskRadious)) + minFogMaskRadious;

        //print(formula);
        BWEffect.instance.SetIntensity(formula);
    }

    //TIME MANAGER
    public void ReduceTime(float value)
    {
        if (gameIsEnded)
        {
            return;
        }

        timeLeft -= value;

        if (timeLeft <= 0)
        {
            timeLeft = 0;
            EndGame();
        }
    }

    //FOG MANAGER

    public float GetMaxFogMaskRadious()
    {
        return maxFogMaskRadious;
    }

    public float GetMinFogMaskRadious()
    {
        return minFogMaskRadious;
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

    private void FogLogicHandler()
    {
        if (persantageOfTimeWhenFogIsStartDecrease >= persantageOfTimeWhenFogIsMax)
        {
            throw new System.Exception("Error: fog begins to decrease before it reaches its maximum!");
        }

        if (timeLeft >= persantageOfTimeWhenFogIsMax)
        {
            float formula1 = (((1 - timeLeft) / (maxTime - persantageOfTimeWhenFogIsMax)) * (maxFogMaskRadious - minFogMaskRadious)) + minFogMaskRadious;

            //print(formula1);
            fogSpriteMask.localScale = new Vector3(formula1, formula1, fogSpriteMask.localScale.z);
        }

        if (timeLeft >= 0 && timeLeft <= persantageOfTimeWhenFogIsStartDecrease)
        {
            float formula2 = (((timeLeft) / (persantageOfTimeWhenFogIsStartDecrease - 0)) * (maxFogMaskRadious - minFogMaskRadious)) + minFogMaskRadious;

            //print(formula2);
            fogSpriteMask.localScale = new Vector3(formula2, formula2, fogSpriteMask.localScale.z);
        }
    }

    //COIN MANAGER

    public void AddCoin(int value)
    {
        currentCoins += value;
    }

    public void ReduceCoins(int value)
    {
        if (value > currentCoins)
        {
            throw new System.Exception("Coins that you try to reduce is bigger than player actually have");
        }

        currentCoins -= value;
    }
}
