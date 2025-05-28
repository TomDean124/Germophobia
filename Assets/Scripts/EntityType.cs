using UnityEngine;

public enum EntityTypes
{
    Enemy,
    Germ,
    StaticEnemy
}

public class EntityType : MonoBehaviour
{
	public EntityTypes type;

    private void OnEnable()
    {
        Spawner spawner = FindObjectOfType<Spawner>();
        if (spawner != null)
        {
            switch (type)
            {
                case EntityTypes.Enemy: spawner._AenemyOB.Add(gameObject); break;
                case EntityTypes.Germ: spawner._AgermOB.Add(gameObject); break;
                case EntityTypes.StaticEnemy: spawner._AstaticenemyOB.Add(gameObject); break;
            }
        }
    }

    private void OnDisable()
    {
        GameManager.RemoveFromList(gameObject, type);
    }
}
