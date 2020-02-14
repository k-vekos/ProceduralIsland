using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeWalker : MonoBehaviour
{
    public int iterations = 3;
    public float size = 100f;
    public GameObject nodeObject;
    public float growthLength = 0.1f;

    private Tree _tree;
    private YieldInstruction delay = new WaitForSeconds(0.1f);
    private List<TreeNode> nodesToSpawn = new List<TreeNode>();
    
    // Start is called before the first frame update
    void Start()
    {
        Initialize();
        
        // Place nodes in scene
        for (var i = 0; i < _tree.allNodes.Count; i++)
        {
            var treeNode = _tree.allNodes[i];
            var node = Instantiate(nodeObject, transform);
            var targetPos = transform.TransformPoint(new Vector3(treeNode.position.x, 0f, treeNode.position.y) * size);
            node.transform.SetPositionAndRotation(targetPos, Quaternion.identity);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerable Initialize()
    {
        // Place first node randomly in area and add to tree
        var rootNode = new TreeNode(Random.Range(0f, 1f), Random.Range(0f, 1f));
        _tree = new Tree(rootNode);
        nodesToSpawn.Add(rootNode);

        var itr = 0;
        while (itr < iterations)
        {
            // Get a random sample
            var sample = RandomSample();
            
            // Get the closest node in the tree to the sample
            var closest = _tree.GetClosestNode(sample);
            
            // Create a new node between the closest node and the sample.
            var extension = ExtendToward(closest, sample);
            
            // If we managed to create a new node, add it to the tree.
            if (extension != null)
            {
                _tree.AddChild(closest, extension);
                nodesToSpawn.Add(extension);
            }
            
            itr++;
        }
    }

    private TreeNode RandomSample()
    {
        return new TreeNode(Random.Range(0f, 1f), Random.Range(0f, 1f));
    }

    private TreeNode ExtendToward(TreeNode from, TreeNode towards)
    {
        var dir = towards.position - from.position;
        var dist = dir.magnitude;
        dir = dir.normalized;
        dir = from.position + (dir * Mathf.Min(growthLength, dist));
        return new TreeNode(dir.x, dir.y);
    }
}
