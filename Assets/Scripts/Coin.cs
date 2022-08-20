using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    public void Pickup()
    {
        AudioManager.instance.PlayObjectsSFX(4, true);
        GameManager.instance.AddCoin(1);
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            PlayerController.instance.SetCanPickupCoinActive(true, this);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            PlayerController.instance.SetCanPickupCoinActive(false, null);
        }
    }
}
