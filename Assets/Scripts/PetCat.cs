using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PetCat : MonoBehaviour
{
    public static PetCat instance;

    [SerializeField] Sprite inncativeCat;
    [SerializeField] Sprite activeCat;

    [SerializeField] Sprite loveCat;

    bool canShowLove = true;

    [SerializeField] Animator showLoveAnimator;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        GetComponent<SpriteRenderer>().sprite = inncativeCat;
    }

    public void ShowLove()
    {
        if (!canShowLove)
        {
            return;
        } else
        {
            StartCoroutine(ShowLoveCor());
        }
    }

    IEnumerator ShowLoveCor()
    {
        canShowLove = false;

        GetComponent<SpriteRenderer>().sprite = loveCat;
        showLoveAnimator.SetBool("ShowLove", true);

        AudioManager.instance.PlayObjectsSFX(2);

        yield return new WaitForSeconds(1f);

        showLoveAnimator.SetBool("ShowLove", false);

        GetComponent<SpriteRenderer>().sprite = inncativeCat;

        canShowLove = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player" && canShowLove)
        {
            PlayerController.instance.SetPetCatOptionActive(true);
            GetComponent<SpriteRenderer>().sprite = activeCat;

            if (Random.Range(0, 1f) <= 0.2f)
            {
                GetComponent<SpriteRenderer>().flipX = !GetComponent<SpriteRenderer>().flipX;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player" && canShowLove)
        {
            PlayerController.instance.SetPetCatOptionActive(false);
            GetComponent<SpriteRenderer>().sprite = inncativeCat;
        }
    }

}
