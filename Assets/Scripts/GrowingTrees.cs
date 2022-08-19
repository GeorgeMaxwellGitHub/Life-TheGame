using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrowingTrees : MonoBehaviour
{
    [SerializeField] float timeToGrowForOnePhase;

    [SerializeField] GameObject[] phases;

    int currentPhase = 0;
    bool canChangePhase = false;

    private void OnEnable()
    {
        GameManager.instance.ReduceTime(0.05f);
        StartGrow();
    }

    // Update is called once per frame
    void Update()
    {
        if (currentPhase == phases.Length)
        {
            return;
        }

        if (canChangePhase)
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
        canChangePhase = false;
        yield return new WaitForSeconds(timeToGrowForOnePhase);

        if (currentPhase + 1 <= phases.Length - 1)
        {
            currentPhase++;
            UpdatePhase();
            canChangePhase = true;
        } else
        {
            UpdatePhase();
        }
    }

    private void UpdatePhase()
    {
        foreach (GameObject phase in phases)
        {
            phase.gameObject.SetActive(false);
        }

        phases[currentPhase].gameObject.SetActive(true);
    }
}
