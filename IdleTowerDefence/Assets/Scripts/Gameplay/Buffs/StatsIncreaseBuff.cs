using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatsIncreaseBuff : BuffBase
{
    private float currentDefenseAdded;
    private float currentDefenseRedAdded;
    private float currentHpAdded;
    private float currentAttackAdded;
    public StatsIncreaseBuff(BuffData data) : base(data) {}

    public override void Apply(Enemy enemy, BuffModifier buffModifier = null, GameObject appliedBy = null)
    {
        base.Apply(enemy);

        float durationMul = buffModifier != null ? buffModifier.buffDurationMultiplier : 1f;
        float powerMul = buffModifier != null ? buffModifier.buffPowerMultiplier : 1f;

        remainingTime *= durationMul;

        currentDefenseAdded = data.DefenseAdd * powerMul;
        enemy.defense += currentDefenseAdded;

        currentDefenseRedAdded = data.DefenseRed * powerMul;
        enemy.damageRed += currentDefenseRedAdded;

        currentHpAdded = data.LifeAdd * powerMul;
        enemy.maxHealth += currentHpAdded;


        enemy.moveSpeed += data.MovementSpeedAdd;

        currentAttackAdded = data.AttackAdd * powerMul;
        enemy.damage += currentAttackAdded;

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
            enemy.defense -= currentDefenseAdded;
            enemy.damageRed -= currentDefenseRedAdded;
            enemy.maxHealth -= currentHpAdded;
            enemy.damage -= currentAttackAdded;
            enemy.moveSpeed -= data.MovementSpeedAdd;
        }

        base.Remove(enemy);
    }
}
