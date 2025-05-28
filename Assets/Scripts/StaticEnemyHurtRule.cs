using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject / Spawner / StaticEnemyHurtRule")]
public class StaticEnemyHurtRule : SpawnRuleSO
{
    public SpawnLocation spawnPosition;
    [Tooltip("The enemy prefab that will spawn")]
    public GameObject Prefab;
    public int maxSpawned;
    public int minSpawned;
    private Vector2 _infectedCellPos;



    [Tooltip("The time it takes to spawn")]
    public float time;

    [Tooltip("Determines the chance of higher values")]
    public float scale;
    public override bool canSpawn(Spawner _spawner)
    {
        foreach (var staticEnemy in _spawner._AstaticenemyOB)
        {
            if (staticEnemy == null) continue;

            var health = staticEnemy.GetComponent<Health>();

            if (health != null)
            {
                if (health.currentHealth < health.maxHealth)
                {
                    _infectedCellPos = (Vector2)staticEnemy.transform.position;
                    return true;
                }
            }
        }
        return false;
    }
    public override int ammountToSpawn(Spawner _spawner)
    {
        int germCount = _spawner._AgermOB.Count;
        return _spawner.CalculateEnemySpawnCount(germCount, scale, minSpawned, maxSpawned);
    }

    public override GameObject prefab() => Prefab;
    public override float timeToSpawn() => time;

    public override Vector2 GetSpawnPosition(Spawner _spawner)
    {
        switch (spawnPosition)
        {
            case SpawnLocation.Player:
                return _spawner.Player.transform.position;
            case SpawnLocation.Random:
                return GameManager.randomPos();
            case SpawnLocation.InfectedCell:
                return _infectedCellPos;
        }
        return Vector2.zero;
    }
}
