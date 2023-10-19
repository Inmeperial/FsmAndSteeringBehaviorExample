using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinionManager : MonoBehaviour
{
    private List<GameObject> _minionsList;

    public List<GameObject> MinionsList
    {
        get { return _minionsList; }
        set { _minionsList = value; }
    }

    public void StartNewMinionList()
    {
        _minionsList = new List<GameObject>();
    }

    public void CreateNewMinion(GameObject preFab, Vector3 pos, HeroScript heroFather, bool isTeamOne)
    {
        GameObject obj = Instantiate(preFab, pos, Quaternion.identity);
        SetPropertiesNewMinion(obj, heroFather, isTeamOne);
        AddMinionToList(obj);

    }

    private void SetPropertiesNewMinion(GameObject obj, HeroScript heroFather, bool isTeamOne)
    {
        obj.GetComponent<MinionIA>().HeroAlly = heroFather;
        obj.transform.SetParent(this.transform.parent);
        obj.layer = this.gameObject.layer;

        if (isTeamOne)
        {
            obj.GetComponent<MeshRenderer>().material = _material1;
        }
        else
        {
            obj.GetComponent<MeshRenderer>().material = _material2;
        }

    }

    public void KillAllMinions()
    {
        foreach (GameObject minion in _minionsList)
        {
            minion.GetComponent<MinionIA>().DieMinion(this.gameObject);
        }
    }

    public void RemoveMinion(GameObject obj)
    {
        _minionsList.Remove(obj);
    }

    private void AddMinionToList(GameObject obj)
    {
        _minionsList.Add(obj);
    }

}
