using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects / SoundManagerSO")]
public class SoundManagerSO : ScriptableObject
{
    public AudioClip[] germMovementSFX; //Germ movement sounds
    public AudioClip[] explosionSFX; //Explosion death sounds
    public AudioClip[] spawnerSFX; //Spawning sounds
    public AudioClip[] collectFoodSFX; //Collecting food sounds
    public AudioClip[] upgradeSFX; //Upgrading sounds 
}
