using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject / EnemyManager")]
public class EnemyManager : ScriptableObject 
{	
    public EnemyData[] EnemyTypes; 
    public StaticEnemyData[] staticEnemyTypes;

    [System.Serializable]
    public struct EnemyData
    {
        public string EnemyName;
        public GameObject EnemyPrefab;
        public float moveSpeed; 
        public int defaultMaxHealth; 
        public float playerDetectionRange;
        public float germDetectionRange;
        public float patrolSpeed; 
        public float searchDelay;
        public float wanderStrength;
        public bool ExplodesOnDeath; 
        public bool CanBeAttacked;
        public float NeededSpawnTime;
        public int maxSpawnRate; 
        public int spawningOrder; 
        public float germAttackRange; 
        public float playerAttackRange;
        public enum enemyTag {Macrophage, Neutrophil}; //add more tags for more enemies;
        public enemyTag Tag; 
    }

    [System.Serializable]
    public struct StaticEnemyData
    {
        public string name;

        public GameObject prefab;
        public int defaultMaxHealth; 
        public bool ExplodesOnDeath;
        public float NeededSpawnTime;
        public int maxSpawnRate; 
        public float spawnRadius;
        public float importance;

        //Contains all of the data pertaining to how it will die
        public DeathManagerSO deathData;
    }


}