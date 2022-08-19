using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bird : MonoBehaviour
{
    [SerializeField] float birdFlyTime;
    [SerializeField] Animator birdAnimator;

    float birdFlyDuration;
    bool cantStartFlyEncounter;

    bool playerInArea;

    private void Update()
    {
        if (playerInArea)
        {
            birdFlyDuration = 0;
            return;
        }

        if (cantStartFlyEncounter)
        {
            birdFlyDuration += Time.deltaTime;
            if (birdFlyDuration >= birdFlyTime)
            {
                birdAnimator.SetBool("BirdFly", false);
                cantStartFlyEncounter = false;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            playerInArea = true;

            cantStartFlyEncounter = true;
            birdFlyDuration = 0;

            if (!birdAnimator.GetBool("BirdFly"))
            {
                AudioManager.instance.PlayObjectsSFX(1);
                birdAnimator.SetBool("BirdFly", true);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            playerInArea = false;
        }
    }
}
