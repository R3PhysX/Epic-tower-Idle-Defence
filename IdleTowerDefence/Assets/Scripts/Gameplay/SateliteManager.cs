using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SateliteManager : MonoBehaviour
{
    public CardData cardData;
    public static SateliteManager Get;
    public Satelite satelitePrefab;

    public float radius;
    public int numberOfSatelite = 3;    // Number of mines to throw
    internal int spawnedSatelite = 0;    // Number of mines to throw
    public int spawnInterval = 10; // Time interval between throws

    public ParticleSystem blastParticle;

    private void Awake()
    {
        Get = this;
    }

    void Start()
    {
        Debug.Log("Spawn 222");
        var data = cardData.cards[3];
        switch (data.savedData.level)
        {
            case 2:
                numberOfSatelite = data.level2.value1;
                spawnInterval = data.level2.value2;
                break;
            case 3:
                numberOfSatelite = data.level3.value1;
                spawnInterval = data.level3.value2;
                break;
            default:
                numberOfSatelite = data.level1.value1;
                spawnInterval = data.level1.value2;
                break;
        }
        // Start the throwing coroutine
        StartCoroutine(SpawnSatelite());
    }

    IEnumerator SpawnSatelite()
    {
        while (true)
        {
            if (spawnedSatelite == 0)
            {
                yield return new WaitForSeconds(spawnInterval);
                if (Player.Instance.isDead == false)
                {
                    for (int i = 0; i < numberOfSatelite; i++)
                    {
                        
                        // Instantiate mine at the calculated position
                        var sateliteInstance = Instantiate(satelitePrefab.gameObject, transform).GetComponent<Satelite>();
                        sateliteInstance.transform.localPosition = Vector3.zero;
                        float radius = Random.Range(0.85f, 1.25f);
                        sateliteInstance.Set(radius);
                        spawnedSatelite += 1;
                        yield return new WaitForSeconds(0.3f);
                    }
                }
            }
            yield return null;
        }
    }
}
