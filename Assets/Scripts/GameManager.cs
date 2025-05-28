using UnityEngine;
using System.Collections.Generic;
using System.Reflection;

public static class GameManager
{
    public static float[] GenerateStaticEnemyRadiusValues(float[] radiuses, EnemyManager enemyManager)
    {
        for (int i = 0; i < radiuses.Length; i++)
        {
            radiuses[i] = enemyManager.staticEnemyTypes[i].spawnRadius;
        }
        return radiuses;
    }

    public static T[] PopulateEnemyTypeInformation<T>(int fieldIndex, EnemyManager enemyManager)
    {
        if (enemyManager.EnemyTypes == null || enemyManager.EnemyTypes.Length == 0)
        {
            return new T[0];
        }

        var fields = typeof(EnemyManager.EnemyData).GetFields();

        if (fieldIndex < 0 || fieldIndex >= fields.Length)
        {
            return new T[0];
        }

        var field = fields[fieldIndex];
        List<T> values = new List<T>();

        foreach (var enemy in enemyManager.EnemyTypes)
        {
            values.Add((T)field.GetValue(enemy));
        }

        return values.ToArray();
    }

    public static bool areallenemiesdead(List<GameObject> enemyA)
    {
        foreach (GameObject enemy in enemyA)
        {
            if (enemy != null)
            {
                return false;
            }
        }
        return true;
    }

    public static Vector2 GenerateRadius(float radius)
    {
        float angle = Random.Range(0f, 360f);
        float x = Mathf.Cos(angle * Mathf.Deg2Rad) * radius;
        float y = Mathf.Sin(angle * Mathf.Deg2Rad) * radius;
        Vector2 vector2Radius = new Vector2(x, y);
        return vector2Radius;
    }

    public static MutationManager.MutationTypes? CheckForValidMutations(MutationManager manager, MutationManager.MutationTypes.applyToType targetType)
    {
        if (manager.mutationTypes.Length == 0)
        {
            Debug.LogWarning("No Mutations Found In The Array");
            return null;
        }

        float randomNumber = Random.Range(0f, 100f);
        float currentPoint = 0f;

        foreach (var mutation in manager.mutationTypes)
        {
            currentPoint += mutation.mutationChance;
            if (randomNumber <= currentPoint && (mutation.applyToWhatTypes == targetType || mutation.applyToWhatTypes == MutationManager.MutationTypes.applyToType.All))
            {
                return mutation;
            }
        }
        return null; 
    }

    public static void ApplyMutations(GameObject prefab, MutationManager.MutationTypes mutations)
    {
        if (mutations.affectHealth)
        {
            int MaxHealth = prefab.GetComponent<Health>().maxHealth;
            int newMaxHealth = (int)(MaxHealth * mutations.HealthMultiplier);
            prefab.GetComponent<Health>().maxHealth = newMaxHealth;
        }

        if (mutations.affectHunger)
        {
            Hunger hunger = prefab.GetComponent<Hunger>();
            int MaxHunger = hunger.maxHunger;
            int newMaxHunger = (int)(MaxHunger * mutations.maxHungerMultiplier);
            hunger.maxHunger = newMaxHunger;
        }

        if (mutations.affectDamage)
        {
            Hunger hunger = prefab.GetComponent<Hunger>();
            int MaxHunger = hunger.maxHunger;
            int newMaxHunger = (int)(MaxHunger * mutations.maxHungerMultiplier);
            hunger.maxHunger = newMaxHunger;
        }
    }

    public static void RemoveFromList(GameObject gameObject, EntityTypes entityType)
    {
        Spawner spawner = UnityEngine.Object.FindObjectOfType<Spawner>();

        if (spawner == null) return;

        switch (entityType)
        {
            case EntityTypes.Enemy:
                spawner._AenemyOB.Remove(gameObject);
                break;
            case EntityTypes.Germ:
                spawner._AgermOB.Remove(gameObject);
                break;
            case EntityTypes.StaticEnemy:
                spawner._AstaticenemyOB.Remove(gameObject);
                break;
        }
    }

    public static Vector2 randomPos()
    {
        float ranX = Random.Range(-100, 100);
        float ranY = Random.Range(-100, 100);

        Vector2 newPos = new Vector2(ranX, ranY);
        return newPos;
    }

    public static int GetCurrentFoodNeed(Spawner _spawner)
    {
        int foodNeed = _spawner._AgermOB.Count; 

        return foodNeed;   
    }

    public static void IncreaseHungerOfAllInArray(List<GameObject> _array, float amount)
    { 
        for(int i=0; i < _array.Count; i++)
        {
            Hunger _hunger = _array[i].GetComponent<Hunger>();

            if(_hunger != null)
            {
                Debug.Log("Incrasing Hunger Level (GameManager.cs)");
                _hunger.increaseHunger(amount);
            }
        }
    }
}