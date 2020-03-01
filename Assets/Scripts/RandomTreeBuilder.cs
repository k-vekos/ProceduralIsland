using UnityEngine;

public static class RandomTreeBuilder
{
    public static Tree GetRandomTree(int iterations, float maxBranchLength, float size)
    {
        // Place first node randomly in area and add to tree
        var root = RandomSample(0, size);
        var tree = new Tree(root);
        
        for (var i = 1; i < iterations; i++)
        {
            GrowTree(tree, i, maxBranchLength, size);
        }
        
        return tree;
    }
    
    private static void GrowTree(Tree tree, int iteration, float growthLength, float size)
    {
        // Get a random sample
        var sample = RandomSample(iteration, size);
        
        // Get the closest node in the tree to the sample
        var closest = tree.GetClosestNode(sample);

        // Create a new node between the closest node and the sample.
        var extension = ExtendToward(closest, sample, growthLength, iteration);
        
        // If we managed to create a new node, add it to the tree.
        if (extension != null)
        {
            tree.AddChild(closest, extension);
        }
        
        // Debug.Log($"Finished creating tree with {tree.Nodes.Count} nodes in {iteration} iterations.");
    }

    private static TreeNode RandomSample(int nodeIndex, float size)
    {
        return new TreeNode(Random.Range(0f, size), Random.Range(0f, size), nodeIndex);
    }

    private static TreeNode ExtendToward(TreeNode from, TreeNode towards, float growthLength, int iteration)
    {
        var dir = towards.position - from.position;
        var dist = dir.magnitude;
        dir = dir.normalized;
        dir = from.position + (dir * Mathf.Min(growthLength, dist));
        return new TreeNode(dir.x, dir.y, iteration, from);
    }
}