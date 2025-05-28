using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject / SpawningManager")]
public class SpawningManager : ScriptableObject
{
	private SpawningManager _instance;
	public SpawningManager instance {get; private set;}
	public int maxGerms;
	public int maxEnemies;

	public float germSpawningInterval; 
	public float macrophageSpawningInterval;
	public float nuetrophilSpawningInterval;
}

