using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject / ReplicationManager")]
public class ReplicationManager : ScriptableObject
{
	public replicationData[] Replication;

	[System.Serializable]
	public struct replicationData
	{
	public string elementName;
	public float healthForReplicationOption; 
	public FactoryType applyToType;
	}

	public enum FactoryType
	{
    HealthyCell
	}

}
