using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinesManager : MonoBehaviour
{
    public CardData cardData;
    public static MinesManager Get;
    public Mine mine;

    public int numberOfMines = 3;    // Number of mines to throw
    internal int minesSpawned = 0;    // Number of mines to throw
    public int throwInterval = 10; // Time interval between throws

    public ParticleSystem blastParticle;

    private void Awake()
    {
        Get = this;
    }

    void Start()
    {
        var data = cardData.cards[1];
        switch (data.savedData.level)
        {
            case 2:
                numberOfMines = data.level2.value1;
                throwInterval = data.level2.value2;
                break;
            case 3:
                numberOfMines = data.level3.value1;
                throwInterval = data.level3.value2;
                break;
            default:
                numberOfMines = data.level1.value1;
                throwInterval = data.level1.value2;
                break;
        }
        // Start the throwing coroutine
        StartCoroutine(ThrowMines());
    }

    IEnumerator ThrowMines()
    {
        while (true)
        {
            if (minesSpawned == 0)
            {
                yield return new WaitForSeconds(throwInterval);

                if (Player.Instance.isDead == false)
                {
                    // Instantiate mines
                    for (int i = 0; i < numberOfMines; i++)
                    {
                        // Calculate a random angle
                        float angle = Random.Range(0f, 360f);

                        // Calculate position around the circle
                        Vector3 targetLocation = transform.position + Quaternion.Euler(0f, 0f, angle) * Vector3.right * (Random.Range(1f, Constants.Get.BoundaryRadius - 0.2f));

                        // Instantiate mine at the calculated position
                        var mineInstance = Instantiate(mine.gameObject, transform).GetComponent<Mine>();
                        mineInstance.transform.position = targetLocation;
                        mineInstance.Set(throwInterval - 1);
                        minesSpawned += 1;
                    }
                }
            }
            yield return null;
        }
    }
}