using UnityEngine;

[RequireComponent(typeof(Health))]
public class Hunger : MonoBehaviour
{
	//public variables
	public int maxHunger;
	public float currentHunger;
	public float hungerRate;
	public float starvingDamageRate;
	//bools
	public bool starving = false;

    //private variables
    private Health _health; 

	void Start(){
		currentHunger = maxHunger;
		_health = GetComponent<Health>();
	}

	void Update(){
	decreaseHunger(hungerRate);
	
	if(currentHunger <= 0){
		starving = true; 
		currentHunger = 0; 
	}

	if(currentHunger > maxHunger)
	{
		currentHunger = maxHunger;
	}

	if(starving){
		_health.TakeDamage(starvingDamageRate * Time.deltaTime);
	}
	
	}

	void decreaseHunger(float rate){
	if(!starving && _health != null) currentHunger -= Time.deltaTime * rate;
	}

	public void increaseHunger(float gainAmount){
	Debug.Log("IncreaseHungerCalled! (Hunger.cs)");
	if(currentHunger < maxHunger && _health != null && currentHunger < gainAmount) currentHunger += gainAmount;
	}
}

