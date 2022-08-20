using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    [SerializeField] bool disableAfterInteration;

    [SerializeField] bool canChangePlayerPosition;
    [SerializeField] Transform futurePlayerPosition;

    [SerializeField] bool canEnableGameObject;
    [SerializeField] GameObject gameObjToEnable;

    [SerializeField] bool canDisableGameObject;
    [SerializeField] GameObject gameObjToDisable;

    [SerializeField] bool canReduceTime;
    [SerializeField] float timeToReducenInProcent;

    [SerializeField] bool canReduceCoins;
    [SerializeField] int coinsToReduce;

    [SerializeField] string textToAsk;

    [SerializeField] bool canPlayCustomAudioClip;
    [SerializeField] AudioClip clipToPlay;

    [SerializeField] bool canSetFadeTime;
    [SerializeField] float fadeDuration;

    public void Activate()
    {
        if (canSetFadeTime)
        {
            StartCoroutine(FadeAndWait(fadeDuration));
        } else
        {
            StartCoroutine(FadeAndWait(1f));
        }
        
        print("Interactable Object Activate!");
    }

    private IEnumerator FadeAndWait(float waitTime)
    {
        GameManager.instance.FadeIn();

        if (canPlayCustomAudioClip)
        {
            AudioManager.instance.PlayCustomSFX(clipToPlay);
        }

        yield return new WaitForSeconds(waitTime / 2);

        //«¿–≈‘¿ “Œ–»“‹ - «¿¬≈–Õ”“‹ »‘€ ¬ ‘”Õ ÷»»

        if (canReduceCoins)
        {
            GameManager.instance.ReduceCoins(coinsToReduce);
        }

        if (canDisableGameObject)
        {
            gameObjToDisable.SetActive(false);
        }

        if (canEnableGameObject)
        {
            gameObjToEnable.SetActive(true);
        }

        if (canChangePlayerPosition)
        {
            PlayerController.instance.transform.position = futurePlayerPosition.position;
        }

        if (canReduceTime)
        {
            GameManager.instance.ReduceTime(GameManager.instance.maxTime * (timeToReducenInProcent / 100));
        }

        yield return new WaitForSeconds(waitTime / 2);

        GameManager.instance.FadeOut();

        if (disableAfterInteration)
        {
            DisableThisIterationObject();
        }
    }

    public int GetRequiedCoint()
    {
        return coinsToReduce;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            PlayerController.instance.SetInteractableObjectActive(true, this, textToAsk);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            PlayerController.instance.SetInteractableObjectActive(false, null, "");
        }
    }

    private void DisableThisIterationObject()
    {
        this.gameObject.SetActive(false);
    }
}
