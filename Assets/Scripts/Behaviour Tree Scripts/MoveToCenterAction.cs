using UnityEngine;

public class MoveToCenterAction : BTNode
{
	public GermMovementManager germMovementManager; 

	public override bool Execute(){
		germMovementManager.MoveToCenter();
		return true; 
	}
}
