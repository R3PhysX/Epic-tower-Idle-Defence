using UnityEngine;

[CreateAssetMenu(fileName = "BuffData", menuName = "Buffs/Buff Data")]
public class BuffData : ScriptableObject
{
    [Header("Identity")]
    public int ID;
    public BuffType BuffType;

    [Header("Duration")]
    public float Duration = 5f;

    [Header("Defense")]
    public int DefenseAdd;
    public int DefenseRed;

    [Header("Attack")]
    public int AttackAdd;

    [Header("Life")]
    public int LifeAdd;
    public float LifeAddRate;

    [Header("Speed")]
    public float MovementSpeedAdd;
    public float AttackSpeedAddRate;

    [Header("Damage over Time")]
    public bool IsDOT;
    public float DotInterval = 0.5f;
    public int FireDamage;
    public int PoisonDamage;
    public int ColdDamage;

    [Header("Crowd Control")]
    public bool TargetImmobilise;
    public bool TargetUnmovable;

    [Header("Stat Modifiers")]
    public float XPRateBonus;
    public float GoldRateBonus;
    public float GemRateBonus;

    [Header("Visuals")]
    public GameObject VisualPrefab;
    public Sprite Icon;
    public Color SpriteColorChange;
}