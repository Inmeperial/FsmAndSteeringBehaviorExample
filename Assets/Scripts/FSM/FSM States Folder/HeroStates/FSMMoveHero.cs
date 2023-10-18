
using System.Collections.Generic;
using UnityEngine;

public class FSMMoveHero<T> : FSMState<T>
{
    private HeroScript _heroScript;
    private Transform _heroScriptTransform;
    private bool _isEnemyFound = false;
    private readonly float _timeForGetNodeCloserTotal = 0.5f;
    private float _timeForGetNodeCloser = 0;
    private int indexNode = 0;
    private float _time;
    private readonly float timeTotal = 0.3f;
    private List<GameObject> listeNodeEnd;

    public FSMMoveHero(HeroScript heroScript)
    {
        _heroScript = heroScript;
        _heroScriptTransform = _heroScript.gameObject.transform;
        _timeForGetNodeCloser = _timeForGetNodeCloserTotal;
        listeNodeEnd = GameManager.gameManagerStatic.GetListOfNodes();
    }

    public override void OnEnter()
    {
        _isEnemyFound = _heroScript.EnemyHeroFound;
        if (!_isEnemyFound) // If hero found the Hero enemy dont set up nodes for movement
        {
            SetNodesForMove();
        }
    }

    public override void OnUpdate()
    {
        if (!_isEnemyFound) // If hero found the Hero enemy dont move
        {
            WayPointSystem();
            TimerForGetNodeCloser();
            CheckEnemyHeroOnAoEByTime();
        }
    }

    public override void OnSleep()
    {

    }

    private void TimerForGetNodeCloser()
    {
        if (_timeForGetNodeCloser > 0)
        {
            _timeForGetNodeCloser -= Time.deltaTime;
        }
        else
        {
            _heroScript.NodeActualPos = GameManager.gameManagerStatic.GetNodeCloserToObj(_heroScript.gameObject);
            _timeForGetNodeCloser = _timeForGetNodeCloserTotal;
        }
    }

    private void SetNodesForMove()
    {
        _heroScript.EndNode = listeNodeEnd[UnityEngine.Random.Range(1, (listeNodeEnd.Count))].GetComponent<Node>();
        _heroScript.NodeActualPos = GameManager.gameManagerStatic.GetNodeCloserToObj(_heroScript.gameObject);
        _heroScript.ListNodesForWayPoint = GameManager.gameManagerStatic.GetPathTheta(_heroScript.NodeActualPos, _heroScript.EndNode);
        indexNode = 0;
    }

    private void WayPointSystem()
    {
        Vector3 targetPostition = new Vector3(_heroScript.ListNodesForWayPoint[indexNode].transform.position.x, _heroScriptTransform.position.y, _heroScript.ListNodesForWayPoint[indexNode].transform.position.z);
        _heroScriptTransform.LookAt(targetPostition);

        _heroScriptTransform.position = _heroScriptTransform.position + _heroScriptTransform.forward * _heroScript.Speed * Time.deltaTime;

        if (Vector3.Distance(_heroScript.NodeActualPos.gameObject.transform.position, _heroScript.ListNodesForWayPoint[indexNode].gameObject.transform.position) < 0.09f)
        {
            if (indexNode + 1 != _heroScript.ListNodesForWayPoint.Count)
            {
                indexNode++;
                _heroScriptTransform.LookAt((_heroScript.ListNodesForWayPoint[indexNode].transform));
            }
            else
            {
                indexNode = 0;
                SetNodesForMove();
            }
        }
    }

    private void CheckEnemyHeroOnAoEByTime()
    {
        if (_time > 0)
        {
            _time -= Time.deltaTime;
        }
        else
        {
            Collider[] collEnemiesList = Physics.OverlapSphere(_heroScriptTransform.position, _heroScript.RadiusBehavior, _heroScript.LayerMinionEnemy);

            foreach (Collider item in collEnemiesList)
            {
                if (item.GetComponent<HeroScript>())
                {
                    EnemyHeroFound(item.gameObject);
                    break;
                }
            }
            _time = timeTotal;
        }
    }

    private void EnemyHeroFound(GameObject obj)
    {
        _heroScript.ListNodesForWayPoint.Clear();
        _heroScript.SetEnemyFound(obj);
        _isEnemyFound = true;
    }

}
