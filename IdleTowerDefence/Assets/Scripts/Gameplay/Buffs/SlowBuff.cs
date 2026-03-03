using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowBuff : BuffBase
{
    private float currentMoveSpeedAdded;

    public SlowBuff(BuffData data) : base(data) {}

    public override void Apply(Enemy enemy, BuffModifier buffModifier = null, GameObject appliedBy = null)
    {
        base.Apply(enemy);

        float durationMul = buffModifier != null ? buffModifier.buffDurationMultiplier : 1f;
        float powerMul = buffModifier != null ? buffModifier.buffPowerMultiplier : 1f;

        remainingTime *= durationMul;

        currentMoveSpeedAdded = data.MovementSpeedAdd * powerMul;
        enemy.moveSpeed += currentMoveSpeedAdded;
        
        //enemy.healthBar.color = Color.blue;
    }

    public override void Tick(Enemy enemy, float deltaTime)
    {
        base.Tick(enemy, deltaTime);
    }

    public override void Remove(Enemy enemy)
    {
        if (enemy != null)
        {
            enemy.moveSpeed -= currentMoveSpeedAdded;
        }

        base.Remove(enemy);
    }
}
