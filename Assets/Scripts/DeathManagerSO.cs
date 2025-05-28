using UnityEngine;

[CreateAssetMenu(fileName = "NewDeath", menuName = "ScriptableObject/Deaths")]
public class DeathManagerSO : ScriptableObject
{
public string deathName; 
public bool explodeOnDeath;
public bool useParticles;
public ParticleSystem deathParticles;
}
