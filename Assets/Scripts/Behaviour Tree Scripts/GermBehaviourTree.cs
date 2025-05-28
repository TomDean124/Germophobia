using UnityEngine;
using System.Collections.Generic;

public class GermBehaviourTree : MonoBehaviour
{
	private BTNode BTnode;
	public GermMovementManager germMovementManager; 

	void Start(){
		var MoveToCenterAction = new MoveToCenterAction{germMovementManager = germMovementManager};
		var BoidAvoidanceAction = new BoidAvoidanceAction{germMovementManager = germMovementManager};
		var AlignmentAction = new AlignmentAction{germMovementManager = germMovementManager};

		var parrallelNode = new ParallelNode();
		parrallelNode.children = new List<BTNode> {MoveToCenterAction,BoidAvoidanceAction,AlignmentAction};

		BTnode = parrallelNode;
	}

	void Update(){
		BTnode.Execute();
	}

}
