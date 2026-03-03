using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "New Buffs Database", menuName = "Buffs/BuffsDatabase")]
public class BuffsDatabase : ScriptableObject
{
    public List<BuffData> buffs = new();
    public List<AuraData> auras = new();

    public BuffData GetBuff(int id)
    {
        return buffs.FirstOrDefault(x => x.ID == id);
    }

    public BuffData GetBuff(BuffType type)
    {
        return buffs.FirstOrDefault(x => x.BuffType == type);
    }

    public AuraData GetAura(AuraType type)
    {
        return auras.FirstOrDefault(x => x.AuraType == type);
    }
}