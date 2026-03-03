using DigitalRuby.LightningBolt;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfernoManager : MonoBehaviour
{
    public CardData cardData;
    public static InfernoManager Get;

    public int numberOfAttack = 2;    // Number of mines to throw
    public int attackDuration = 10; // Time interval between throws

    public LightningBoltScript lightningParticle;
    private LightningBoltScript spawnedLightning;
    public SpriteRenderer boundary;

    public Transform tower;
    public float attackRange = 1f;

    private void Awake()
    {
        Get = this;
    }

    void Start()
    {
        var data = cardData.cards[2];
        switch (data.savedData.level)
        {
            case 2:
                numberOfAttack = data.level2.value1;
                attackDuration = data.level2.value2;
                attackRange = 1.1f;
                break;
            case 3:
                numberOfAttack = data.level3.value1;
                attackDuration = data.level3.value2;
                attackRange = 1.4f;
                break;
            default:
                numberOfAttack = data.level1.value1;
                attackDuration = data.level1.value2;
                attackRange = 0.8f;
                break;
        }
        boundary.transform.localScale = Vector3.one * attackRange;
        // Start the throwing coroutine
        StartCoroutine(ThrowLightning());
        spawnedLightning = GameObject.Instantiate(lightningParticle.gameObject).GetComponent<LightningBoltScript>();
        spawnedLightning.transform.parent = transform;
    }
     
    IEnumerator ThrowLightning()
    {
        bool isThrown = true;
        while (true)
        {
            if (isThrown)
            {
                yield return new WaitForSeconds(attackDuration / numberOfAttack);
                isThrown = false;
            }
            else
            {
                yield return new WaitForSeconds(0.1f);
            }

            if (Player.Instance.isDead == false)
            {

                Enemy nearestEnemy = null;
                float nearestDistance = float.MaxValue;

                foreach ((Enemy, UnityPool) enemy in EnemyGenerator.Get.spawnedEnemy)
                {
                    float distance = Vector2.Distance(transform.position, enemy.Item1.transform.position);

                    // Check if the enemy is within the shooting radius and closer than the current nearest enemy
                    if (distance <= attackRange && distance < nearestDistance)
                    {
                        nearestEnemy = enemy.Item1;
                        nearestDistance = distance;
                    }
                }

                if (nearestEnemy != null)
                {
                    Vector3 target = nearestEnemy.transform.position;
                    target.z = -2;

                    spawnedLightning.StartPosition = tower.position;
                    spawnedLightning.EndPosition = target;
                    spawnedLightning.gameObject.SetActive(true);
                    AudioManager.Instance?.PlaySFXSound(AudioClipsType.InfernoSound);
                    nearestEnemy.TakeDamage(40, DamageType.Kill);
                    isThrown = true;

                    LeanTween.delayedCall(0.25f, () =>
                    {

                        spawnedLightning.StartPosition = tower.position;
                        spawnedLightning.EndPosition = tower.position;

                        LeanTween.delayedCall(0.05f, () => { spawnedLightning.gameObject.SetActive(false); });

                    });

                }
            }
        }
    }
}
