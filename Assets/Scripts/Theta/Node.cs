using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    private List<Node> _neightbourds = new List<Node>();

    public void SetNeightbourds()
    {
        GetNeightbourd(Vector3.right);
        GetNeightbourd(Vector3.left);
        GetNeightbourd(Vector3.forward);
        GetNeightbourd(Vector3.back);
    }

    public List<Node> Neightbourds
    {
        get { return _neightbourds; }
        set { _neightbourds = value; }
    }


    private void GetNeightbourd(Vector3 dir)
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, dir, out hit, 2.2f))
        {
            if (hit.collider.GetComponent<Node>() != null)
            {
                var node = hit.collider.GetComponent<Node>();
                Neightbourds.Add(node);
            }
        }
    }

    void OnDrawGizmos()
    {
        if (Neightbourds != null)
        {
            foreach (var item in Neightbourds)
            {
                Debug.DrawLine(this.transform.position, item.gameObject.transform.position, Color.red);
            }
        }
    }
}
