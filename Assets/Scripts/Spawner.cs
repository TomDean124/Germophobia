using UnityEngine;
using System.Collections.Generic;

public class Spawner : MonoBehaviour
{
    [Header("ScriptableObjects References")]
    public GermManager germManager;
    public MutationManager mutationManager;
    public EnemyManager enemyManager;

    [Header("GameObject References")]
    public GameObject Player;

    [Header("Spawning Variables")]
    public float scalingFactor;
    public SpawnRuleSO[] enemySpawnRules;
    private float[] _StaticEnemyTypesSpawnRadiuses;

    [Header("Private Variables")]
    private float _germSpawnTimer;
    private float _enemySpawnTimer;
    public float _staticEnemyTimer;
    private float _germMaxTimerValue;
    private float _enemyMaxTimerValue;

    private int[] _currentGermsSpawned;
    private int[] _currentEnemiesSpawned;
    private int[] _currentStaticEnemiesSpawned;
    private float[] _enemySpawnTimers;
    private float[] _germSpawnTimers;
    private float[] _factoryTypeSpawnTimers;

    private float[] _germTimerOrigValues;
    private float[] _enemyTimerOrigValues;
    private float[] _staticEnemyTimerOrigValues;
    private float[] _factoryTypeOrigValues;
    public List<float> _AfactoryTypeTimers = new List<float>();
    public List<float> _AfactoryTypeTimersOrigValues = new List<float>();

    private float[] _StaticEnemyTypesSpawnTimers;

    private int _arrayNumber;

    [Header("Lists")]
    [HideInInspector] public List<GameObject> _AgermOB = new List<GameObject>();
    [HideInInspector] public List<GameObject> _AenemyOB = new List<GameObject>();
    public List<GameObject> _AstaticenemyOB = new List<GameObject>();
    [HideInInspector] public List<GameObject> _AFactories = new List<GameObject>();

    private float currentMaxTime = 0;

    void Start()
    {
        _currentGermsSpawned = new int[germManager.GermTypes.Length];
        _currentEnemiesSpawned = new int[enemyManager.EnemyTypes.Length];
        _currentStaticEnemiesSpawned = new int[enemyManager.staticEnemyTypes.Length];
        _germSpawnTimers = new float[germManager.GermTypes.Length];
        _enemySpawnTimers = new float[enemyManager.EnemyTypes.Length];
        _StaticEnemyTypesSpawnTimers = new float[enemyManager.staticEnemyTypes.Length];
        _StaticEnemyTypesSpawnRadiuses = new float[enemyManager.staticEnemyTypes.Length];
        _enemySpawnTimers = TimeGenerator.GenerateTimerValues(germManager, enemyManager, _enemySpawnTimers, true, false);
        _germSpawnTimers = TimeGenerator.GenerateTimerValues(germManager, enemyManager, _germSpawnTimers, false, false);
        _StaticEnemyTypesSpawnTimers = TimeGenerator.GenerateTimerValues(germManager, enemyManager, _StaticEnemyTypesSpawnTimers, false, true);
        _germTimerOrigValues = (float[])_germSpawnTimers.Clone();
        _staticEnemyTimerOrigValues = (float[])_StaticEnemyTypesSpawnTimers.Clone();
        _enemyTimerOrigValues = (float[])_enemySpawnTimers.Clone();
    }

    void Update()
    {
        if (Player != null)
        {
            _germSpawnTimer += Time.deltaTime;
            _enemySpawnTimer += Time.deltaTime;
            _staticEnemyTimer += Time.deltaTime;
            DecreaseTimer();

            DefaultSpawnGerms();
            RuleBasedSpawnEnemies();
            DefaultSpawnStaticEnemies();
        }
    }

    public void DefaultSpawnGerms()
    {
        for (int i = 0; i < germManager.GermTypes.Length; i++)
        {
            if (_germSpawnTimers[i] <= 0 && _AgermOB.Count < germManager.GermTypes[i].maxSpawnRate)
            {
                Vector2 spawnPos = Player.transform.position;
                GameObject germ = Instantiate(germManager.GermTypes[i].Prefab, spawnPos, Quaternion.identity);
                
                var germData = germManager.GermTypes[i];
                germ.tag = germData.Tag.ToString();
                germ.GetComponent<Boid>().SetupBoids(germData.Speed, germData.maxSpeed);
                _AgermOB.Add(germ);
                _currentGermsSpawned[i]++;

                GermAI germAI = germ.GetComponent<GermAI>();
                germ.GetComponent<Hunger>().hungerRate = germData.hungerRate;
                germ.GetComponent<Hunger>().starvingDamageRate = germData.starvingDamageRate;
                var mutations = GameManager.CheckForValidMutations(mutationManager, MutationManager.MutationTypes.applyToType.Germs);

                if (mutations.HasValue)
                {
                    GameManager.ApplyMutations(germ, mutations.Value);
                }
                else
                {
                    germ.GetComponent<Health>().maxHealth = germData.defaultMaxHealth;
                    germ.GetComponent<Hunger>().maxHunger = germData.defaultMaxHunger;
                }

                if (germAI != null)
                {
                    germAI.Player = Player;
                    germAI.executionOptionHealth = germData.executionOptionHealth;
                    germAI.moveSpeed = germData.Speed;
                    germAI.germDetectionRange = germData.ViewRadius;
                    germAI.searchDelay = germData.searchDelay;
                    germAI.wanderStrength = germData.wanderStrength;
                    germAI.attackRange = germData.attackRange;
                    germAI.recallStopDistance = germData.recallStopDistance;
                    germAI.recallSpeedMultiplier = germData.recallSpeedMultiplier;
                }
                _germSpawnTimers[i] = _germTimerOrigValues[i];
            }
        }
    }

    private void RuleBasedSpawnEnemies()
    {
        for (int i = 0; i < enemySpawnRules.Length; i++)
        {
            var rule = enemySpawnRules[i];
            if (rule == null) continue;

            if (rule.canSpawn(this))
            {
                int amount = rule.ammountToSpawn(this);
                GameObject enemyPrefab = rule.prefab();
                float timer = rule.timeToSpawn();

                int maxAllowed = int.MaxValue;
                if (i < enemyManager.EnemyTypes.Length && i < _currentEnemiesSpawned.Length)
                {
                    maxAllowed = enemyManager.EnemyTypes[i].maxSpawnRate - _currentEnemiesSpawned[i];
                }

                amount = Mathf.Clamp(amount, 0, maxAllowed);

                if (i < _enemySpawnTimers.Length && _enemySpawnTimers[i] <= 0 && amount > 0)
                {
                    for (int j = 0; j < amount; j++)
                    {
                        Vector2 spawnPos = rule.GetSpawnPosition(this);
                        GameObject enemy = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
                        _AenemyOB.Add(enemy);

                        if (i < enemyManager.EnemyTypes.Length)
                        {
                            var enemyData = enemyManager.EnemyTypes[i];
                            var enemyAI = enemy.GetComponent<EnemyAI>();

                            var mutations = GameManager.CheckForValidMutations(mutationManager, MutationManager.MutationTypes.applyToType.Enemy);
                            if (mutations.HasValue)
                            {
                                GameManager.ApplyMutations(enemy, mutations.Value);
                            }
                            else
                            {
                                enemy.GetComponent<Health>().maxHealth = enemyData.defaultMaxHealth;
                            }

                            if (enemyAI != null)
                            {
                                enemyAI.Player = Player;
                                enemyAI.playerDetectionRange = enemyData.playerDetectionRange;
                                enemyAI.germDetectionRange = enemyData.germDetectionRange;
                                enemyAI.moveSpeed = enemyData.moveSpeed;
                                enemyAI.patrolSpeed = enemyData.patrolSpeed;
                                enemyAI.searchDelay = enemyData.searchDelay;
                                enemyAI.wanderStrength = enemyData.wanderStrength;
                                enemyAI.germAttackRange = enemyData.germAttackRange;
                                enemyAI.playerAttackRange = enemyData.playerAttackRange;
                            }
                        }
                    }

                    if (i < _currentEnemiesSpawned.Length) _currentEnemiesSpawned[i] += amount;
                    if (i < _enemySpawnTimers.Length) _enemySpawnTimers[i] = timer;
                }
            }
        }
    }

    public void DefaultSpawnStaticEnemies()
    {
        for (int i = 0; i < enemyManager.staticEnemyTypes.Length; i++)
        {
            if (_StaticEnemyTypesSpawnTimers[i] <= 0 && _currentStaticEnemiesSpawned[i] < enemyManager.staticEnemyTypes[i].maxSpawnRate)
            {
                Vector2 spawnPos = GameManager.randomPos();
                GameObject staticEnemy = Instantiate(enemyManager.staticEnemyTypes[i].prefab, spawnPos, Quaternion.identity);

                var staticEnemyData = enemyManager.staticEnemyTypes[i];
                var mutations = GameManager.CheckForValidMutations(mutationManager, MutationManager.MutationTypes.applyToType.staticEnemy);
                if (mutations.HasValue)
                {
                    GameManager.ApplyMutations(staticEnemy, mutations.Value);
                }
                else
                {
                    staticEnemy.GetComponent<Health>().maxHealth = staticEnemyData.defaultMaxHealth;
                }

                _AstaticenemyOB.Add(staticEnemy);
                _currentStaticEnemiesSpawned[i]++;
                _StaticEnemyTypesSpawnTimers[i] = _staticEnemyTimerOrigValues[i];
            }
        }
    }

    public void DecreaseTimer()
    {
        for (int i = 0; i < _enemySpawnTimers.Length; i++)
        {
            if (_enemySpawnTimers[i] > 0)
                _enemySpawnTimers[i] -= Time.deltaTime;
        }

        for (int j = 0; j < _germSpawnTimers.Length; j++)
        {
            if (_germSpawnTimers[j] > 0)
                _germSpawnTimers[j] -= Time.deltaTime;
        }

        for (int i = 0; i < _StaticEnemyTypesSpawnTimers.Length; i++)
        {
            if (_StaticEnemyTypesSpawnTimers[i] > 0)
                _StaticEnemyTypesSpawnTimers[i] -= Time.deltaTime;
        }

        for (int i = 0; i < _AfactoryTypeTimers.Count; i++)
        {
            if (_AfactoryTypeTimers[i] > 0)
                _AfactoryTypeTimers[i] -= Time.deltaTime;
        }
    }

    public void SpawnFactoryObject(FactoryTypeSO _factoryType, Vector2 _spawnPos)
    {
        var _prefab = _factoryType.prefabToSpawn;
        float time = _factoryType.replicationSpeed;
        GameObject obj = Instantiate(_prefab, _spawnPos, Quaternion.identity);
        _AFactories.Add(obj);
        _AfactoryTypeTimers.Add(time);
        _AfactoryTypeTimersOrigValues.Add(time);

    if (_factoryType.typeToSpawn == FactoryTypeSO.spawningEntityType.Germ)
    {
    GameObject germ = Instantiate(germManager.GermTypes[0].Prefab, _spawnPos, Quaternion.identity);
    var germData = germManager.GermTypes[0];
    GermAI germAI = germ.GetComponent<GermAI>();
    var boid = germ.GetComponent<Boid>();
    if (boid != null)
    {
        boid.SetupBoids(germData.Speed, germData.maxSpeed);
    }

    germ.GetComponent<Hunger>().hungerRate = germData.hungerRate;
    germ.GetComponent<Hunger>().starvingDamageRate = germData.starvingDamageRate;
    var mutations = GameManager.CheckForValidMutations(mutationManager, MutationManager.MutationTypes.applyToType.Germs);

    if (mutations.HasValue)
    {
        GameManager.ApplyMutations(germ, mutations.Value);
    }
    else
    {
        germ.GetComponent<Health>().maxHealth = germData.defaultMaxHealth;
        germ.GetComponent<Hunger>().maxHunger = germData.defaultMaxHunger;
    }

    if (germAI != null)
    {
        germAI.Player = Player;
        germAI.executionOptionHealth = germData.executionOptionHealth;
        germAI.moveSpeed = germData.Speed;
        germAI.germDetectionRange = germData.ViewRadius;
        germAI.searchDelay = germData.searchDelay;
        germAI.wanderStrength = germData.wanderStrength;
        germAI.attackRange = germData.attackRange;
        germAI.recallStopDistance = germData.recallStopDistance;
        germAI.recallSpeedMultiplier = germData.recallSpeedMultiplier;
    }

    _AgermOB.Add(germ);
}

    }

    public int CalculateEnemySpawnCount(int GermCount, float scalingFactor, int minSpawnCount, int maxEnemySpawned)
    {
        int calculatedSpawnCount = Mathf.RoundToInt(Mathf.Pow(GermCount, scalingFactor));
        calculatedSpawnCount = Mathf.Clamp(calculatedSpawnCount, minSpawnCount, maxEnemySpawned);
        return calculatedSpawnCount;
    }

    public int CalculateStaticEnemySpawnCount(int GermCount, float scalingFactor, int minSpawnCount, int maxSpawnCount)
    {
        int calculatedSpawnCount = Mathf.RoundToInt(Mathf.Pow(GermCount, scalingFactor));
        calculatedSpawnCount = Mathf.Clamp(calculatedSpawnCount, minSpawnCount, maxSpawnCount);
        return calculatedSpawnCount;
    }
}
