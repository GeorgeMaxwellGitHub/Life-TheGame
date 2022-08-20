using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Girl : MonoBehaviour
{
    int index = 1;

    [SerializeField] Sprite state1Sprite;
    [SerializeField] Sprite state2Sprite;
    [SerializeField] Sprite state3Sprite;

    [SerializeField] Animator showLoveAnimator;

    public void ChangeState(int stateNumber)
    {
        if (stateNumber > 3 || stateNumber < 1)
        {
            throw new System.Exception("stateNumber variable must equals 1, 2 or 3!!");
        }

        switch (stateNumber)
        {
            case 1:
                GetComponent<SpriteRenderer>().sprite = state1Sprite;
                index = 1;
                break;
            case 2:
                GetComponent<SpriteRenderer>().sprite = state2Sprite;
                index = 2;
                break;
            case 3:
                GetComponent<SpriteRenderer>().sprite = state3Sprite;
                index = 3;
                break;
            default:
                break;
        }

        index = stateNumber;
    }

    public void MakeLove()
    {
        StartCoroutine(MakeLoveCor());
    }

    IEnumerator MakeLoveCor()
    {
        ChangeState(3);
        showLoveAnimator.SetBool("ShowLove", true);
        AudioManager.instance.PlayObjectsSFX(3);
        yield return new WaitForSeconds(1f);
        showLoveAnimator.SetBool("ShowLove", false);
        ChangeState(2);
    }



    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            if (index == 1)
            {
                PlayerController.instance.SetTryRelationshipActive(true, this);

            } else if (index == 2) 
            {
                PlayerController.instance.SetCanMakeLoveWithGirlActive(true, this);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            if (index == 1)
            {
                PlayerController.instance.SetTryRelationshipActive(false ,null);

            }
            else if (index == 2)
            {
                PlayerController.instance.SetCanMakeLoveWithGirlActive(false, null);
            }
        }
    }


}
