using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Tree
{
    private TreeNode root;
    private List<TreeNode> _allNodes = new List<TreeNode>();

    public Tree(TreeNode root)
    {
        this.root = root;
        _allNodes.Add(root);
    }

    public List<TreeNode> Nodes => _allNodes;

    public void AddChild(TreeNode parent, TreeNode child)
    {
        parent.children.Add(child);
        _allNodes.Add(child);
    }

    public TreeNode GetClosestNode(TreeNode target)
    {
        return GetClosestNode(target.position);
    }

    public TreeNode GetClosestNode(Vector2 point)
    {
        var closest = _allNodes[0];
        var minDistance = Vector2.Distance(closest.position, point);
        for (var i = 1; i < _allNodes.Count; i++)
        {
            var distance = Vector2.Distance(_allNodes[i].position, point);
            if (distance < minDistance)
            {
                closest = _allNodes[i];
                minDistance = distance;
            }
        }

        return closest;
    }
}
