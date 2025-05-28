using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject / MutationManager")]
public class MutationManager : ScriptableObject
{

public MutationTypes[] mutationTypes;


[System.Serializable]
public struct MutationTypes{
public string mutationName;

[Header("Health Change")]
public bool affectHealth;
public float HealthMultiplier;
[Header("Reproduction Change")]
public bool affectReproduction; 
public float reproductionTimeMultiplier;

[Header("Speed Change")]
public bool affectSpeed; 
public float SpeedMultiplier;

[Header("Hunger Change")]
public bool affectHunger;
public float maxHungerMultiplier; 

[Header("Damage Change")]
public bool affectDamage;

public float damageMultiplier; 

public enum applyToType{staticEnemy, Enemy, Germs, Player, All};
public applyToType applyToWhatTypes;

public float mutationChance;
}

}
