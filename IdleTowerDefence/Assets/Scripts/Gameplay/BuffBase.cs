using System.Data;
using UnityEngine;

[System.Serializable]
public class BuffModifier
{
    public float buffDurationMultiplier = 1f;
    public float buffPowerMultiplier = 1f;
}

public abstract class BuffBase
{
    protected BuffData data;
    protected float remainingTime;
    protected GameObject visualInstance;
    protected GameObject source;

    public BuffType Type => data.BuffType;
    public bool IsExpired => remainingTime <= 0;
    public GameObject Source => source;

    protected BuffBase(BuffData data)
    {
        this.data = data;
        remainingTime = data.Duration;
    }

    public virtual void Apply(Enemy enemy, BuffModifier buffModifier = null, GameObject appliedBy = null)
    {
        source = appliedBy;

        if (data.VisualPrefab != null && enemy != null)
        {
            visualInstance = Object.Instantiate(
                data.VisualPrefab,
                enemy.transform
                );
        }
        enemy.enemySprite.color = data.SpriteColorChange;
    }

    public virtual void Tick(Enemy enemy, float deltaTime)
    {
        remainingTime -= deltaTime;
    }

    public virtual void Refresh()
    {
            remainingTime = data.Duration;
    }

    public virtual void Remove(Enemy enemy)
    {
        if (visualInstance != null)
        {
            Object.Destroy(visualInstance);
            enemy.enemySprite.color = Color.white;
        }
    }
}
