using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeNode
{
    public Vector2 position;
    public List<TreeNode> children = new List<TreeNode>();
    public TreeNode parent = null;

    public TreeNode(float x, float y)
    {
        position = new Vector2(x, y);
    }

    public TreeNode(float x, float y, TreeNode parent) : this(x, y)
    {
        this.parent = parent;
    }
}
