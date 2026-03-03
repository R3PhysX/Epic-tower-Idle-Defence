using DigitalRuby.LightningBolt;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.EventSystems.EventTrigger;

public class MovingGroundsManager : MonoBehaviour
{
    public CardData cardData;
    protected AuraData dataaura;
    public static MovingGroundsManager Get;
    [SerializeField] private BuffsDatabase database;
    public AuraType auraType;
    protected float nextTickTime;
    public int slowAmount = 10;

    [SerializeField] public EnemyTargetsAround EnemyTargetsAround;

    public float attackRange = 0.8f;

    private void Awake()
    {
        Get = this;
    }

    void Start()
    {
        if (EnemyTargetsAround == null)
        {
            EnemyTargetsAround = GetComponentInChildren<EnemyTargetsAround>();

            if (EnemyTargetsAround != null)
            {
                EnemyTargetsAround.cc.radius = 1.8f;
            }
        }

        AddAura(auraType);

        var data = cardData.cards[6];
        switch (data.savedData.level)
        {
            case 2:
                slowAmount = data.level2.value2;
                attackRange = 0.9f;
                break;
            case 3:
                slowAmount = data.level3.value2;
                attackRange = 1f;
                break;
            default:
                slowAmount = data.level1.value2;
                attackRange = 0.8f;
                break;
        }
        transform.localScale = Vector3.one * attackRange;
        // Start the throwing coroutine

        //spawnedLightning = GameObject.Instantiate(lightningParticle.gameObject).GetComponent<LightningBoltScript>();
        //spawnedLightning.transform.parent = transform;
    }

    public void AddAura(AuraType type)
    {

        var data = database.GetAura(type);
        if (data == null)
        {
            Debug.LogWarning($"AuraData for {type} not found in database!");
            return;
        }

        dataaura = data;

    }


    protected virtual void EmitBuffs()
    {
        // Apply each buff this aura provides to nearby allies
        foreach (var buffType in dataaura.EmittedBuffs)
        {
            ApplyBuffToNearbyAllies(buffType);
        }
    }

    protected void ApplyBuffToNearbyAllies(BuffType buffType)
    {
        if (EnemyTargetsAround == null) return;

        // Apply to nearby allies
        foreach (Enemy ally in EnemyTargetsAround.friendlyUnits)
        {
            if (ally != null && ally.buffManager != null)
            {
                BuffModifier buffModifier = new BuffModifier();
                buffModifier.buffPowerMultiplier = slowAmount / 100f;

                ally.buffManager.AddBuff(buffType, buffModifier, null);
            }
        }
    }

    private void Update()
    {
        float dt = Time.deltaTime;

        nextTickTime -= dt;

        if (nextTickTime <= 0f)
        {
            EmitBuffs();
            nextTickTime = dataaura.TickInterval;
        }
    }

}