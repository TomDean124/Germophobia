using UnityEngine;
using System.Linq;

[CreateAssetMenu(menuName = "ScriptableObject / GermManager")]
public class GermManager : ScriptableObject
{
	public GermData[] GermTypes;

    [HideInInspector] public string[] germTypeNames;


    [System.Serializable]
	public struct GermData{
		public string Name;
		public GameObject Prefab;
		public float Speed;
		public float maxSpeed; 
		public float turnSpeed;
		public float ViewRadius; 
		public int defaultMaxHealth; 
		public int defaultMaxHunger;
		public float ChanceOfMutation;
		public float NeededSpawnTime; 
		public float searchDelay;
		public float attackRange; 
		public float wanderStrength;
		public int maxSpawnRate;
		public float hungerRate; 
		public float starvingDamageRate;
		public float recallStopDistance;

		public float recallSpeedMultiplier;
        public float executionOptionHealth;

		public enum germTags { Germ };
        public germTags Tag;
	}

	public string[] GetAllGermNames(){
		return GermTypes.Select(g => g.Name).ToArray();
	}
}
