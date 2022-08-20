using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rock : MonoBehaviour
{
    [SerializeField] Animator attachetBeatle;
    public void Flip()
    {
        if (!GetComponent<Animator>().GetBool("FlipRock"))
        {
            AudioManager.instance.PlayObjectsSFX(3);
            GetComponent<Animator>().SetBool("FlipRock", true);
            attachetBeatle.SetTrigger("MoveBeatle");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            if (!GetComponent<Animator>().GetBool("FlipRock"))
            {
                PlayerController.instance.SetFlipRockOptionActive(true, this);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            PlayerController.instance.SetFlipRockOptionActive(false, null);
        }
    }
}
