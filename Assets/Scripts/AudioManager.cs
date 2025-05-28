using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/AudioManager")]
public class AudioManager : ScriptableObject
{
	public AudioClip[] ClipAudio;
	public MusicAudio[] Music;

	[System.Serializable]
	public struct AudioClip{
		public string name;
		public AudioSource source; 
		public AudioClip[] clip;
		public float Volume; 
		public bool PlayOnAwake;
	}
	[System.Serializable]
	public struct MusicAudio{
		public string name;
		public AudioSource source; 
		public AudioClip[] clip;
		public float Volume;
		public bool PlayOnAwake;

	}
}
