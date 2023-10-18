using System.Collections.Generic;
using UnityEngine;

public class RayCastForNodes : MonoBehaviour
{
    public LayerMask layerForCheck;
    public TagsForObjectsEnum tagForActivateNode;
    public GameObject nodeStartToCreate, nodeEndToCreate, objToInstantiate = default;
    private List<GameObject> _listOfObjNodes = new List<GameObject>();
    private int _heightForObj = 10;
    private GameObject _parentForNodes;

    public void StartRainAndSetNeightbourds()
    {
        StartRainRaycast(nodeStartToCreate, nodeEndToCreate);
        if (_listOfObjNodes != null && _listOfObjNodes.Count > 0)
        {
            SetNeightbourdsForNodes(_listOfObjNodes);
        }
    }

    public List<GameObject> ListOfObjNodes
    {
        get { return _listOfObjNodes; }
        set { _listOfObjNodes = value; }
    }

    private void StartRainRaycast(GameObject nodeS, GameObject nodeE)
    {
        _parentForNodes = new GameObject("ParentForNodes");
        _parentForNodes.transform.SetParent(this.transform);
        int mapXTotal = ((int)(nodeE.transform.position.x - nodeS.transform.position.x)) / 2;
        int mapZTotal = ((int)(nodeE.transform.position.z - nodeS.transform.position.z)) / 2;
        int mapX = mapXTotal;
        int mapZ = mapZTotal;

        int placeZ = (int)nodeS.transform.position.z;
        int placeX = 0;

        RaycastHit hit;

        for (int i = mapZ; i <= 0; i++)
        {
            for (int j = mapX; j >= 0; j--)
            {
                if (Physics.Raycast(new Vector3(nodeS.transform.position.x + placeX, _heightForObj, placeZ), (nodeS.transform.up * -1), out hit, _heightForObj + 2, layerForCheck))
                {
                    if (hit.transform.tag == tagForActivateNode.ToString())
                    {
                        GameObject obj = Instantiate(objToInstantiate, hit.point, this.transform.rotation, _parentForNodes.transform);
                        obj.AddComponent<BoxCollider>();
                        obj.AddComponent<Node>();
                        obj.layer = LayerMask.NameToLayer("NodesTheta");
                        _listOfObjNodes.Add(obj);
                    }
                }

                placeX += 2;
            }
            placeX = 0;
            mapX = mapXTotal;
            placeZ -= 2;
        }
    }

    private void SetNeightbourdsForNodes(List<GameObject> list)
    {
        foreach (var item in list)
        {
            item.GetComponent<Node>().SetNeightbourds();
        }
    }

    private void DestroyListOfObjects(List<GameObject> list)
    {
        foreach (var item in list)
        {
            Destroy(item);
        }
    }

}
