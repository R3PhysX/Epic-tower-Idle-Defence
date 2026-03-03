using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathSplitBuff : BuffBase
{

    public DeathSplitBuff(BuffData data) : base(data) {}

    public override void Apply(Enemy enemy, BuffModifier buffModifier = null , GameObject appliedBy = null)
    {
        base.Apply(enemy);

        if (buffModifier != null)
        {
            remainingTime *= buffModifier.buffDurationMultiplier;
        }

        //enemy.healthBar.color = Color.blue;
    }

    public override void Tick(Enemy enemy, float deltaTime)
    {
        base.Tick(enemy, deltaTime);
    }

    public override void Remove(Enemy enemy)
    {

        base.Remove(enemy);
    }
}
