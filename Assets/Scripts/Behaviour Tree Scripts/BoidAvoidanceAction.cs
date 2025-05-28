using UnityEngine;

public class BoidAvoidanceAction : BTNode
{
	public GermMovementManager germMovementManager; 

	public override bool Execute(){
		germMovementManager.AvoidOtherBoids();
		return true; 
	}
}
