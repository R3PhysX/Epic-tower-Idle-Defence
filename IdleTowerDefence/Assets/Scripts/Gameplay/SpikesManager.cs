using DigitalRuby.LightningBolt;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class SpikeData
{
    public int hit;
    public SpriteRenderer spikeImage;
}

public class SpikesManager : MonoBehaviour
{
    public CardData cardData;
    public static SpikesManager Get;

    public List<SpikeData> spikes;
    public List<Sprite> spikeSprites;

    public ParticleSystem blastParticle;

    public int noOfHit = 2;
    public float respawnInterval = 10f;
    
    private void Awake()
    {
        Get = this;
    }

    private void Start()
    {
        var data = cardData.cards[5];
        switch (data.savedData.level)
        {
            case 2:
                noOfHit = data.level2.value1;
                respawnInterval = data.level2.value2;
                break;
            case 3:
                noOfHit = data.level3.value1;
                respawnInterval = data.level3.value2;
                break;
            default:
                noOfHit = data.level1.value1;
                respawnInterval = data.level1.value2;
                break;
        }

        StopAllCoroutines();

        spikes.ForEach(x => x.spikeImage.gameObject.SetActive(false));
        StartCoroutine(SpawnSpikes());
    }

    IEnumerator SpawnSpikes()
    {        
        yield return new WaitForSeconds(respawnInterval);
        foreach (var item in spikes)
        {
            item.hit = noOfHit;
            item.spikeImage.sprite = spikeSprites[noOfHit];
            item.spikeImage.gameObject.SetActive(true);

            yield return new WaitForSeconds(0.05f);
        }

        StartCoroutine(CheckForEnemy());
    }

    IEnumerator CheckForEnemy()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.1f);
            List<(Enemy, SpikeData)> nearestEnemy = new List<(Enemy, SpikeData)>();

            var activeSpike = spikes.Where(x => x.hit >= 0).ToList();
            foreach (var spike in activeSpike)
            {
                foreach ((Enemy, UnityPool) enemy in EnemyGenerator.Get.spawnedEnemy)
                {
                    float distance = Vector2.Distance(spike.spikeImage.transform.position, enemy.Item1.transform.position);
                    if (distance <= 0.1f)
                    {
                        nearestEnemy.Add((enemy.Item1, spike));
                    }
                }
            }
            if (nearestEnemy.Count > 0)
            {
                foreach (var (enemy, spike) in nearestEnemy)
                {
                    if (spike.hit >= 0)
                    {
                        Vector3 pos = spike.spikeImage.transform.position;
                        pos.z = -2;
                        var obj = GameObject.Instantiate(blastParticle.gameObject, pos, Quaternion.identity);

                        enemy.TakeDamage(40, DamageType.Kill);
                        spike.hit--;
                        if (spike.hit >= 0)
                            spike.spikeImage.sprite = spikeSprites[spike.hit];
                        else
                        {
                            spike.spikeImage.gameObject.SetActive(false);
                            StartCoroutine(RespawnSpike(spike));
                        }

                        AudioManager.Instance?.PlaySFXSound(AudioClipsType.BombSound);
                    }
                }
            }
        }
    }

    IEnumerator RespawnSpike(SpikeData spike)
    {
        yield return new WaitForSeconds(respawnInterval);

        spike.hit = noOfHit;
        spike.spikeImage.sprite = spikeSprites[noOfHit];
        spike.spikeImage.gameObject.SetActive(true);
    }

}