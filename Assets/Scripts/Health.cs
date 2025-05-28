using UnityEngine;

public class Health : MonoBehaviour
{
public float currentHealth;
public int maxHealth;
private Infected _infected;


void Start()
{
	currentHealth = maxHealth;
	_infected = GetComponent<Infected>();
}

void Update(){
	if(currentHealth <= 0){
		Death();
	}
	if(currentHealth < maxHealth){
		if(_infected != null)
		{
			_infected.infected = true;
		}
	}
}

public void TakeDamage(float damage){
	currentHealth -= damage; 
	if(this._infected != null)
	{
		_infected.infected = true;
	}
}

public void GainHealth(float gainAmount){
	if(currentHealth < maxHealth && (currentHealth + gainAmount) <= maxHealth){
	currentHealth += gainAmount;
	}
}

private void Death(){
	Destroy(this.gameObject);
}

}
