using UnityEngine;

public class ConditionNode : BTNode
{
	private System.Func<bool> condition;
	public ConditionNode(System.Func<bool> cond){condition = cond;}
	public override bool Execute(){return condition();}
}
