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
        print(waitTime);

        GameManager.instance.FadeIn();

        if (canPlayCustomAudioClip)
        {
            AudioManager.instance.PlayCustomSFX(clipToPlay);
        }

        yield return new WaitForSeconds(waitTime / 2);

        ReduceCoinHandler();
        DisableGameObjHandler();
        EnableGameObjHandler();
        ChangePlayerPositioHandler();
        ReduceTimeHandler();

        yield return new WaitForSeconds(waitTime / 2);

        GameManager.instance.FadeOut();
        DisableAfterInteractionHandler();
    }

    private void DisableAfterInteractionHandler()
    {
        if (disableAfterInteration)
        {
            gameObject.SetActive(false);
        }
    }

    private void ReduceTimeHandler()
    {
        if (canReduceTime)
        {
            GameManager.instance.ReduceTime(GameManager.instance.GetMaxTime() * (timeToReducenInProcent / 100));
        }
    }

    private void ChangePlayerPositioHandler()
    {
        if (canChangePlayerPosition)
        {
            PlayerController.instance.transform.position = futurePlayerPosition.position;
        }
    }

    private void EnableGameObjHandler()
    {
        if (canEnableGameObject)
        {
            gameObjToEnable.SetActive(true);
        }
    }

    private void DisableGameObjHandler()
    {
        if (canDisableGameObject)
        {
            gameObjToDisable.SetActive(false);
        }
    }

    private void ReduceCoinHandler()
    {
        if (canReduceCoins)
        {
            GameManager.instance.ReduceCoins(coinsToReduce);
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
}
