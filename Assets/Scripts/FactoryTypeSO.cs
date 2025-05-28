using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Factory")]
public class FactoryTypeSO : ScriptableObject
{
    public string displayName;
    public Sprite icon;
    public int buildCost;
    [Tooltip("Pertains To The Factory Prefab not what it will be spawning")]
    public GameObject prefabToSpawn;
    public float replicationSpeed; 
    public enum spawningEntityType {Germ, StaticEnemy}; 
    public spawningEntityType typeToSpawn;
}
