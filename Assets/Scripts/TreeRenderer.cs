using UnityEditor.UI;
using UnityEngine;

public class TreeRenderer : MonoBehaviour
{
    public GameObject nodeObject;
    public LineRenderer lineRendererObject;

    //private Tree _tree;
    private GameObject _container;

    public void SpawnNodesAndLines(Tree tree, int mapSize, Terrain terrain)
    {
        // Destroy any existing container
        if (_container != null)
            DestroyImmediate(_container);
        else
        {
            foreach (Transform child in transform) {
                DestroyImmediate(child.gameObject);
            }
        }

        // Add empty child
        _container = new GameObject("Container");
        _container.transform.SetParent(this.transform);
        _container.transform.localPosition = Vector3.zero;
        
        for (var i = 0; i < tree.Nodes.Count; i++)
        {
            var treeNode = tree.Nodes[i];
            var newNodeObject = Instantiate(nodeObject, GetNodeWorldPosition(treeNode, mapSize, terrain), Quaternion.identity,
                _container.transform);
            newNodeObject.name = "Node_" + treeNode.nodeIndex;
            
            if (treeNode.parent != null)
                AddLineRenderer(treeNode, newNodeObject, mapSize, terrain);
        }
    }
    
    private void AddLineRenderer(TreeNode node, GameObject nodeGameObject, int mapSize, Terrain terrain)
    {
        // Add line renderer
        var lineRenderer =
            Instantiate(lineRendererObject, Vector3.zero, Quaternion.identity, nodeGameObject.transform);
                
        // Add line point at parent position
        lineRenderer.positionCount = 2;
        lineRenderer.SetPositions(new[]
        {
            GetNodeWorldPosition(node.parent, mapSize, terrain),
            GetNodeWorldPosition(node, mapSize, terrain)
        });
                
        // Color line according to iteration #
        var color = Color.blue;
        lineRenderer.startColor = color;
        lineRenderer.endColor = color;
        lineRenderer.startWidth = 3f;
        lineRenderer.endWidth = 3f;
    }
    
    private Vector3 GetNodeWorldPosition(TreeNode treeNode, int mapSize, Terrain terrain)
    {
        var normalX = Mathf.InverseLerp(0, mapSize, treeNode.position.x);
        var normalY = Mathf.InverseLerp(0, mapSize, treeNode.position.y);

        /*var remappedX = Mathf.Lerp(0, terrainSize, normalX);
        var remappedY = Mathf.Lerp(0, terrainSize, normalY);*/

        return new Vector3(
            normalX * terrain.terrainData.size.x,
            terrain.terrainData.size.y,
            normalY * terrain.terrainData.size.x);
        //return transform.TransformPoint(new Vector3(treeNode.position.x, 0f, treeNode.position.y));
    }
}