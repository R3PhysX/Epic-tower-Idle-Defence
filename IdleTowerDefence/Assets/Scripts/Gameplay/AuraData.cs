using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AuraData", menuName = "Buffs/Aura Data")]
public class AuraData : ScriptableObject
{
    [Header("Identity")]
    public AuraType AuraType;

    [Header("Buff Emission")]
    [Tooltip("Which buff(s) does this aura apply to nearby allies?")]
    public List<BuffType> EmittedBuffs = new();

    [Tooltip("How far does this aura reach?")]
    public float Radius = 5f;

    [Tooltip("Apply buff to self?")]
    public bool ApplyToSelf = false;

    [Tooltip("How often to check and apply buffs (in seconds)")]
    public float TickInterval = 0.25f;

    [Header("Visuals - Shown on the SOURCE")]
    [Tooltip("Visual effect on the enemy that HAS this aura")]
    public GameObject VisualPrefab;
    public Sprite Icon;
}