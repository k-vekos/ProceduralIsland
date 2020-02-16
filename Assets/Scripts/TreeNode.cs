using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeNode
{
    public Vector2 position;
    public List<TreeNode> children = new List<TreeNode>();
    public TreeNode parent;
    public int nodeIndex;

    public TreeNode(float x, float y, int index)
    {
        position = new Vector2(x, y);
        nodeIndex = index;
    }

    public TreeNode(float x, float y, int index, TreeNode parent) : this(x, y, index)
    {
        this.parent = parent;
    }
}
