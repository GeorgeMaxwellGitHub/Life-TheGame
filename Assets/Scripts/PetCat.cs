using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PetCat : MonoBehaviour
{
    public static PetCat instance;

    [SerializeField] Sprite innactiveCatSprite;
    [SerializeField] Sprite activeCatSprite;

    [SerializeField] Sprite loveCatSprite;

    bool _canShowLove = true;

    [SerializeField] Animator showLoveAnimator;

    void Start()
    {
        instance = this;
        GetComponent<SpriteRenderer>().sprite = innactiveCatSprite;
    }

    public void ShowLove()
    {
        if (!_canShowLove)
        {
            return;
        } else
        {
            StartCoroutine(ShowLoveCor());
        }
    }

    IEnumerator ShowLoveCor()
    {
        _canShowLove = false;

        GetComponent<SpriteRenderer>().sprite = loveCatSprite;
        showLoveAnimator.SetBool("ShowLove", true);

        AudioManager.instance.PlayObjectsSFX(2);

        yield return new WaitForSeconds(1f);

        showLoveAnimator.SetBool("ShowLove", false);

        GetComponent<SpriteRenderer>().sprite = innactiveCatSprite;

        _canShowLove = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player" && _canShowLove)
        {
            PlayerController.instance.SetPetCatOptionActive(true);
            GetComponent<SpriteRenderer>().sprite = activeCatSprite;

            if (Random.Range(0, 1f) <= 0.2f)
            {
                GetComponent<SpriteRenderer>().flipX = !GetComponent<SpriteRenderer>().flipX;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player" && _canShowLove)
        {
            PlayerController.instance.SetPetCatOptionActive(false);
            GetComponent<SpriteRenderer>().sprite = innactiveCatSprite;
        }
    }
}
