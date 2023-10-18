using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEditor.Progress;

public class HeroScript : MonoBehaviour, ICharacterInterface
{
    [SerializeField] private bool _isTeamOne = false;
    [SerializeField] private float _speed = default;
    [SerializeField] private int _lifeTotalHero = 0;
    [SerializeField] private int _lifeTotalMinion = 0;
    [SerializeField] private int _totalMinions = 0;
    [SerializeField] private GameObject _minionPrefab;
    [SerializeField] private LayerMask _layerNodes;
    [SerializeField] private LayerMask _layerMinionEnemy;
    [SerializeField] private LayerMask _maskToAvoid;

    [SerializeField] private Material _material1;
    [SerializeField] private Material _material2;

    private List<GameObject> _minionsList;
    private int _life;
    private GameObject _stateColorObj;
    private readonly float _radiusBehavior = 4;
    private bool _enemyHeroFound = false;
    private GameObject _objetiveToGo;
    private GameObject _heroEnemy;

    // NodesMovement
    private Node _endNode = default;
    private Node _nodeActualPos = default;
    private List<Node> _listNodesForWayPoint = default;

    // FSMHero
    private FSM<StateHeroEnum> _fsm;
    private FSMHyperHeal<StateHeroEnum> _hyperHealFSM;
    private FSMMoveHero<StateHeroEnum> _moveHeroFSM;
    private FSMFocusToMinion<StateHeroEnum> _focusToMinionFSM;
    private FSMAllBlocking<StateHeroEnum> _allBlockingFSM;
    private FSMRandomFocusToHero<StateHeroEnum> _randomFSM;
    private StateHeroEnum[] _heroStateList;
    private Dictionary<StateHeroEnum, int> _dicStatesForRandom = new Dictionary<StateHeroEnum, int>();
    private int _totalWeight = 0;
    private bool _isRandomStarted = false;

    // Guizmo
    private readonly float _radiusGuizmo = 0.6f;
    private readonly Vector3 _offsetGuizmo = new Vector3(0, 1.3f, 0);

    //Cd for skills
    private float _timeSkill = 0;
    private float _timeSkillTotal = 2;

    void Start()
    {
        SetUpHero();
    }

    private void Update()
    {
        if (_fsm != null)
        {
            _fsm.OnUpdate();
        }
        if (EnemyHeroFound)
        {
            SetNextState();
        }
    }

    #region Get/Set
    
    public GameObject ObjectiveToGo
    {
        get { return _objetiveToGo; }
        set { _objetiveToGo = value; }
    }

    public GameObject HeroEnemy
    {
        get { return _heroEnemy; }
        set { _heroEnemy = value; }
    }

    public List<GameObject> MinionsList
    {
        get { return _minionsList; }
        set { _minionsList = value; }
    }

    public bool EnemyHeroFound
    {
        get { return _enemyHeroFound; }
        set { _enemyHeroFound = value; }
    }

    public float RadiusBehavior
    {
        get { return _radiusBehavior; }
    }

    public Node EndNode
    {
        get { return _endNode; }
        set { _endNode = value; }
    }

    public float Speed
    {
        get { return _speed; }
    }

    public LayerMask MaskToAvoid
    {
        get { return _maskToAvoid; }
    }

    public LayerMask LayerMinionEnemy
    {
        get { return _layerMinionEnemy; }
    }

    public LayerMask LayerNodes
    {
        get { return _layerNodes; }
    }

    public List<Node> ListNodesForWayPoint
    {
        get { return _listNodesForWayPoint; }
        set { _listNodesForWayPoint = value; }
    }

    public Node NodeActualPos
    {
        get { return _nodeActualPos; }
        set { _nodeActualPos = value; }
    }

    public int LifeTotalMinion
    {
        get { return _lifeTotalMinion; }
    }

    #endregion

    #region Functions

    public void AttackedFocus(GameObject obj)
    {
        ObjectiveToGo = obj;
    }

    public void SetEnemyFound(GameObject obj)
    {
        EnemyHeroFound = true;
        HeroEnemy = obj;
        this.transform.LookAt(obj.transform);
    }

    public void SetColorForState(StateHeroEnum index)
    {
        switch (index)
        {
            case StateHeroEnum.hyperHeal:
                _stateColorObj.GetComponent<Renderer>().material.SetColor("_Color", Color.blue);
                break;
            case StateHeroEnum.moveHero:
                _stateColorObj.GetComponent<Renderer>().material.SetColor("_Color", Color.green);
                break;
            case StateHeroEnum.allBlocking:
                _stateColorObj.GetComponent<Renderer>().material.SetColor("_Color", Color.red);
                break;
            case StateHeroEnum.focusToMinion:
                _stateColorObj.GetComponent<Renderer>().material.SetColor("_Color", Color.yellow);
                break;
            case StateHeroEnum.random:
                _stateColorObj.GetComponent<Renderer>().material.SetColor("_Color", Color.white);
                break;
            default:
                _stateColorObj.GetComponent<Renderer>().material.SetColor("_Color", Color.green);
                break;
        }
    }

    public void SetStateFSM(StateHeroEnum index)
    {
        SetColorForState(index);
        _fsm.Transition(index);
    }

    public void GetDamage(int num)
    {
        _life = _life - num;
        if (_life <= 0)
        {
            this.Die();
        }
    }

    public void Die()
    {
        KillAllMinions();
        Destroy(this.gameObject);
    }

    public void RemoveMinion(GameObject obj)
    {
        _minionsList.Remove(obj);
    }

    private void SetNextState()
    {
        if (_timeSkill > 0)
        {
            _timeSkill -= Time.deltaTime;
        }
        else
        {
            if (!_isRandomStarted)
            { 
                SetStateFSM(StateHeroEnum.moveHero); // if we found enemy then MoveHero State is just for iddle form and is used as transition between others states.
                SetStateFSM(SetWeightForStates());
                _timeSkill = _timeSkillTotal;
            }
            else
            {
                Debug.Log("_isRandomStarted true");
            }
        }
    }

    private StateHeroEnum SetWeightForStates()
    {
        StateHeroEnum stateToCheckIfRandom = StateHeroEnum.moveHero;

        int _numForRandomState = 0;

        if (_minionsList.Count + 1 <= GetNumberOfEnemysOnAoe())
        {
            _numForRandomState += 10;
        }
        if (_minionsList.Count == _totalMinions / 2)
        {
            _numForRandomState += 10;
        }
        if (_life == _lifeTotalHero / 2)
        {
            _numForRandomState += 10;
        }

        int _numForMovingState = 100 - _numForRandomState - _totalWeight;

        _dicStatesForRandom[StateHeroEnum.random] = _numForRandomState;
        _dicStatesForRandom[StateHeroEnum.moveHero] = _numForMovingState;

        float random = UnityEngine.Random.Range(0, (_numForMovingState + _numForRandomState + _totalWeight) + 1);

        foreach (var dicState in _dicStatesForRandom)
        {
            random -= dicState.Value;
            if (random <= 0)
            {
                stateToCheckIfRandom = dicState.Key;
                break;
            }
        }

        if (stateToCheckIfRandom == StateHeroEnum.random)
        {
            _isRandomStarted = true;
        }

        return stateToCheckIfRandom;
    }

    private int GetNumberOfEnemysOnAoe()
    {
        List<Collider> collNodesList = Physics.OverlapSphere(this.transform.position, _radiusBehavior, _layerMinionEnemy).ToList<Collider>();

        return collNodesList.Count;
    }

    private void CreateMinions(int totalMinions)
    {
        List<Collider> collNodesList = Physics.OverlapSphere(this.transform.position, _radiusBehavior, _layerNodes).ToList<Collider>();

        System.Random rd = new System.Random();
        int rInt = 0;

        if (totalMinions > 0)
        {
            for (var i = 0; i < totalMinions; i++)
            {
                rInt = rd.Next(0, collNodesList.Count);
                Vector3 posV3 = new Vector3(collNodesList[rInt].gameObject.transform.position.x, this.transform.position.y, collNodesList[rInt].gameObject.transform.position.z);
                GameObject obj = Instantiate(_minionPrefab, posV3, Quaternion.identity);
                obj.GetComponent<MinionIA>().HeroAlly = this;
                obj.transform.SetParent(this.transform.parent);
                obj.layer = this.gameObject.layer;

                if (_isTeamOne)
                {
                    obj.GetComponent<MeshRenderer>().material = _material1;
                }
                else
                {
                    obj.GetComponent<MeshRenderer>().material = _material2;
                }

                collNodesList.RemoveAt(rInt);
                AddMinionToList(obj);
            }
        }
        else
        {
            Debug.Log("Need more minions than 0 to spawn");
        }
    }

    private void SetUpHero()
    {
        _heroStateList = new StateHeroEnum[3];
        _heroStateList[0] = StateHeroEnum.hyperHeal;
        _heroStateList[1] = StateHeroEnum.focusToMinion;
        _heroStateList[2] = StateHeroEnum.allBlocking;

        _dicStatesForRandom.Add(StateHeroEnum.random, 0);
        _dicStatesForRandom.Add(StateHeroEnum.moveHero, 0);
        _dicStatesForRandom.Add(StateHeroEnum.hyperHeal, 20);
        _dicStatesForRandom.Add(StateHeroEnum.focusToMinion, 20);
        _dicStatesForRandom.Add(StateHeroEnum.allBlocking, 20);

        foreach (var dicState in _dicStatesForRandom)
        {
            _totalWeight += dicState.Value;
        }

        CreateAndSetFSM();
        _stateColorObj = this.gameObject.transform.GetChild(0).gameObject;
        _fsm = new FSM<StateHeroEnum>(_moveHeroFSM);
        SetColorForState(StateHeroEnum.moveHero);

        _life = _lifeTotalHero;
        _minionsList = new List<GameObject>();

        if (_isTeamOne)
        {
            this.gameObject.layer = LayerMask.NameToLayer("Hero1");
            this.gameObject.GetComponent<MeshRenderer>().material = _material1;
        }
        else
        {
            this.gameObject.layer = LayerMask.NameToLayer("Hero2");
            this.gameObject.GetComponent<MeshRenderer>().material = _material2;
        }

        CreateMinions(_totalMinions);
    }

    private void KillAllMinions()
    {
        foreach (GameObject minion in _minionsList.ToList())
        {
            minion.GetComponent<MinionIA>().DieMinion(this.gameObject);
        }
    }

    private void AddMinionToList(GameObject obj)
    {
        _minionsList.Add(obj);
    }

    private void CreateAndSetFSM()
    {
        _hyperHealFSM = new FSMHyperHeal<StateHeroEnum>(this);
        _moveHeroFSM = new FSMMoveHero<StateHeroEnum>(this);
        _focusToMinionFSM = new FSMFocusToMinion<StateHeroEnum>(this);
        _allBlockingFSM = new FSMAllBlocking<StateHeroEnum>(this);
        _randomFSM = new FSMRandomFocusToHero<StateHeroEnum>(this);

        _moveHeroFSM.SetTransition(StateHeroEnum.hyperHeal, _hyperHealFSM);
        _moveHeroFSM.SetTransition(StateHeroEnum.focusToMinion, _focusToMinionFSM);
        _moveHeroFSM.SetTransition(StateHeroEnum.allBlocking, _allBlockingFSM);
        _moveHeroFSM.SetTransition(StateHeroEnum.random, _randomFSM);
        _moveHeroFSM.SetTransition(StateHeroEnum.moveHero, _moveHeroFSM);

        _hyperHealFSM.SetTransition(StateHeroEnum.moveHero, _moveHeroFSM);
        _allBlockingFSM.SetTransition(StateHeroEnum.moveHero, _moveHeroFSM);
        _focusToMinionFSM.SetTransition(StateHeroEnum.moveHero, _moveHeroFSM);
        _randomFSM.SetTransition(StateHeroEnum.moveHero, _moveHeroFSM);

    }

    #endregion

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        if (NodeActualPos != null)
            Gizmos.DrawSphere(NodeActualPos.transform.position + _offsetGuizmo, _radiusGuizmo);
        if (ListNodesForWayPoint == null) return;

        Gizmos.color = Color.blue;

        foreach (var item in ListNodesForWayPoint)
        {
            if (item != NodeActualPos && item != EndNode.gameObject)
            {
                Gizmos.DrawSphere(item.transform.position + _offsetGuizmo, _radiusGuizmo);
            }
        }

        if (_radiusBehavior > 0)
        {
            Gizmos.DrawWireSphere(this.transform.position, _radiusBehavior);
        }
    }

}

