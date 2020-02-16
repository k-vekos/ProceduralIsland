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
        var closest = _allNodes[0];
        var minDistance = Vector2.Distance(closest.position, target.position);
        for (var i = 1; i < _allNodes.Count; i++)
        {
            var distance = Vector2.Distance(_allNodes[i].position, target.position);
            //Debug.Log($"Distance from #{i} to #{_allNodes.Count} is {distance}");
            if (distance < minDistance)
            {
                closest = _allNodes[i];
                minDistance = distance;
            }
        }

        //Debug.Log($"Closest to #{_allNodes.Count} found to be #{closest.nodeIndex}");
        return closest;
    }
}
