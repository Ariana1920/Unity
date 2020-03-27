using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//representing a set of NodeViews and attached to Graph gameObject
[RequireComponent(typeof(Graph))]
public class GraphView : MonoBehaviour
{
    public GameObject nodeViewPrefab;

    //2d array of node view
    public NodeView[,] nodeViews;

    // two based color
    public Color baseColor = Color.white;
    public Color wallColor = Color.black;

    public void Initialize(Graph graph)
    {
        if (graph == null)
        {
            Debug.LogWarning("GRAPHVIEW No graph to initialize!");
            return;
        }
        nodeViews = new NodeView[graph.Width, graph.Height];

        foreach (Node n in graph.nodes)
        {
            //create a nodeview for each corresponding node
            GameObject instance = Instantiate(nodeViewPrefab, Vector3.zero, Quaternion.identity);
            NodeView nodeView = instance.GetComponent<NodeView>();

            if (nodeView != null)
            {
                nodeView.Initialize(n);

                //add each nodeview into array
                nodeViews[n.xIndex, n.yIndex] = nodeView; 
                //this way can find each node crresponding to nodeview

                if (n.nodeType == NodeType.Blocked)
                {
                    nodeView.ColorNode(wallColor);
                }
                else
                {
                    nodeView.ColorNode(baseColor);
                }
            }
        }
    }
    // color node view all at once
    public void ColorNodes(List<Node> nodes, Color color)
    {
        foreach (Node n in nodes)
        {
            if (n != null)
            {
                NodeView nodeView = nodeViews[n.xIndex, n.yIndex];

                if (nodeView != null)
                {
                    nodeView.ColorNode(color);
                }
            }
        }
    }
}
