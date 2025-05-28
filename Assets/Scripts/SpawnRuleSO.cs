using UnityEngine;

public enum SpawnLocation
{
    Player,
    Random,
    InfectedCell,
}


public abstract class SpawnRuleSO : ScriptableObject
{
    public abstract bool canSpawn(Spawner _spawner);
    public abstract int ammountToSpawn(Spawner _spawner);
    public abstract GameObject prefab();
    public abstract Vector2 GetSpawnPosition(Spawner _spawner);
    public abstract float timeToSpawn();
}
