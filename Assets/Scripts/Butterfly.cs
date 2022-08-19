using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Butterfly : MonoBehaviour
{
    public void Fly()
    {
        GetComponent<Animator>().SetTrigger("Fly");
    }
}
