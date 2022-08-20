using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Girl : MonoBehaviour
{
    //This index indicates is Player in love with Girl. Variable 1 indicates that player doesn't, but 2 sets ability to show love
    int _index = 1;

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
                _index = 1;
                break;
            case 2:
                GetComponent<SpriteRenderer>().sprite = state2Sprite;
                _index = 2;
                break;
            case 3:
                GetComponent<SpriteRenderer>().sprite = state3Sprite;
                _index = 3;
                break;
            default:
                break;
        }

        _index = stateNumber;
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

        ChangeState(2);
        showLoveAnimator.SetBool("ShowLove", false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            if (_index == 1)
            {
                PlayerController.instance.SetTryRelationshipActive(true, this);

            } else if (_index == 2) 
            {
                PlayerController.instance.SetCanMakeLoveWithGirlActive(true, this);
            }
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            if (_index == 1)
            {
                PlayerController.instance.SetTryRelationshipActive(false ,null);

            }
            else if (_index == 2)
            {
                PlayerController.instance.SetCanMakeLoveWithGirlActive(false, null);
            }
        }
    }
}
