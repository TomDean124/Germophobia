using UnityEngine;

public enum soundType
{
    Movement, 
    Spawning, 
    Upgrading, 
    Collecting, 
    Enviromental
};

public class SoundManager : MonoBehaviour
{
    public SoundManagerSO _soundManagerSO;
    public AudioSource sfxAudioSource; 
    public AudioSource musicAudioSource;

    public static SoundManager Instance {get; private set;}

    private void Awake()
    {
        if(Instance != null && Instance == this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this; 
    }


    public void PlayDefinedAudio(soundType type, float volume, bool loop)
    {
        if(type == soundType.Movement)
        {
            var movementSounds = _soundManagerSO.germMovementSFX;
            int numClips = movementSounds.Length; 

            Debug.Log(numClips);

            int randNum = Random.Range(0, numClips);

            sfxAudioSource.clip = movementSounds[randNum];
            sfxAudioSource.volume = volume;
            sfxAudioSource.loop = loop;
            sfxAudioSource.Play();
        }
    } 

    public void PlayAudio(AudioClip _clip, float volume)
    {
        sfxAudioSource.PlayOneShot(_clip, volume);
    }

    

}
