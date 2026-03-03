using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BuffFactory
{
    public static BuffBase Create(BuffData data)
    {

        if (data == null)
        {
            Debug.LogError("BuffFactory: Cannot create buff from null data!");
            return null;
        }

        return data.BuffType switch
        {
            BuffType.Slow => new SlowBuff(data),
            BuffType.SlowPercentage => new SlowPercentageBuff(data),
            BuffType.DefenseI => new DefenseBuff(data),
            BuffType.DefenseII => new DefenseBuff(data),
            BuffType.DeathSplit => new DeathSplitBuff(data),
            BuffType.ShieldAOE => new StatsIncreaseBuff(data),
            BuffType.None => null,
            _ => throw new System.NotImplementedException(
                $"BuffFactory: No implementation for BuffType.{data.BuffType}. " +
                $"Add it to the switch statement in BuffFactory.Create()"
            )
        };
    }

}
