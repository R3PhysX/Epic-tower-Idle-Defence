using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BuffManager : MonoBehaviour
{
    [SerializeField] private BuffsDatabase database;
    private readonly List<BuffBase> activeBuffs = new();
    private readonly List<AuraBase> activeAuras = new();

    private Enemy enemy;

    private void Awake()
    {
        enemy = GetComponent<Enemy>();
    }

    private void Start()
    {
        //AddBuff(BuffType.Slow);
    }

    private void Update()
    {
       float dt = Time.deltaTime;

        for (int i = activeBuffs.Count - 1; i >= 0; i--)
        {
            var buff = activeBuffs[i];
            buff.Tick(enemy, dt);

            if(buff.IsExpired)
            {
                buff.Remove(enemy);
                activeBuffs.RemoveAt(i);
            }
        }

        foreach (var aura in activeAuras)
        {
            aura.Tick(dt);
        }
    }

    public void AddBuff(BuffType type, BuffModifier buffModifier = null, GameObject appliedBy = null)
    {
        var data = database.GetBuff(type);
        if (data == null)
        {
            Debug.LogWarning($"BuffData for {type} not found in database!");
            return;
        }

        var existing = activeBuffs.Find(b => b.Type == type);

        if (existing != null)
        {
            // Refresh existing buff
            existing.Refresh();
        }
        else
        {
            var buff = BuffFactory.Create(data);
            if (buff == null)
            {
                Debug.LogWarning($"BuffFactory couldn't create buff for {type}!");
                return;
            }

            buff.Apply(enemy, buffModifier, appliedBy);
            activeBuffs.Add(buff);
        }
    }

    public void AddAura(AuraType type)
    {
        if (activeAuras.Exists(a => a.Type == type))
            return;

        var data = database.GetAura(type);
        if (data == null)
        {
            Debug.LogWarning($"AuraData for {type} not found in database!");
            return;
        }

        AuraBase aura = new(data, enemy);
        aura.Apply();
        activeAuras.Add(aura);

    }

    /// Removes an aura from this enemy
    public void RemoveAura(AuraType type)
    {
        var aura = activeAuras.Find(a => a.Type == type);
        if (aura != null)
        {
            aura.Remove();
            activeAuras.Remove(aura);
        }
    }

    /// Removes all buffs that were applied by a specific source (useful when source dies)
    public void RemoveBuffsFromSource(GameObject source)
    {
        for (int i = activeBuffs.Count - 1; i >= 0; i--)
        {
            if (activeBuffs[i].Source == source)
            {
                activeBuffs[i].Remove(enemy);
                activeBuffs.RemoveAt(i);
            }
        }
    }

    public bool HasBuff(BuffType type)
    {
        return activeBuffs.Exists(b => b.Type == type);
    }

    public bool HasAura(AuraType type)
    {
        return activeAuras.Exists(a => a.Type == type);
    }

    private void OnDisable()
    {
        // Clean up all auras when enemy dies/is disabled
        foreach (var aura in activeAuras)
        {
            aura.Remove();
        }
        activeAuras.Clear();

        // Clean up all buffs
        foreach (var buff in activeBuffs)
        {
            buff.Remove(enemy);
        }
        activeBuffs.Clear();
    }
}
