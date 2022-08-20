using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GirlMechanicHandler : MonoBehaviour
{
    public static GirlMechanicHandler instance;

    [SerializeField] GameObject spawnArea;

    [SerializeField] Transform girlSpawnAreaIfSuccesfulRelationship; 

    [SerializeField] Girl[] girls;

    [SerializeField] float howMuchTimeReduceIfUnsuccessful;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        RespawnGirl(girls[Random.Range(0, girls.Length)]);
    }

    public void TryRelationship(Girl girl, bool onlyRespawnGirl = false)
    {
        StartCoroutine(TryRelationshipCor(girl, onlyRespawnGirl));
    }

    IEnumerator TryRelationshipCor(Girl girl, bool onlyRespawnGirl = false)
    {
        GameManager.instance.FadeIn();

        yield return new WaitForSeconds(1f);

        if (onlyRespawnGirl)
        {
            Destroy(girl.gameObject);
            RespawnGirl(girls[Random.Range(0, girls.Length)]);
        } else
        {
            GameManager.instance.ReduceTime(howMuchTimeReduceIfUnsuccessful);

            if (Random.Range(0, 1f) >= 0.5f)
            {
                girl.ChangeState(2);
                girl.transform.position = girlSpawnAreaIfSuccesfulRelationship.position;

                PlayerController.instance.ShowLoveHeart();
            }
            else
            {
                Destroy(girl.gameObject);
                RespawnGirl(girls[Random.Range(0, girls.Length)]);

                PlayerController.instance.ShowBrokenHeart();
            }
        }

        GameManager.instance.FadeOut();
    }

    private void RespawnGirl(Girl girl)
    {
        var newGirl = Instantiate(girl, this.gameObject.transform);

        Vector3 randomPosition;
        float xRange = spawnArea.transform.localScale.x / 2;
        float yRange = spawnArea.transform.localScale.y / 2;

        float xPosition = Random.Range(transform.position.x - xRange, transform.position.x + xRange);
        float yPosition = Random.Range(transform.position.y - yRange, transform.position.y + yRange);

        randomPosition = new Vector3(xPosition, yPosition, 0);

        newGirl.transform.position = randomPosition; 
    }  
}
