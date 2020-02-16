using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeWalker : MonoBehaviour
{
    public int iterations = 3;
    public float size = 100f;
    public GameObject nodeObject;
    public LineRenderer lineRendererObject;
    public float growthLength = 0.1f;
    public float updateInterval = 0.1f;

    private Tree _tree;
    private YieldInstruction _updateWait;
    private Queue<TreeNode> _nodesToSpawn = new Queue<TreeNode>();
    private int _iteration;

    // Start is called before the first frame update
    void Start()
    {
        _updateWait = new WaitForSeconds(updateInterval);
        
        StartCoroutine(WalkCoroutine());
    }

    // Update is called once per frame
    void Update()
    {
        // Place any queued nodes in scene
        while (_nodesToSpawn.Count > 0)
        {
            var treeNode = _nodesToSpawn.Dequeue();
            var newNodeObject = Instantiate(nodeObject, GetNodeWorldPosition(treeNode), Quaternion.identity, transform);
            newNodeObject.name = "Node_" + treeNode.nodeIndex;

            if (treeNode.parent != null)
                AddLineRenderer(treeNode, newNodeObject);
        }
    }

    private void AddLineRenderer(TreeNode node, GameObject nodeGameObject)
    {
        // Add line renderer
        var lineRenderer = Instantiate(lineRendererObject, Vector3.zero, Quaternion.identity, nodeGameObject.transform);
                
        // Add line point at parent position
        lineRenderer.positionCount = 2;
        lineRenderer.SetPositions(new[]
        {
            GetNodeWorldPosition(node.parent),
            GetNodeWorldPosition(node)
        });
                
        // Color line according to iteration #
        var color = Color.Lerp(Color.green, Color.red, (float) _iteration / iterations);
        lineRenderer.startColor = color;
        lineRenderer.endColor = color;
    }

    private Vector3 GetNodeWorldPosition(TreeNode treeNode)
    {
        return transform.TransformPoint(new Vector3(treeNode.position.x, 0f, treeNode.position.y) * size);
    }

    private IEnumerator WalkCoroutine()
    {
        // Place first node randomly in area and add to tree
        var rootNode = new TreeNode(Random.Range(0f, 1f), Random.Range(0f, 1f), 0);
        _tree = new Tree(rootNode);
        _nodesToSpawn.Enqueue(rootNode);

        yield return _updateWait;

        _iteration = 0;
        while (_iteration < iterations)
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
                _nodesToSpawn.Enqueue(extension);
            }
            
            _iteration++;
            yield return _updateWait;
        }
        
        Debug.Log($"Finished creating tree with {_tree.Nodes.Count} nodes in {_iteration} iterations.");
    }

    private TreeNode RandomSample()
    {
        return new TreeNode(Random.Range(0f, 1f), Random.Range(0f, 1f), -1);
    }

    private TreeNode ExtendToward(TreeNode from, TreeNode towards)
    {
        var dir = towards.position - from.position;
        var dist = dir.magnitude;
        dir = dir.normalized;
        dir = from.position + (dir * Mathf.Min(growthLength, dist));
        return new TreeNode(dir.x, dir.y, _iteration + 1, from);
    }
}
