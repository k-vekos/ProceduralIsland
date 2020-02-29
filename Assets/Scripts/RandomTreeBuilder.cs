using UnityEngine;

public static class RandomTreeBuilder
{
    public static Tree GetRandomTree(int iterations, float growthLength)
    {
        // Place first node randomly in area and add to tree
        var root = RandomSample(0);
        var tree = new Tree(root);
        
        for (var i = 1; i < iterations; i++)
        {
            GrowTree(tree, i, growthLength);
        }
        
        return tree;
    }
    
    private static void GrowTree(Tree tree, int iteration, float growthLength)
    {
        // Get a random sample
        var sample = RandomSample(iteration);
        
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

    private static TreeNode RandomSample(int nodeIndex)
    {
        return new TreeNode(Random.Range(0f, 1f), Random.Range(0f, 1f), nodeIndex);
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