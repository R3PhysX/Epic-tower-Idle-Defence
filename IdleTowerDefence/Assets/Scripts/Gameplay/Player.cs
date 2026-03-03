//using Lofelt.NiceVibrations;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class TownSprites
{
    public List<Sprite> townSprite;
}

public class Player : MonoBehaviour
{
    public static Player Instance;

    [SerializeField] private Image hitEffect;
    [SerializeField] private Transform shockWave;
    [SerializeField] private List<Color> ringColor;
    [SerializeField] private List<TownSprites> townSprites;

    [SerializeField] private SpriteRenderer town;

    [Header("UI")]
    [SerializeField] private Image healthBar;
    [SerializeField] private TMP_Text healthText;

    [SerializeField] internal Image shieldhealthBar;
    [SerializeField] internal TMP_Text shieldhealthText;

    [SerializeField] private BulletManager bulletManager;

    private float nextShootTime;

    [SerializeField] private float playerBoundaryOffset = 0.6f;
    [SerializeField] private SpriteRenderer playerBoundary;

    [SerializeField] internal float currentHealth;
    internal bool isDead = false;

    internal ShieldManager shieldManager;
    [SerializeField] private ParticleSystem smokeParticle;
    [SerializeField] internal ParticleSystem upgradeParticle;

    private bool isRevived;

    private void Awake()
    {
        Instance = this;
    }

    private void OnEnable()
    {
        EventManager.AddListener(EventID.Player_HealthUpdate, OnHealthUpdate);
        EventManager.AddListener(EventID.Player_HealthAdd, OnHealthAdd);
    }

    private void OnDisable()
    {
        EventManager.RemoveListener(EventID.Player_HealthUpdate, OnHealthUpdate);
        EventManager.RemoveListener(EventID.Player_HealthAdd, OnHealthAdd);
    }

    private void Start()
    {
        LeanTween.delayedCall(0.1f, Init);
    }

    private void Init()
    {
        currentHealth = Constants.Get.PlayerHealth;
        EventManager.TriggerEvent(EventID.Player_HealthUpdate, null);

        DrawBoundaryCircle();
        StartCoroutine(StartHealthRegeneration());
    }

    private IEnumerator StartHealthRegeneration()
    {
        while (true)
        {
            if (currentHealth < Constants.Get.PlayerHealth)
            {
                yield return new WaitForSeconds(0.5f);
                currentHealth += (Constants.Get.HealthRegenPerSecond / 2f);
                currentHealth = Mathf.Clamp(currentHealth, 0f, Constants.Get.PlayerHealth);
                EventManager.TriggerEvent(EventID.Player_HealthUpdate, null);

                yield return new WaitForSeconds(0.5f);
                currentHealth += (Constants.Get.HealthRegenPerSecond / 2f);
                currentHealth = Mathf.Clamp(currentHealth, 0f, Constants.Get.PlayerHealth);
                EventManager.TriggerEvent(EventID.Player_HealthUpdate, null);
            }
            yield return null;
        }
    }

    [ContextMenu("DrawBoundaryCircle")]
    internal void DrawBoundaryCircle()
    {
        float scale = Constants.Get.BoundaryRadius / playerBoundaryOffset;
        playerBoundary.transform.localScale = Vector3.one * scale;
        playerBoundary.color = ringColor[ActiveGameData.Instance.currentSelectedWorld];
    }

    private bool doubleShotFired;

    private void Update()
    {
        // Check if it's time to shoot
        if (Time.time >= nextShootTime && isDead == false)
        {
            // Find the nearest enemy within the shooting radius
            Enemy nearestEnemy = FindNearestEnemy();

            // If there is an enemy in range, shoot at it
            if (nearestEnemy != null)
            {
                bulletManager.ShootAt(nearestEnemy);

                // Set the next shoot time based on the shoot interval
                nextShootTime = Time.time + Constants.Get.ShootInterval;

                if (doubleShotFired == false && GameplayManager.Get.randomGenerator.Next(0, 100) < Constants.Get.DoubleShotChance)
                {
                    nextShootTime = Time.time + 0.05f;
                    doubleShotFired = true;
                }
                else
                {
                    doubleShotFired = false;
                }
            }
        }
    }

    private Enemy FindNearestEnemy()
    {
        Enemy nearestEnemy = null;
        float nearestDistance = float.MaxValue;

        foreach ((Enemy, UnityPool) enemy in EnemyGenerator.Get.spawnedEnemy)
        {
            float distance = Vector2.Distance(transform.position, enemy.Item1.transform.position);

            // Check if the enemy is within the shooting radius and closer than the current nearest enemy
            if (distance <= Constants.Get.BoundaryRadius && distance < nearestDistance)
            {
                nearestEnemy = enemy.Item1;
                nearestDistance = distance;
            }
        }

        return nearestEnemy;
    }

    private bool hitEffectRunning = false;

    internal void DoDamage(float damage)
    {
        AudioManager.Instance?.PlaySFXSound(AudioClipsType.TownDamage);
        damage -= (damage * Constants.Get.DamageResistancePercentage / 100);

        if (shieldManager != null && shieldManager.currentShieldHealth > 0)
        {
            var diff = shieldManager.currentShieldHealth - damage;
            if (diff >= 0)
            {
                shieldManager.DoDamageToShield(damage);
                //if (ActiveGameData.Instance.saveData.VibrateEffect == 1)
                //    HapticPatterns.PlayPreset(HapticPatterns.PresetType.Warning);
                return;
            }
            else if (diff < 0)
            {
                damage -= shieldManager.currentShieldHealth;
                shieldManager.DoDamageToShield(shieldManager.currentShieldHealth);
            }
        }

        if (hitEffectRunning == false)
        {
            hitEffectRunning = true;
            LeanTween.cancel(hitEffect.gameObject);
            var color = hitEffect.color;
            color.a = 1f;
            hitEffect.color = color;
            hitEffect.gameObject.SetActive(true);

            LeanTween.value(hitEffect.gameObject, (val) =>
            {
                var color = hitEffect.color;
                color.a = val;
                hitEffect.color = color;
            }, 1f, 0f, 0.6f).setEase(LeanTweenType.easeOutSine)
    .setOnComplete(() => { hitEffect.gameObject.SetActive(false); hitEffectRunning = false; });

        }

        currentHealth -= damage;
        EventManager.TriggerEvent(EventID.Player_HealthUpdate, null);
        if (isDead == false && currentHealth <= 0)
        {
            Die();
        }
        else
        {
            //if (ActiveGameData.Instance.saveData.VibrateEffect == 1)
            //    HapticPatterns.PlayPreset(HapticPatterns.PresetType.Warning);
        }
    }

    private void Die()
    {
        Debug.Log("Death at: "  + GameplayManager.Get.currentWave);
        GameAnalyticsManager.Instance.NewProgressonEventGA(GameAnalyticsSDK.GAProgressionStatus.Fail,ActiveGameData.Instance.currentSelectedWorld.ToString(),GameplayManager.Get.currentWave.ToString());
        isDead = true;
        GameplayManager.Get.isGameplayStarted = false;
        Time.timeScale = 1f;
        //if (ActiveGameData.Instance.saveData.VibrateEffect == 1)
        //    HapticPatterns.PlayPreset(HapticPatterns.PresetType.Failure);
        // ScreenManager.Get.GetScreen<GameScreen>().Hide();
        // ScreenManager.Get.GetScreen<AttributeScreen>().Hide();

        AudioManager.Instance?.PlaySFXSound(AudioClipsType.WarEnd);

        if (isRevived == false && GameplayManager.Get.currentWave > 5)
        {
            isRevived = true;
            LeanTween.delayedCall(2f, () => { ScreenManager.Get.GetScreen<ReviveScreen>().Show(); });
        }
        else
        {
            LeanTween.delayedCall(2f, () => { ScreenManager.Get.GetScreen<GameOverScreen>().Show(); });
        }
    }

    private void OnHealthUpdate(object arg)
    {
        healthBar.fillAmount = currentHealth / Constants.Get.PlayerHealth;
        healthText.text = currentHealth.ToString("0") + "/" + Constants.Get.PlayerHealth.ToString("0");
        SetTownSprite();
    }

    private void OnHealthAdd(object arg)
    {
        float healthToAdd = (float)arg;
        currentHealth += healthToAdd;
        healthBar.fillAmount = currentHealth / Constants.Get.PlayerHealth;
        healthText.text = currentHealth.ToString("0") + "/" + Constants.Get.PlayerHealth.ToString("0");
    }

    private void SetTownSprite()
    {
        if (currentHealth >= 0 && currentHealth <= Constants.Get.PlayerHealth / 4)
        {
            //0-25%
            town.sprite = townSprites[ActiveGameData.Instance.currentSelectedWorld].townSprite[3];
            smokeParticle.Play();
        }
        else if (currentHealth > Constants.Get.PlayerHealth / 4 && currentHealth <= Constants.Get.PlayerHealth / 2f)
        {
            //25-50%
            town.sprite = townSprites[ActiveGameData.Instance.currentSelectedWorld].townSprite[2];
            smokeParticle.Play();
        }
        else if (currentHealth > Constants.Get.PlayerHealth / 2 && currentHealth <= Constants.Get.PlayerHealth / 1.3333f)
        {
            //50-75
            town.sprite = townSprites[ActiveGameData.Instance.currentSelectedWorld].townSprite[1];
            smokeParticle.Stop();
        }
        else
        {
            // 100
            town.sprite = townSprites[ActiveGameData.Instance.currentSelectedWorld].townSprite[0];
            smokeParticle.Stop();
        }
    }

    internal void ShockWaveEffect()
    {
        shockWave.transform.localScale = Vector3.zero;
        shockWave.gameObject.SetActive(true);
        LeanTween.scale(shockWave.gameObject, Vector3.one * 30f, 1f).setOnComplete(() =>
        {
            shockWave.gameObject.SetActive(false);
        });
    }
}