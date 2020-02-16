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
    private YieldInstruction updateWait;
    private Queue<TreeNode> nodesToSpawn = new Queue<TreeNode>();
    private int _iteration;

    // Start is called before the first frame update
    void Start()
    {
        updateWait = new WaitForSeconds(updateInterval);
        
        StartCoroutine(WalkCoroutine());
    }

    // Update is called once per frame
    void Update()
    {
        // Place any queued nodes in scene
        while (nodesToSpawn.Count > 0)
        {
            var treeNode = nodesToSpawn.Dequeue();
            var node = Instantiate(nodeObject, transform);
            var targetPos = GetNodeWorldPosition(treeNode);
            node.transform.SetPositionAndRotation(targetPos, Quaternion.identity);

            // Add line renderers
            if (treeNode.parent != null)
            {
                // Add line renderer
                var lineRenderer = Instantiate(lineRendererObject, Vector3.zero, Quaternion.identity, node.transform);
                
                // Add line point at parent position
                lineRenderer.positionCount = 2;
                lineRenderer.SetPositions(new[]
                {
                    GetNodeWorldPosition(treeNode.parent),
                    GetNodeWorldPosition(treeNode)
                });
                
                // Color line according to iteration #
                var color = Color.Lerp(Color.green, Color.red, (float) _iteration / iterations);
                lineRenderer.startColor = color;
                lineRenderer.endColor = color;
            }
        }
    }

    private Vector3 GetNodeWorldPosition(TreeNode treeNode)
    {
        return transform.TransformPoint(new Vector3(treeNode.position.x, 0f, treeNode.position.y) * size);
    }

    private IEnumerator WalkCoroutine()
    {
        // Place first node randomly in area and add to tree
        var rootNode = new TreeNode(Random.Range(0f, 1f), Random.Range(0f, 1f));
        _tree = new Tree(rootNode);
        nodesToSpawn.Enqueue(rootNode);

        yield return updateWait;

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
                nodesToSpawn.Enqueue(extension);
            }
            
            _iteration++;
            yield return updateWait;
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
        return new TreeNode(dir.x, dir.y, from);
    }
}
