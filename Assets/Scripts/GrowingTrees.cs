using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrowingTrees : MonoBehaviour
{
    [SerializeField] float timeToGrowForOnePhase;

    [SerializeField] GameObject[] growingPhases;

    int _currentGrowingPhase = 0;
    bool _canChangePhase = false;

    private void OnEnable()
    {
        GameManager.instance.ReduceTime(0.05f);
        StartGrow();
    }

    void Update()
    {
        if (_currentGrowingPhase == growingPhases.Length)
        {
            return;
        }

        if (_canChangePhase)
        {
            StartGrow();
        }
    }

    public void StartGrow()
    {
        UpdatePhase();
        StartCoroutine(GrowCor());
    }

    IEnumerator GrowCor()
    {
        _canChangePhase = false;
        yield return new WaitForSeconds(timeToGrowForOnePhase);

        if (_currentGrowingPhase + 1 <= growingPhases.Length - 1)
        {
            _currentGrowingPhase++;
            UpdatePhase();
            _canChangePhase = true;
        } else
        {
            UpdatePhase();
        }
    }

    private void UpdatePhase()
    {
        foreach (GameObject phase in growingPhases)
        {
            phase.gameObject.SetActive(false);
        }

        growingPhases[_currentGrowingPhase].gameObject.SetActive(true);
    }
}
