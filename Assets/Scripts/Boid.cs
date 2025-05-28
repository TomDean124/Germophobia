using UnityEngine;

public class Boid : MonoBehaviour
{
	public Vector2 velocity; 
	public Vector2 acceleration;
	private float _maxSpeed;
	private float _speed; 

	void Update(){
		velocity += acceleration * _speed;
		velocity = Vector2.ClampMagnitude(velocity, _maxSpeed);
        Vector2 movement = velocity * Time.deltaTime;
        transform.position = new Vector3(transform.position.x + movement.x, transform.position.y + movement.y, transform.position.z);

		acceleration = Vector2.zero;
	}

	public void SetupBoids(float speed, float maxSpeed){
		_speed = speed;
		_maxSpeed = maxSpeed;
		velocity = Random.insideUnitCircle.normalized * speed;
		acceleration = Vector2.zero;
	}

	public void ApplyForce(Vector2 force){
		acceleration += force;
	}
}
