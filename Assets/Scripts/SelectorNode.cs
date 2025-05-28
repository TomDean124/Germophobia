using System.Collections.Generic;

public class SelectorNode : BTNode
{
    private List<BTNode> children;
    public SelectorNode(List<BTNode> nodes) { children = nodes; }
    public override bool Execute()
    {
        foreach (var node in children)
        {
            if (node.Execute()) return true;
        }
        return false;
    }
}