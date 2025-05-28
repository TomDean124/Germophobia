using UnityEngine;
using System.Collections.Generic;

public class ParallelNode : BTNode
{
    public List<BTNode> children; 

    public override bool Execute()
    {
        bool allChildrenSuccess = true;

        foreach (var child in children)
        {
            bool result = child.Execute();

            if (!result)
            {
                allChildrenSuccess = false;
            }
        }
        return allChildrenSuccess;
    }
}
