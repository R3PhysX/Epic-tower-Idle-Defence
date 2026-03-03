using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DeadLaserManager : MonoBehaviour
{
    public CardData cardData;
    public static DeadLaserManager Get;

    public GameObject fireMissleObject;
    public GameObject blastEffectObject;

    public int duration = 2;
    public int interval = 10;
    public float missleInterval = 0.1f;
    public int numberOfMissle;

    public List<Transform> spawnPositions;
    public List<Transform> targetPositions;

    private UnityPool misslePool;
    private UnityPool blastPool;

    public float shakeDuration;
    public float shakeMagnitude;

    private Vector3 originalPosition;
    private void Awake()
    {
        Get = this;
    }

    void Start()
    {
        originalPosition = GameplayManager.Get.gameplayPanel.transform.position;
        var data = cardData.cards[4];
        switch (data.savedData.level)
        {
            case 2:
                numberOfMissle = data.level2.value1;
                interval = data.level2.value2;
                break;
            case 3:
                numberOfMissle = data.level3.value1;
                interval = data.level3.value2;
                break;
            default:
                numberOfMissle = data.level1.value1;
                interval = data.level1.value2;
                break;
        }

        misslePool = new UnityPool(fireMissleObject.gameObject, 10, transform, true);
        blastPool = new UnityPool(blastEffectObject.gameObject, 10, transform, true);
        StartCoroutine(MissleRoutine());
    }

    private IEnumerator MissleRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(interval);
            if (Player.Instance.isDead)
            {
                StopAllCoroutines();
                break;
            }

            for (int i = 0; i < numberOfMissle; i++)
            {
                int ran = Random.Range(0, spawnPositions.Count);
                Vector3 spawnLocation = spawnPositions[ran].position;
                Vector3 targetLocation = targetPositions[ran].position;

                var missle = misslePool.Get<GameObject>(transform);
                missle.transform.position = spawnLocation;

                LeanTween.move(missle.gameObject, targetLocation, 2f).setOnComplete(() =>
                {
                    var blast = blastPool.Get<GameObject>(transform);
                    blast.transform.position = targetLocation;
                    ShakeObject();
                    LeanTween.delayedCall(1f, () => { misslePool.Add(missle.gameObject); });
                    LeanTween.delayedCall(3f, () => { blastPool.Add(blast.gameObject); });
                });

                yield return new WaitForSeconds(missleInterval);

                var list = new List<(Enemy, UnityPool)>(EnemyGenerator.Get.spawnedEnemy);
                if (i == ((int)((numberOfMissle * 80) / 100)))
                {
                    foreach ((Enemy, UnityPool) enemy in list)
                    {
                        enemy.Item1.TakeDamage(40, DamageType.Kill);
                    }
                }
            }

        }
    }

    private void ShakeObject()
    {
        LeanTween.cancel(GameplayManager.Get.gameplayPanel);

        // Create a shaking effect by moving the object to random positions within the shakeMagnitude
        LeanTween.moveLocal(GameplayManager.Get.gameplayPanel, originalPosition + Random.insideUnitSphere * shakeMagnitude, shakeDuration)
                 .setEase(LeanTweenType.easeShake)
                 .setOnComplete(() =>
                 {
                     // Reset the object's position after the shake
                     GameplayManager.Get.gameplayPanel.transform.localPosition = originalPosition;
                 });
    }
}