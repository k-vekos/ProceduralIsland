using UnityEngine;

public class TreeRenderer : MonoBehaviour
{
    public int iterations = 3;
    public float size = 100f;
    public GameObject nodeObject;
    public LineRenderer lineRendererObject;
    public float growthLength = 0.1f;

    private Tree _tree;

    void Start()
    {
        _tree = RandomTreeBuilder.GetRandomTree(iterations, growthLength);
        SpawnNodesAndLines();
    }

    void SpawnNodesAndLines()
    {
        for (var i = 0; i < _tree.Nodes.Count; i++)
        {
            var treeNode = _tree.Nodes[i];
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
        var color = Color.Lerp(Color.green, Color.red, (float) node.nodeIndex / iterations);
        lineRenderer.startColor = color;
        lineRenderer.endColor = color;
    }
    
    private Vector3 GetNodeWorldPosition(TreeNode treeNode)
    {
        return transform.TransformPoint(new Vector3(treeNode.position.x, 0f, treeNode.position.y) * size);
    }
}