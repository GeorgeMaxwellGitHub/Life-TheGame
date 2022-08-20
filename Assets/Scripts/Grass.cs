using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grass : MonoBehaviour
{
    [SerializeField] ParticleSystem particles;

    [SerializeField] bool canActivateButterfly;
    [SerializeField] Butterfly butterfly;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            AudioManager.instance.PlayObjectsSFX(0);

            GetComponent<Animator>().SetBool("Wiggle", true);

            if (canActivateButterfly)
            {
                butterfly.Fly();
            }
            particles.Play();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            GetComponent<Animator>().SetBool("Wiggle", false);
        }
    }
}
