using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinionManager : MonoBehaviour
{
    private List<GameObject> _minionsList;
    // private bool _isHeroDead; // need this so the code can clean the _minionsList and dont crash with RemoveMinion.

    public List<GameObject> MinionsList
    {
        get { return _minionsList; }
        set { _minionsList = value; }
    }

    public void StartNewMinionList()
    {
        _minionsList = new List<GameObject>();
    }

    public void CreateNewMinion(Vector3 pos, HeroScript heroFather, bool isTeamOne)
    {
        GameObject obj = Instantiate(GameManager.gameManagerStatic.FlyweightManager.minionPrefab, pos, Quaternion.identity);
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
            obj.GetComponent<MeshRenderer>().material = GameManager.gameManagerStatic.FlyweightManager.materialTeamOne;
        }
        else
        {
            obj.GetComponent<MeshRenderer>().material = GameManager.gameManagerStatic.FlyweightManager.materialTeamTwo;
        }
    }

    public void KillAllMinions()
    {
        // _isHeroDead = true;
        foreach (GameObject minion in _minionsList)
        {
            // minion.GetComponent<MinionIA>().DieMinion(this.gameObject);
            minion.GetComponent<MinionIA>().Die();
        }
    }

    public void RemoveMinion(GameObject obj)
    {
        obj.SetActive(false);
        //Debug.Log("RemoveMinion");
        //if (!_isHeroDead)
        //{
        //    _minionsList.Remove(obj);
        //}
    }

    private void AddMinionToList(GameObject obj)
    {
        _minionsList.Add(obj);
    }

}
