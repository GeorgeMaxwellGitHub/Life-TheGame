using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bird : MonoBehaviour
{
    [SerializeField] float birdFlyTime;
    [SerializeField] Animator birdAnimator;

    float _birdFlyCounter;
    bool _cantStartFlyEncounter;
    bool _isBirdCurrentlyFly;

    bool _isPlayerInArea;

    private void Update()
    {
        if (_isPlayerInArea)
        {
            _birdFlyCounter = 0;
            return;
        }

        if (_cantStartFlyEncounter)
        {
            _birdFlyCounter += Time.deltaTime;

            if (_birdFlyCounter >= birdFlyTime)
            {
                birdAnimator.SetBool("BirdFly", false);
                _cantStartFlyEncounter = false;

                _isBirdCurrentlyFly = false;
            }
        }
    }

    private void BirdLogicHandlerWhenPlayerEnterTheArea()
    {
        _isPlayerInArea = true;

        _cantStartFlyEncounter = true;
        _birdFlyCounter = 0;

        if (!_isBirdCurrentlyFly)
        {
            AudioManager.instance.PlayObjectsSFX(1);
            birdAnimator.SetBool("BirdFly", true);

            _isBirdCurrentlyFly = true;
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            BirdLogicHandlerWhenPlayerEnterTheArea();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            _isPlayerInArea = false;
        }
    }
}
