using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.EventSystems.EventTrigger;

public enum DamageType
{
    Normal,
    Crit,
    DeadHit,
    Stun,
    Kill
}

[System.Serializable]
public enum EnemyType
{
    BasicEnemy,
    MidEnemy,
    BulletEnemy,
    Boss,
    EliteEnemy
}

[System.Serializable]
public enum BuffType
{
    None,
    ShieldAOE,
    DefenseI,
    DefenseII,
    DeathSplit,
    Slow,
    SlowPercentage
}

[System.Serializable]
public enum AuraType
{
    None,
    SlowAura,
    DefenseAura,
    DefenseII,
    Frenzy,
    SlowAuraPercentage
}

[System.Serializable]
public class Enemy : MonoBehaviour
{
    public EnemyType enemyType;

    public SpriteRenderer enemySprite; // Set the target (e.g., player) in the Inspector
    private Transform target; // Set the target (e.g., player) in the Inspector
    public float moveSpeed = 0.75f;
    public float attackDistance = 1.5f; // Distance to start attacking
    public float attackInterval = 0.65f;   // Time between attacks
    public float maxHealth = 100;
    private int silverCoinForKill = 0;
    public float damage = 1;

    public float defense = 0;
    public float damageRed = 0;

    private float currentHealth;
    private float nextAttackTime;

    private bool isAttacking;
    private Vector3 baseScale;
    private Color baseColor;

    private UnityPool pool;
    private bool isDead = false;

    public Image healthBar;
    public Color dieColor = Color.red;

    public float minScale = 0.8f;
    public float maxScale = 1.2f;
    public float breathDuration = 1.5f;

    public float tempMoveSpeed;

    [Header("Buffs & Auras")]
    [SerializeField] public EnemyTargetsAround EnemyTargetsAround;
    public float checkUnitsAroundInterval = 0.25f;
    public float checkUnitsAroundTimer;
    public float rangeUnitsCheck = 5f;
    public BuffType buffType;
    public AuraType auraType;
    public BuffManager buffManager;

    private void Awake()
    {
        buffManager = GetComponent<BuffManager>();
        baseScale = transform.localScale;
        baseColor = enemySprite.color;

    }

    private void Start()
    {
        if (EnemyTargetsAround == null)
        {
            EnemyTargetsAround =   GetComponentInChildren<EnemyTargetsAround>();

            if (EnemyTargetsAround != null)
            {
                EnemyTargetsAround.cc.radius = rangeUnitsCheck;
            }
        }

        StartBreathing();

        //if(buffManager != null)
        //    buffManager.AddAura(auraType);
    }

    private void AssignRandomBuff(Enemy enemy)
    {
        enemy.buffType = BuffType.None;

        enemy.buffType = GetRandomBuff();

        BuffModifier buffModifier = new BuffModifier();
        buffModifier.buffDurationMultiplier = 10f;

        int wave = GameplayManager.Get.currentWave;
        int tier = wave / 10;

        buffModifier.buffPowerMultiplier = 1f + tier * 0.20f;


        if (buffManager != null)
            buffManager.AddBuff(enemy.buffType, buffModifier);

    }

    private void AssignRandomBuffOrAura(Enemy enemy)
    {
        enemy.buffType = BuffType.None;
        enemy.auraType = AuraType.None;

        int roll = UnityEngine.Random.Range(0, 100);

        // 50% none, 30% buff, 20% aura (example balance)
        if (roll < 25)
        {
            return; // none
        }
        else if (roll < 60)
        {
            enemy.buffType = GetRandomBuff();

            BuffModifier buffModifier = new BuffModifier();
            buffModifier.buffDurationMultiplier = 10f;

            int wave = GameplayManager.Get.currentWave;
            int tier = wave / 10;

            buffModifier.buffPowerMultiplier = 1f + tier * 0.40f;

            if (buffManager != null)
            buffManager.AddBuff(enemy.buffType, buffModifier);
        }
        else
        {
            enemy.auraType = GetRandomAura();
            if (buffManager != null)
                buffManager.AddAura(enemy.auraType);
        }
    }

    private BuffType GetRandomBuff()
    {
        BuffType[] buffs =
        {
        BuffType.ShieldAOE,
        BuffType.DefenseI,
        BuffType.DefenseII,
        BuffType.DeathSplit
    };

        return buffs[UnityEngine.Random.Range(0, buffs.Length)];
    }

    private AuraType GetRandomAura()
    {
        AuraType[] auras =
        {
        AuraType.DefenseAura,
        AuraType.Frenzy
    };

        return auras[UnityEngine.Random.Range(0, auras.Length)];
    }

    private void OnEnable()
    {
        if (tempMoveSpeed > 0)
            moveSpeed = tempMoveSpeed;
        StartBreathing();
    }

    private void OnDisable()
    {
        LeanTween.cancel(enemySprite.gameObject);
    }

    void StartBreathing()
    {
        // Scale down
        LeanTween.scale(enemySprite.gameObject, new Vector3(minScale, minScale, minScale), breathDuration / 2)
            .setEase(LeanTweenType.easeOutQuad)
            .setOnComplete(() =>
            {
                // Scale up
                LeanTween.scale(enemySprite.gameObject, new Vector3(maxScale, maxScale, maxScale), breathDuration / 2)
                    .setEase(LeanTweenType.easeInQuad)
                    .setOnComplete(StartBreathing); // Repeat the breathing effect
            });
    }

    internal void Set(UnityPool pool, float health, float damageVal, EnemyType enemyNewType)
    {
        this.pool = pool;

        if (enemyNewType == EnemyType.EliteEnemy)
        {
            health += GameplayManager.Get.currentWave;
        }
        maxHealth = health;
        damage = damageVal;
        currentHealth = maxHealth;
        target = Player.Instance.transform;
        enemyType = enemyNewType;
        isDead = false;
        healthBar.fillAmount = 1f;
        healthBar.transform.parent.gameObject.SetActive(false);



        switch (enemyType)
        {
            case EnemyType.BasicEnemy:
                if(ActiveGameData.Instance.currentSelectedWorld == 0)
                    AudioManager.Instance?.PlaySFXSound(AudioClipsType.BasicEnemy);
                else
                    AudioManager.Instance?.PlaySFXSound(AudioClipsType.W2_BasicEnemy);
                break;
            case EnemyType.MidEnemy:
                if (ActiveGameData.Instance.currentSelectedWorld == 0)
                    AudioManager.Instance?.PlaySFXSound(AudioClipsType.MidEnemy);
                else
                    AudioManager.Instance?.PlaySFXSound(AudioClipsType.W2_MidEnemy);
                break;
            case EnemyType.BulletEnemy:
                if (ActiveGameData.Instance.currentSelectedWorld == 0)
                    AudioManager.Instance?.PlaySFXSound(AudioClipsType.BulletEnemy);
                else
                    AudioManager.Instance?.PlaySFXSound(AudioClipsType.W2_BulletEnemy);
                break;
            case EnemyType.Boss:
                if (ActiveGameData.Instance.currentSelectedWorld == 0)
                    AudioManager.Instance?.PlaySFXSound(AudioClipsType.BossEnemy);
                else
                    AudioManager.Instance?.PlaySFXSound(AudioClipsType.W2_BossEnemy);

                AssignRandomBuffOrAura(this);
                break;
            case EnemyType.EliteEnemy:
                if (ActiveGameData.Instance.currentSelectedWorld == 0)
                    AudioManager.Instance?.PlaySFXSound(AudioClipsType.BossEnemy);
                else
                    AudioManager.Instance?.PlaySFXSound(AudioClipsType.W2_BossEnemy);

                AssignRandomBuff(this);
                transform.localScale *= 1.5f;
                StartCoroutine(RainbowCycle(enemySprite));

                break;
        }
    }

    private void Update()
    {

        if (target != null && Player.Instance.isDead == false)
        {
            // Calculate the distance to the target
            float distanceToTarget = Vector3.Distance(transform.position, target.position);

            // Check if the enemy is close enough to attack
            if (distanceToTarget <= attackDistance)
            {
                // Stop moving when within attack distance
                isAttacking = true;
            }
            else
            {
                // Move towards the target if not within attack distance
                Vector3 direction = (target.position - transform.position).normalized;
                transform.Translate(direction * moveSpeed * Time.deltaTime);
                isAttacking = false;
            }

            // Check if it's time to attack
            if (isAttacking && Time.time >= nextAttackTime)
            {
                Attack();
                nextAttackTime = Time.time + attackInterval;
            }
            
        }
    }

    private void Attack()
    {
        // Implement your attack logic here
        // For example, reduce the target's health

        if (GameplayManager.Get.randomGenerator.Next(0, 100) < Constants.Get.DodgeChance)
        {
            GameplayManager.Get.ShowDodge(Player.Instance.transform.position);
        }
        else
            Player.Instance.DoDamage(damage);

        if (enemyType == EnemyType.BulletEnemy)
            Die();
    }

    public void TakeDamage(float damage, DamageType damageType = DamageType.Normal)
    {
        if (isDead)
            return;

        if (ActiveGameData.Instance.currentSelectedWorld == 0)
            AudioManager.Instance?.PlaySFXSound(AudioClipsType.DeadEffect);
        else
            AudioManager.Instance?.PlaySFXSound(AudioClipsType.W2_DeadEffect);



        currentHealth -= (damage - defense) * ((100 - damageRed) / 100f);
        healthBar.fillAmount = currentHealth / maxHealth;
        healthBar.transform.parent.gameObject.SetActive(true);

        Debug.Log($"damage={damage}, defense={defense}, damageRed={damageRed}, raw={(damage - defense)}, mult={(100 - damageRed) / 100f}");


        if (damageType == DamageType.Crit)
            GameplayManager.Get.ShowCritDamage(transform.position);
        else if (damageType == DamageType.Stun)
            Stun();
        else if (damageType == DamageType.DeadHit && enemyType != EnemyType.Boss && enemyType != EnemyType.EliteEnemy)
            DeadHit();
        else if (damageType == DamageType.Kill && enemyType != EnemyType.Boss && enemyType != EnemyType.EliteEnemy)
            Kill();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Stun()
    {
        if (moveSpeed == 0)
            return;
        GameplayManager.Get.ShowStunDamage(transform.position);
        tempMoveSpeed = moveSpeed;
        moveSpeed = 0f;
        LeanTween.delayedCall(Constants.Get.StunTime, () =>
        {
            moveSpeed = tempMoveSpeed;
        });
    }

    public IEnumerator RainbowCycle(SpriteRenderer sr, float speed = 0.3f)
    {
        float hue = UnityEngine.Random.value;

        while (true)
        {
            hue = Mathf.Repeat(hue + Time.deltaTime * speed, 1f);
            sr.color = Color.HSVToRGB(hue, 1f, 1f);
            yield return null;
        }
    }

    private void DeadHit()
    {
        GameplayManager.Get.ShowDeadHitDamage(transform.position);
        currentHealth = 0;
    }

    private void Kill()
    {
        GameplayManager.Get.ShowKill(transform.position);
        currentHealth = 0;
    }

    private void Die()
    {
        ParticleManager.Get.ShowDeadParticle(transform.position, dieColor);
        isDead = true;

        if (enemyType == EnemyType.EliteEnemy)
        {
            transform.localScale = baseScale;
            enemySprite.color = baseColor;
            StopAllCoroutines();
        }

        pool.Add(this);
        EnemyGenerator.Get.spawnedEnemy.Remove((this, pool));

        if (ActiveGameData.Instance.saveData.introTutorial == 0 && TutorialManager.Get.isTutorialShowing == false)
            TutorialManager.Get.ShowIntroductionStep();

        if (enemyType == EnemyType.Boss)
        {
            EventManager.TriggerEvent(EventID.Boss_Die);
            ActiveGameData.Instance.saveData.bossDestroyed[ActiveGameData.Instance.currentSelectedWorld] += 1;

            if (ActiveGameData.Instance.currentSelectedWorld != 2)
            {
                Vector3 target = ScreenManager.Get.GetScreen<GameScreen>().ticketText.transform.position;
                if(ActiveGameData.Instance.diceRollActive)
                    CoinAnimation.Get.ShowTicket(transform.position, target);
                GameplayManager.Get.bossRewardScreen.ShowRewardPopup();
            }
        }
        else
        {
            EventManager.TriggerEvent(EventID.Enemy_Die);
            ActiveGameData.Instance.saveData.enemyDestroyed[ActiveGameData.Instance.currentSelectedWorld] += 1;
        }

        silverCoinForKill = (enemyType == EnemyType.EliteEnemy) ? GameplayManager.Get.randomGenerator.Next(10, 200) : GameplayManager.Get.randomGenerator.Next(1, 3);

        EventManager.TriggerEvent(EventID.Add_SilverCoin, silverCoinForKill);

        GameplayManager.Get.ShowSilverCoinGain(transform.position, silverCoinForKill);

        if (GameplayManager.Get.randomGenerator.Next(0, 100) < 27)
        {
            EventManager.TriggerEvent(EventID.Add_GoldCoin, 1);
            GameplayManager.Get.ShowGoldCoinGain(transform.position + new Vector3(0.2f, 0.2f, 0), 1);
        }
        else
        {
            if(enemyType == EnemyType.EliteEnemy)
            {
                EventManager.TriggerEvent(EventID.Add_GoldCoin, GameplayManager.Get.randomGenerator.Next(10, 50));
                GameplayManager.Get.ShowGoldCoinGain(transform.position + new Vector3(0.2f, 0.2f, 0), 1);
            }
        }
    }
}