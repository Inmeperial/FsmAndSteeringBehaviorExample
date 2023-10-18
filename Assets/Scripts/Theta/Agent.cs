using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Agent : MonoBehaviour
{
    private LayerMask _layersInSight;
    private Node _initNode;
    private Node _endNode;
    private List<Node> _list;
    Theta<Node> _theta = new Theta<Node>();

    private void Start()
    {
        // TestForNodes(); // Only Testing the nodes.
    }

    public LayerMask LayersInSight
    {
        get { return _layersInSight; }
        set { _layersInSight = value; }
    }

    private void TestForNodes()
    {
        for (int i = 0; i < this.transform.childCount; i++)
        {
            this.transform.GetChild(i).GetComponent<Node>().SetNeightbourds();
        }
    }

    public List<Node> GetPathFindingTheta(Node init, Node end)
    {
        _initNode = init;
        _endNode = end;

        _list = _theta.Run(_initNode, Satisfies, GetNeightAstar, Heuristic, InSight, GrandCost);

        return _list;
    }

    bool Satisfies(Node curr)
    {
        return curr.Equals(_endNode);
    }

    float Heuristic(Node curr)
    {
        float cost = 0;
        cost += Vector3.Distance(curr.transform.position, _endNode.transform.position);
        return cost;
    }

    float GrandCost(Node grandFather, Node grandChild)
    {
        return Vector3.Distance(grandFather.transform.position, grandChild.transform.position);
    }

    bool InSight(Node grandFather, Node grandChild)
    {
        RaycastHit hit;
        var dir = (grandChild.transform.position - grandFather.transform.position);
        if (Physics.Raycast(grandFather.transform.position, dir.normalized, out hit, dir.magnitude, _layersInSight))
        {
            return false;
        }
        return true;
    }

    Dictionary<Node, float> GetNeightAstar(Node curr)
    {
        var dic = new Dictionary<Node, float>();

        foreach (var nodeNeigh in curr.Neightbourds)
        {
            var cost = Vector3.Distance(nodeNeigh.transform.position, curr.transform.position);
            dic.Add(nodeNeigh, cost);
        }
        return dic;
    }

}
