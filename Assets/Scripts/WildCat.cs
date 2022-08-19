using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WildCat : MonoBehaviour
{
    [SerializeField] Sprite calmCat;
    [SerializeField] Sprite activeCat;

    private void Awake()
    {
        GetComponent<SpriteRenderer>().sprite = calmCat;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            GetComponent<SpriteRenderer>().sprite = activeCat;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            GetComponent<SpriteRenderer>().sprite = calmCat;
        }
    }
}
