using UnityEngine;
using System.Collections.Generic;

public class SequenceNode : BTNode
{
    private List<BTNode> children;
    public SequenceNode(List<BTNode> nodes) { children = nodes; }
    public override bool Execute()
    {
        foreach (var node in children)
        {
            if (!node.Execute()) return false;
        }
        return true;
    }
}
