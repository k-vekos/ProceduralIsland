using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Tree
{
    public TreeNode root;
    public List<TreeNode> allNodes = new List<TreeNode>();

    public Tree(TreeNode root)
    {
        this.root = root;
        allNodes.Add(root);
    }
    
    public void AddChild(TreeNode parent, TreeNode child)
    {
        parent.children.Add(child);
        allNodes.Add(child);
    }

    public TreeNode GetClosestNode(TreeNode target)
    {
        var closest = allNodes[0];
        var minDistance = Vector2.Distance(closest.position, target.position);
        for (var i = 1; i < allNodes.Count; i++)
        {
            var distance = Vector2.Distance(allNodes[i].position, target.position);
            if (distance < minDistance)
                closest = allNodes[i];
        }

        return closest;
    }
}
