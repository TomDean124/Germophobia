using UnityEngine;

public class AlignmentAction : BTNode
{
public GermMovementManager germMovementManager;

public override bool Execute(){
	germMovementManager.Align();
	return true;
}
}
