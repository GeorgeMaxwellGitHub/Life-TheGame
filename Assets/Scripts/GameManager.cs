using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    //Cached component references
    [SerializeField] SpriteRenderer fadeBlackScreen;

    [SerializeField] Transform fogSpriteMask;

    [SerializeField] int currentCoins;
    [SerializeField] Text curentCoinsUiText;

    [SerializeField] Animator welcomeToGameTextAnimator;
    [SerializeField] SpriteRenderer welcomeToGameScreen;

    [SerializeField] Animator endGameTextAnimator;

    //Config
    [SerializeField] float maxGameTime;
    float _gameTimeLeft;

    [SerializeField] float fadeToBlackSpeed;

    [SerializeField] float minFogMaskRadious = 6;
    [SerializeField] float maxFogMaskRadious = 23;

    [Range(1f, 0f)] [SerializeField] float persantageOfTimeWhenFogIsMax;
    [Range(1f, 0f)] [SerializeField] float persantageOfTimeWhenFogIsStartDecrease;

    [Range(0f, 1f)] [SerializeField] float minimumSaturationValue;

    //States
    bool _isFaded = false;
    bool _isFadeIn;
    bool _isFadeOut;
    bool _isGameEnded;
    bool _isGameStarted;

    //TEST
    [SerializeField] bool skipIntro;

    void Start()
    {
        instance = this;

        //Cursor.visible = false; 
        curentCoinsUiText.text = GetCurrentCoins().ToString();

        if (skipIntro)
        {
            _gameTimeLeft = maxGameTime;
            _isGameStarted = true;
            return;
        }

        StartCoroutine(StartGameCor(2f));
    }

    void FixedUpdate()
    {
        FadeToBlackController();
        FogLogicHandler();
        SaturationLogicHandler();

        curentCoinsUiText.text = GetCurrentCoins().ToString();
    }

    //MAIN GAME MANAGER METHODS

    IEnumerator StartGameCor(float fadingDuration)
    {
        welcomeToGameScreen.color = new Color(welcomeToGameScreen.color.r, welcomeToGameScreen.color.g, welcomeToGameScreen.color.b, 1);
        AudioManager.instance.PlayLifeMusic();

        yield return new WaitForSeconds(3f);

        welcomeToGameTextAnimator.SetBool("Active", true);

        yield return new WaitForSeconds(21f);

        float currentAlphaValue = 0;
        float start = 1;

        while (currentAlphaValue < fadingDuration)
        {
            currentAlphaValue += Time.deltaTime;

            welcomeToGameScreen.color = new Color(welcomeToGameScreen.color.r,
                welcomeToGameScreen.color.g,
                welcomeToGameScreen.color.b,
                Mathf.Lerp(start, 0, currentAlphaValue / fadingDuration));

            yield return null;
        }

        _gameTimeLeft = maxGameTime;
        _isGameStarted = true;
    }

    public bool GameStartedStatus()
    {
        return _isGameStarted;
    }

    public bool GetGameEndsStatus()
    {
        return _isGameEnded;
    }

    private void EndGame()
    {
        _isGameEnded = true;

        StartCoroutine(EndGameCor());
        print("Game Ends");
    }

    IEnumerator EndGameCor()
    {
        if (AudioManager.instance.IsBridgeMusicPlay())
        {
            AudioManager.instance.StopBridgeMusic(false);
        }

        fadeToBlackSpeed = 0.001f;

        AudioManager.instance.PlayLifeMusic();

        yield return new WaitForSeconds(33.5f);

        PlayerController.instance.CompletelyStopPlayer();

        yield return new WaitForSeconds(3f);

        FadeIn();

        yield return new WaitForSeconds(3f);

        endGameTextAnimator.SetBool("Run", true);

        yield return new WaitForSeconds(10f);
    }


    //FADING MANAGER
    private void FadeToBlackController()
    {
        if (_isFadeOut)
        {
            if (fadeBlackScreen.color.a <= 0)
            {
                _isFaded = false;
                _isFadeOut = false;
            }

            fadeBlackScreen.color = new Color(fadeBlackScreen.color.r, fadeBlackScreen.color.g, fadeBlackScreen.color.b, fadeBlackScreen.color.a - fadeToBlackSpeed);
        }

        if (_isFadeIn)
        {
            if (fadeBlackScreen.color.a >= 1)
            {
                _isFaded = true;
                _isFadeIn = false;
            }

            fadeBlackScreen.color = new Color(fadeBlackScreen.color.r, fadeBlackScreen.color.g, fadeBlackScreen.color.b, fadeBlackScreen.color.a + fadeToBlackSpeed);
        }
    }

    public bool GetFadingState()
    {
        return _isFaded;
    }

    public void FadeIn()
    {
        if (_isFadeOut == true)
        {
            throw new System.Exception("FadeOut process is already has been started.");
        }

        if (_isFaded == true)
        {
            throw new System.Exception("Scene is already faded");
        }

        _isFadeIn = true;
    }

    public void FadeOut()
    {
        if (_isFadeOut == true)
        {
            throw new System.Exception("FadeIn process is already has been started. ");
        }

        if (_isFaded == false)
        {
            throw new System.Exception("Scene is already doesn't faded");
        }

        print("Starts Fade Out");
        _isFadeOut = true;
    }

    //SATURATION MANAGER
    private void SaturationLogicHandler()
    {
        if (persantageOfTimeWhenFogIsStartDecrease >= persantageOfTimeWhenFogIsMax)
        {
            throw new System.Exception("Error: fog begins to decrease before it reaches its maximum!");
        }

        if (!_isGameStarted)
        {
            BWEffect.instance.SetIntensity(0);
            return;
        }

        float formula = (1 - _gameTimeLeft) * minimumSaturationValue;

        BWEffect.instance.SetIntensity(formula);
    }

    //TIME MANAGER
    public void ReduceTime(float value)
    {
        if (_isGameEnded)
        {
            return;
        }

        _gameTimeLeft -= value;

        if (_gameTimeLeft <= 0)
        {
            _gameTimeLeft = 0;
            EndGame();
        }
    }

    public float GetMaxTime()
    {
        return maxGameTime;
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

        if (_gameTimeLeft >= persantageOfTimeWhenFogIsMax)
        {
            //IDK how I get this formula, but it make a direct dependence between current time and fog sprite mask radius, normilize time and fog radius variables.
            float formula1 = (((1 - _gameTimeLeft) / (maxGameTime - persantageOfTimeWhenFogIsMax)) * (maxFogMaskRadious - minFogMaskRadious)) + minFogMaskRadious;

            fogSpriteMask.localScale = new Vector3(formula1, formula1, fogSpriteMask.localScale.z);
        }

        if (_gameTimeLeft >= 0 && _gameTimeLeft <= persantageOfTimeWhenFogIsStartDecrease)
        {
            float formula2 = (((_gameTimeLeft) / (persantageOfTimeWhenFogIsStartDecrease - 0)) * (maxFogMaskRadious - minFogMaskRadious)) + minFogMaskRadious;

            fogSpriteMask.localScale = new Vector3(formula2, formula2, fogSpriteMask.localScale.z);
        }
    }

    //COIN MANAGER
    public void AddCoins(int value)
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

    public int GetCurrentCoins()
    {
        return currentCoins;
    }
}
