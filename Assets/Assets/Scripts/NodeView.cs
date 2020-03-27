using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeView : MonoBehaviour
{

    public GameObject tile;
    Node m_node;

    //border of each node
    [Range(0, 0.5f)]
    public float borderSize = 0.15f;

    //initialze the nodeview with the corresponding node
    public void Initialize(Node node)
    {
        if (tile != null)
        {
            //rename nodeview object to include node's x and y
            gameObject.name = "Node (" + node.xIndex + "," + node.yIndex + ")";
            //move nodeview to vector3 position
            gameObject.transform.position = node.position;
            tile.transform.localScale = new Vector3(1f - borderSize, 1f, 1f - borderSize);
            //for later use
            m_node = node;
        }
    }
    //color gameobject
    void ColorNode(Color color, GameObject go)
    {
        if (go != null)
        {
            Renderer goRenderer = go.GetComponent<Renderer>();

            if (goRenderer != null)
            {
                goRenderer.material.color = color;
            }
        }
    }

    // color the tile
    public void ColorNode(Color color)
    {
        ColorNode(color, tile);
    }

    //toggle object
    void EnableObject(GameObject go, bool state)
    {
        if (go != null)
        {
            go.SetActive(state);
        }
    }
}
