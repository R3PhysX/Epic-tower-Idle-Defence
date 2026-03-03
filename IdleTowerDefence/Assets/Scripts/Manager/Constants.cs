using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Constants : MonoBehaviour
{
    private static Constants instance;
    public static Constants Get { get { return instance; } set { instance = value; } }

    private void Awake()
    {
        instance = this;
        if (instance == null)
        {
            instance = this;
           // DontDestroyOnLoad(this);
        }
        else
        {
           // Destroy(gameObject);
        }
    }

    [SerializeField] internal int CurrentWaveMilestone = 1;

    internal int cardUnlockWave = 8;

    [Header("Player")]
    [SerializeField] internal float PlayerHealth = 100f;
    [SerializeField] internal float ShootInterval = 1f;

    [SerializeField] internal int SlowTurretAura = 0;
    [SerializeField] internal int DamageTurretAura = 0;

    internal float ShootingSpeed = 1f;

    [SerializeField] internal float BulletDamage = 10f;
    [SerializeField] internal float HealthRegenPerSecond = 0.5f;

    [SerializeField] internal float CriticalShotChance = 12f;
    [SerializeField] internal float CriticalShotDamage = 120f;

    [SerializeField] internal float DoubleShotChance = 14;
    [SerializeField] internal float DoubleShotDamagePercentage = 140;

    [SerializeField] internal float DodgeChance = 25;

    [SerializeField] internal float DamageResistancePercentage = 25;

    [Header("Boundary")]
    [SerializeField] internal float BoundaryRadius = 2f;
    [SerializeField] internal float RangeDamageBonus = 20;

    [Header("Enemy")]
    [SerializeField] internal float SpawningRate = 2f;
    [SerializeField] internal float SpawnOffSetRadius = 1.25f;

    [SerializeField] internal float StunChance = 10;
    [SerializeField] internal float StunTime = 0.1f;

    [SerializeField] internal float DeadHitChance = 8;

    [Header("Currency")]
    [SerializeField] internal int SilverCoin = 0;
    [SerializeField] internal int GoldCoin { get { return ActiveGameData.Instance.saveData.GoldCoin; } set { ActiveGameData.Instance.saveData.GoldCoin = value; } }
    [SerializeField] internal int Diamond { get { return ActiveGameData.Instance.saveData.Diamond; } set { ActiveGameData.Instance.saveData.Diamond = value; } }
    [SerializeField] internal int Gems { get { return ActiveGameData.Instance.saveData.Gems; } set { ActiveGameData.Instance.saveData.Gems = value; } }

    [SerializeField] internal int SilverCoinWaveBonus = 10;
    [SerializeField] internal int SilverCoinEach50Kills = 50;

    [SerializeField] internal int GoldCoinWaveBonus = 10;
    [SerializeField] internal int GoldCoinAfterBossDie = 50;

    internal void UpdateBoundary()
    {
        Player player = FindObjectOfType<Player>();
        if (player == null)
            return;

        player.DrawBoundaryCircle();
    }
}
