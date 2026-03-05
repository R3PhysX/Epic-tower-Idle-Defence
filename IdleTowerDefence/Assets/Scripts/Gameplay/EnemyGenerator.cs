using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public class EnemyData
{
    public Enemy enemyPrefab;
    [Range(0f, 1f)] public float spawnProbability;
    public int waveToStartSpawning = 1; // Wave when this enemy starts spawning
    internal UnityPool pool;
    public float initialHealth;
    public int healthIncreasePerWave;
    public EnemyType enemyType;
    public float initialDamage;
    public int damageIncreasePerWave;
}


public class EnemyGenerator : MonoBehaviour
{
    private enum WaveArchetype
    {
        Normal,
        Swarm,   // more enemies, lower hp
        Tank,    // fewer enemies, higher hp
        Speed,   // fewer enemies, faster spawn (feels faster)
        Elite    // normal count, higher hp
    }

    private struct WavePlan
    {
        public WaveArchetype type;
        public float countMul;
        public float hpMul;
        public float spawnRateMul;
    }

    [SerializeField] private int rewarderOnEveryWave = 4;
    public static EnemyGenerator Get;

    [SerializeField] private Transform enemiesContainer;
    [SerializeField] private List<EnemyData> bossEnemies;
    [SerializeField] private List<EnemyData> enemies;
    internal List<(Enemy, UnityPool)> spawnedEnemy;

    private int baseEnemiesPerWave = 3;
    private int highEnemiesPerWave = 5;

    private List<(int, int)> enemyRange = new List<(int, int)>() { (8, 12), (6, 9), (3, 7), (2, 6), (1, 4), (1, 2) };
    public int enemiesToSpawnCount;

    [SerializeField] private int bossOnEveryWave = 5;
    [SerializeField] private float intervalBetweenWave = 1f;

    private int minAdditionalEnemies = 0;
    private int maxAdditionalEnemies = 0;

    private void Awake()
    {
        Get = this;
    }




    private int GetBandIndex(int wave)
    {
        return Mathf.Clamp((wave - 1) / 10, 0, enemyRange.Count - 1);
    }

    private int GetEnemiesThisWave(int wave)
    {
        int band = GetBandIndex(wave);
        int min = enemyRange[band].Item1;
        int max = enemyRange[band].Item2;

        // Non-linear growth: log curve (ramps early, then smooths)
        float curve = 1f + Mathf.Log(wave + 1, 2f) * 0.50f; // tweak 0.25..0.5
        int baseCount = Random.Range(baseEnemiesPerWave, highEnemiesPerWave + 1);

        // Hard cap prevents infinite scaling into lag
        return Mathf.Clamp(Mathf.RoundToInt(baseCount * curve), 1, 200);
    }

    private WavePlan GetWavePlan(int wave)
    {
        // Cadence: reward waves feel lighter
        bool isRewardWave = (wave % rewarderOnEveryWave == 0);

        // Weighted randomness (controlled)
        int roll = Random.Range(0, 100);

        WavePlan plan;
        if (isRewardWave)
        {
            plan.type = WaveArchetype.Normal;
            plan.countMul = 0.85f;
            plan.hpMul = 0.9f;
            plan.spawnRateMul = 1.05f;
            return plan;
        }
        Debug.Log("Wave " + roll);

        if (roll < 50) plan.type = WaveArchetype.Normal;
        else if (roll < 70) plan.type = WaveArchetype.Swarm;
        else if (roll < 80) plan.type = WaveArchetype.Tank;
        else if (roll < 90) plan.type = WaveArchetype.Speed;
        else plan.type = WaveArchetype.Elite;

        switch (plan.type)
        {
            case WaveArchetype.Swarm:
                plan.countMul = 1.5f;
                plan.hpMul = 0.50f;
                plan.spawnRateMul = 1.15f;
                break;
            case WaveArchetype.Tank:
                plan.countMul = 0.40f;
                plan.hpMul = 2.2f;
                plan.spawnRateMul = 0.95f;
                break;
            case WaveArchetype.Speed:
                plan.countMul = 0.60f;
                plan.hpMul = 1.0f;
                plan.spawnRateMul = 1.40f;
                break;
            case WaveArchetype.Elite:
                plan.countMul = 1.0f;
                plan.hpMul = 1.35f;
                plan.spawnRateMul = 1.0f;
                break;
            default:
                plan.countMul = 1.0f;
                plan.hpMul = 1.0f;
                plan.spawnRateMul = 1.0f;
                break;
        }

        // Small deterministic spikes every 5 waves
        if (wave % 5 == 0)
        {
            plan.countMul *= 1.15f;
            plan.hpMul *= 1.10f;
        }

        return plan;
    }




    internal void RemoveAllEnemy()
    {
        spawnedEnemy.ForEach(x => x.Item2.Add(x.Item1));
        spawnedEnemy.Clear();
    }

    internal void LevelDownBosses()
    {
        foreach (var item in bossEnemies)
        {
            item.initialHealth -= ((item.initialHealth * item.healthIncreasePerWave * 3) / 100f);
            item.initialDamage -= ((item.initialDamage * item.damageIncreasePerWave * 3) / 100f);
        }
    }

    private void Start()
    {
        Constants.Get.SpawningRate = 1.75f;
        spawnedEnemy = new List<(Enemy, UnityPool)>();
        foreach (var item in enemies)
        {
            item.pool = new UnityPool(item.enemyPrefab.gameObject, 10, enemiesContainer);
        }

        foreach (var item in bossEnemies)
        {
            item.pool = new UnityPool(item.enemyPrefab.gameObject, 10, enemiesContainer);
        }
        StartCoroutine(StartWave());
    }


    private IEnumerator StartWave()
    {
        EventManager.TriggerEvent(EventID.Event_WaveUpdate, GameplayManager.Get.currentWave);
        EventManager.TriggerEvent(EventID.Event_WaveEnemyProgressUpdate, 1f);
        yield return new WaitForSeconds(1f);

        while (true)
        {
            int wave = GameplayManager.Get.currentWave;

            if (wave % rewarderOnEveryWave == 0)
                GemRewarderManager.Get.ShowRewarder();

            // Plan the wave (controlled randomness)
            WavePlan plan = GetWavePlan(wave);


            // Compute enemies for THIS wave (not cumulative!)
            int enemiesThisWave = Mathf.Clamp(
                Mathf.RoundToInt(GetEnemiesThisWave(wave) * plan.countMul),
                1,
                250
            );

            switch (plan.type)
            {
                case WaveArchetype.Elite:
                    enemiesThisWave = Mathf.Max(1, enemiesThisWave / 2);
                    break;
            }

            Debug.Log($"Wave: {wave}  Enemies: {enemiesThisWave}  Type: {plan.type}  World: {ActiveGameData.Instance.currentSelectedWorld}");
            GameAnalyticsManager.Instance.NewProgressonEventGA(
                GameAnalyticsSDK.GAProgressionStatus.Start,
                ActiveGameData.Instance.currentSelectedWorld.ToString(),
                wave.ToString()
            );

            var listOfEnemy = SelectRandomEnemiesPrefab(enemiesThisWave, plan);

            // Spawn loop
            for (int i = 0; i < listOfEnemy.Count; i++)
            {
                SpawnEnemy(listOfEnemy[i].Item1, plan.hpMul, listOfEnemy[i].Item2);

                float progressUpdate = (listOfEnemy.Count <= 1)
                    ? 0f
                    : 1f - ((float)i / (listOfEnemy.Count - 1));

                EventManager.TriggerEvent(EventID.Event_WaveEnemyProgressUpdate, progressUpdate);

                // Speed waves spawn quicker; tank waves slightly slower, etc.
                float baseRate = Constants.Get.SpawningRate;
                float rate = Mathf.Clamp(baseRate / plan.spawnRateMul, 0.05f, 3f);

                if (GameplayManager.Get.randomGenerator.Next(0, 100) > 8)
                    yield return new WaitForSeconds(Random.Range(Mathf.Max(0.05f, rate - 0.1f), rate));
                else
                    yield return new WaitForSeconds(0.1f);

                while (Player.Instance.isDead)
                    yield return null;
            }

            //* Safety: don't wait forever if something bugs out
            //float waitTimer = 0f;
            //float maxWait = Mathf.Clamp(15f + enemiesThisWave * 0.5f, 20f, 90f);

            while (spawnedEnemy.Count > 0)
            {
                //waitTimer += Time.deltaTime;
                yield return null;
            }

            // If still stuck, clean up to avoid soft-lock
            /*if (spawnedEnemy.Count > 0)
            {
                Debug.LogWarning("Wave cleanup: enemies stuck alive too long. Removing all enemies to continue.");
                RemoveAllEnemy();
            }*/

            // Next wave
            GameplayManager.Get.IncrementWave();
            EventManager.TriggerEvent(EventID.Event_WaveUpdate, GameplayManager.Get.currentWave);
            EventManager.TriggerEvent(EventID.Event_WaveEnemyProgressUpdate, 1f);

            // Keep your spawning rate ramp, but clamp
            Constants.Get.SpawningRate -= (Constants.Get.SpawningRate * 8) / 100f;
            Constants.Get.SpawningRate = Mathf.Clamp(Constants.Get.SpawningRate, 0.5f, 3f);

            yield return new WaitForSeconds(intervalBetweenWave);
            UpgradeEnemies();
        }
    }

    /*
    private IEnumerator StartWave()
    {
        EventManager.TriggerEvent(EventID.Event_WaveUpdate, GameplayManager.Get.currentWave);
        EventManager.TriggerEvent(EventID.Event_WaveEnemyProgressUpdate, 1f);
        yield return new WaitForSeconds(1f);

        while (true)
        {
            if ((GameplayManager.Get.currentWave) % rewarderOnEveryWave == 0)
            {
                GemRewarderManager.Get.ShowRewarder();
            }

            // Calculate the number of enemies to spawn for this wave

            if (GameplayManager.Get.currentWave > 0 && GameplayManager.Get.currentWave <= 10)
            {
                minAdditionalEnemies = enemyRange[0].Item1;
                maxAdditionalEnemies = enemyRange[0].Item2;
            }
            else if (GameplayManager.Get.currentWave > 10 && GameplayManager.Get.currentWave <= 20)
            {
                minAdditionalEnemies = enemyRange[1].Item1;
                maxAdditionalEnemies = enemyRange[1].Item2;
            }
            else if (GameplayManager.Get.currentWave > 20 && GameplayManager.Get.currentWave <= 30)
            {
                minAdditionalEnemies = enemyRange[2].Item1;
                maxAdditionalEnemies = enemyRange[2].Item2;
            }
            else if (GameplayManager.Get.currentWave > 30 && GameplayManager.Get.currentWave <= 40)
            {
                minAdditionalEnemies = enemyRange[3].Item1;
                maxAdditionalEnemies = enemyRange[3].Item2;
            }
            else if (GameplayManager.Get.currentWave > 40 && GameplayManager.Get.currentWave <= 50)
            {
                minAdditionalEnemies = enemyRange[4].Item1;
                maxAdditionalEnemies = enemyRange[4].Item2;
            }
            else
            {
                minAdditionalEnemies = enemyRange[5].Item1;
                maxAdditionalEnemies = enemyRange[5].Item2;
            }

            enemiesToSpawnCount = enemiesToSpawnCount + Random.Range(minAdditionalEnemies, maxAdditionalEnemies);

            Debug.Log("Wave : " + GameplayManager.Get.currentWave + ", Enemy : " + enemiesToSpawnCount + "  World " + ActiveGameData.Instance.currentSelectedWorld);
            GameAnalyticsManager.Instance.NewProgressonEventGA(GameAnalyticsSDK.GAProgressionStatus.Start,ActiveGameData.Instance.currentSelectedWorld.ToString(), GameplayManager.Get.currentWave.ToString());
            var listOfEnemy = SelectRandomEnemiesPrefab(enemiesToSpawnCount);

            // Spawn the new wave
            for (int i = 0; i < listOfEnemy.Count; i++)
            {
                SpawnEnemy(listOfEnemy[i]);
                float progressUpdate = (1 - ((float)i / ((float)enemiesToSpawnCount - 1)));
                EventManager.TriggerEvent(EventID.Event_WaveEnemyProgressUpdate, progressUpdate);

                if (GameplayManager.Get.randomGenerator.Next(0, 100) > 8)
                {
                    yield return new WaitForSeconds(Random.Range(Constants.Get.SpawningRate - 0.1f, Constants.Get.SpawningRate));
                }
                else
                {
                    yield return new WaitForSeconds(0.1f);
                }

                while (Player.Instance.isDead)
                {
                    yield return null;
                }
            }

            while (spawnedEnemy.Count > 0)
            {
                yield return null;
            }

            GameplayManager.Get.IncrementWave();
            EventManager.TriggerEvent(EventID.Event_WaveUpdate, GameplayManager.Get.currentWave);
            EventManager.TriggerEvent(EventID.Event_WaveEnemyProgressUpdate, 1f);
            Constants.Get.SpawningRate -= ((Constants.Get.SpawningRate * 8) / 100f);
            Constants.Get.SpawningRate = Mathf.Clamp(Constants.Get.SpawningRate, 0.5f, 3f);

            yield return new WaitForSeconds(intervalBetweenWave);
            UpgradeEnemies();
        }
    }*/

    private void UpgradeEnemies()
    {
        foreach (var item in enemies)
        {
            item.initialHealth += ((item.initialHealth * item.healthIncreasePerWave) / 100f);
            item.initialDamage += ((item.initialDamage * item.damageIncreasePerWave) / 100f);
        }

        if (GameplayManager.Get.currentWave % bossOnEveryWave == 0)
        {
            foreach (var item in bossEnemies)
            {
                item.initialHealth += ((item.initialHealth * item.healthIncreasePerWave) / 100f);
                item.initialDamage += ((item.initialDamage * item.damageIncreasePerWave) / 100f);
            }
        }
    }

    private void SpawnEnemy(EnemyData enemyData)
    {
        Vector3 spawnPosition = GetRandomSpawnPositionOutsideRadius();

        if (ActiveGameData.Instance.saveData.currentSelectedWorld != 2 && enemyData.enemyPrefab.enemyType == EnemyType.Boss)
            LeanTween.delayedCall(5f, GameplayManager.Get.ShowBoss);

        if (spawnPosition != Vector3.zero)
        {
            // Spawn the selected enemy prefab at the calculated position
            var obj = enemyData.pool.Get<Enemy>(enemiesContainer);
            //   var obj = Instantiate(enemyPrefab.gameObject, enemiesContainer);

            obj.transform.position = spawnPosition;
            obj.Set(enemyData.pool, enemyData.initialHealth, enemyData.initialDamage, enemyData.enemyType);
            spawnedEnemy.Add((obj, enemyData.pool));
        }
    }

    private void SpawnEnemy(EnemyData enemyData, float hpMultiplier, bool makeElite)
    {
        Vector3 spawnPosition = GetRandomSpawnPositionOutsideRadius();

        if (ActiveGameData.Instance.saveData.currentSelectedWorld != 2 && enemyData.enemyPrefab.enemyType == EnemyType.Boss)
            LeanTween.delayedCall(5f, GameplayManager.Get.ShowBoss);

        if (spawnPosition != Vector3.zero)
        {
            var obj = enemyData.pool.Get<Enemy>(enemiesContainer);
            obj.transform.position = spawnPosition;

            float hp = enemyData.initialHealth * hpMultiplier;

            obj.Set(enemyData.pool, hp, enemyData.initialDamage, makeElite ? EnemyType.EliteEnemy : enemyData.enemyType);

            spawnedEnemy.Add((obj, enemyData.pool));
        }
    }

    public void SpawnSplits(EnemyData enemyData)
    {
        // Coming soon
    }

    private Vector3 GetRandomSpawnPositionOutsideRadius()
    {
        Vector3 playerPosition = Player.Instance.transform.position;
        Vector3 spawnPosition = Vector3.zero;

        // Attempt to find a spawn position outside the radius
        for (int i = 0; i < 100; i++) // Limiting to 100 tries to prevent infinite loops
        {
            Vector2 randomCirclePoint = (Random.insideUnitCircle.normalized * (Constants.Get.BoundaryRadius + Constants.Get.SpawnOffSetRadius)) * Random.Range(1f, 1.1f);
            spawnPosition = playerPosition + new Vector3(randomCirclePoint.x, randomCirclePoint.y, 0f);

            if (Vector3.Distance(playerPosition, spawnPosition) > Constants.Get.BoundaryRadius)
            {
                return spawnPosition;
            }
        }

        return spawnPosition;
    }

    private List<(EnemyData, bool)> SelectRandomEnemiesPrefab(int count, WavePlan plan)
    {
        List<(EnemyData,bool)> selectedEnemies = new();

        // Create a list of available enemies with non-zero probabilities
        List<EnemyData> availableEnemies = enemies
            .Where(enemy => enemy.spawnProbability > 0f && enemy.waveToStartSpawning <= GameplayManager.Get.currentWave)
            .ToList();

        if (availableEnemies.Count > 0)
        {
            if (plan.type == WaveArchetype.Swarm)
            {
                if (availableEnemies.Count > 0)
                {
                    for (int i = 0; i < count; i++)
                    {
                        selectedEnemies.Add((enemies.First(),false));
                    }
                }
            }
            else
            {
                // Ensure that there are available enemies to choose from

                int eliteEnemies = 0;
                int eliteIndex = -1;
                bool isElite = false;
                if (plan.type == WaveArchetype.Elite)
                {
                    eliteEnemies = 1;
                    eliteIndex = (eliteEnemies > 0) ? Random.Range(0, count) : -1;
                }

                // Shuffle the list of available enemies randomly
                availableEnemies = availableEnemies.OrderBy(enemy => Random.value).ToList();

                // Calculate the total spawn probability of available enemies
                float totalProbability = availableEnemies.Sum(enemy => enemy.spawnProbability);

                for (int i = 0; i < count; i++)
                {

                    float randomValue = Random.Range(0f, totalProbability);

                    foreach (var enemyData in availableEnemies)
                    {
                        if (randomValue <= enemyData.spawnProbability)
                        {

                            if (eliteEnemies > 0 && i == eliteIndex)
                            {
                                isElite = (eliteEnemies > 0 && i == eliteIndex);
                                eliteEnemies--;

                            }
                            else
                            {
                                isElite = false;
                            }

                            selectedEnemies.Add((enemyData, isElite));


                            break;
                        }

                        randomValue -= enemyData.spawnProbability;
                    }
                }
            }

        }

        var eligibleBosses = bossEnemies
            .Where(boss => boss.waveToStartSpawning <= GameplayManager.Get.currentWave &&
                           GameplayManager.Get.currentWave % bossOnEveryWave == 0)
            .ToList();

        // If there are any eligible bosses, pick a random one
        if (eligibleBosses.Count > 0)
        {
            System.Random random = new System.Random();
            /*if (selectedEnemies.Count > 0)
                selectedEnemies[Random.Range((selectedEnemies.Count - 10), selectedEnemies.Count)] = (eligibleBosses[Random.Range(0, eligibleBosses.Count)],false);
            else
                selectedEnemies.Add((eligibleBosses[Random.Range(0, eligibleBosses.Count)], false));
            */

            if (selectedEnemies.Count > 0)
            {
                int minIndex = Mathf.Max(0, selectedEnemies.Count - 10);
                int index = Random.Range(minIndex, selectedEnemies.Count);

                selectedEnemies[index] =
                    (eligibleBosses[Random.Range(0, eligibleBosses.Count)], false);
            }
            else
            {
                selectedEnemies.Add(
                    (eligibleBosses[Random.Range(0, eligibleBosses.Count)], false)
                );
            }

        }


        return selectedEnemies;
    }
}