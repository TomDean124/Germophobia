using UnityEngine;

public class ActionNode : BTNode
{
	private System.Action action;
	public ActionNode(System.Action act){action = act;}
	public override bool Execute(){action(); return true;}
}
