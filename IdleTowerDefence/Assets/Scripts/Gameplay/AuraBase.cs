using UnityEngine;

public class AuraBase
{
    protected AuraData data;
    protected GameObject visualInstance;
    protected Enemy owner;
    protected float nextTickTime;

    public AuraType Type => data.AuraType;
    public float Radius => data.Radius;
    public bool SelfApply => data.ApplyToSelf;

    public AuraBase(AuraData data, Enemy owner)
    {
        this.data = data;
        this.owner = owner;
    }

    public virtual void Apply()
    {
        if (data.VisualPrefab != null && owner != null)
        {
            visualInstance = Object.Instantiate(
                data.VisualPrefab,
                owner.transform
                );
        }
    }

    public virtual void Tick(float deltaTime)
    {
        nextTickTime -= deltaTime;

        if (nextTickTime <= 0f)
        {
            EmitBuffs();
            nextTickTime = data.TickInterval;
        }
    }

    protected virtual void EmitBuffs()
    {
        // Apply each buff this aura provides to nearby allies
        foreach (var buffType in data.EmittedBuffs)
        {
            ApplyBuffToNearbyAllies(buffType);
        }
    }

    protected void ApplyBuffToNearbyAllies(BuffType buffType)
    {
        if (owner.EnemyTargetsAround == null) return;

        // Apply to nearby allies
        foreach (Enemy ally in owner.EnemyTargetsAround.friendlyUnits)
        {
            if (ally != null && ally.buffManager != null)
            {
                ally.buffManager.AddBuff(buffType, null, owner.gameObject);
            }
        }

        // Also apply to self if desired
        if (owner.buffManager != null && SelfApply)
        {
            owner.buffManager.AddBuff(buffType, null, owner.gameObject);
        }
    }

    public virtual void Remove()
    {
        if (visualInstance != null)
        {
            Object.Destroy(visualInstance);
        }
    }
}